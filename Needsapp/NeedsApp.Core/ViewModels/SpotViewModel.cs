using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using NeedsApp.Core.Model;
using NeedsApp.Core.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace NeedsApp.Core.ViewModels
{
    [PropertyChanged.ImplementPropertyChanged]
    public class SpotViewModel : BaseViewModel
    {

        private ILocationService _locationService;
        public ILocationService LocationService {
            get { return _locationService; }
            set { _locationService = value; }
        }

        private IHttpService _httpService;
        public IHttpService HttpService {
            get { return _httpService; }
            set { _httpService = value; }
        }

        private ArduinoStation _arduinoStation;

        public ArduinoStation ArduinoStation { get { return _arduinoStation; } set { _arduinoStation = value; RaiseAllPropertiesChanged(); } }

        public SpotViewModel(ILocationService locationService, IHttpService httpService)
        {
            LocationService = locationService;
            HttpService = httpService;

        }
        private int? _arduinoStationID;

        public void Init(NavigationParams navigation)
        {
            _arduinoStationID = navigation.id;
            init();
        }



        private void init()
        {
           LoadDataAsync().Wait() ;
        }

        public ICommand GetLocationCommand {
            get {
                return new MvxAsyncCommand(DoSetLocationAsync);
            }
        }


        private int locationSearchTimeOut = 10000;
        private CancellationTokenSource CancellationTokenSource = null;

        private async Task DoSetLocationAsync()
        {
            if (LocationService != null)
            {


                Exception _ex = null;
                IsBusy = true;
                try
                {
                    //var cts = new CancellationTokenSource(locationSearchTimeOut);
                    CancellationTokenSource = new CancellationTokenSource(locationSearchTimeOut);
                    var t = CancellationTokenSource.Token;

                    var location = await LocationService.GetCurrentLocationTask(t);

                    IsBusy = false;
                    ApplyLocation(location);
                }
                catch (Exception ex)
                {
                    _ex = ex;
                    Mvx.Error(String.Format("Σφάλμα κατά τη λήψη θέσης GPS:{0}", ex.Message));
                }
                finally
                {
                    IsBusy = false;
                }
                if (_ex != null)
                {
                    try
                    {
                        var page = new ContentPage();
                        var result = page.DisplayAlert("Προσοχή", string.Format("Σφάλμα κατά τη λήψη θέσης GPS.{0}{1}", Environment.NewLine, _ex.Message), "OK");
                    }
                    catch (Exception displayalertex)
                    {
                        if (displayalertex != null && !String.IsNullOrEmpty(displayalertex.Message))
                            Mvx.Error(displayalertex.Message);
                    }
                }

            }
        }

        private void ApplyLocation(Position location)
        {
            SpotLocation = location;
        }


        [PropertyChanged.AlsoNotifyFor(nameof(Title))]
        public String SpotName {
            get {
                return ArduinoStation?.Description ?? string.Empty;
            }
            set {
                ArduinoStation.Description = value;

                //if (SaveDataCommand != null && SaveDataCommand.CanExecute(this))
                //    SaveDataCommand.Execute(this);
                Title = value;
                RaisePropertyChanged();
            }
        }


        //public int Sensors {
        //    get {
        //        return ArduinoStation?.Sensors ?? 0;
        //    }
        //}
        
        //public string IsAccessibleString {
        //    get {
        //        if (IsAccessible)
        //            return "Συνδέθηκε";
        //        else
        //            return "Δεν βρέθηκε!";
        //    }
        //}


        //public bool IsAccessible {
        //    get { return ArduinoStation?.Accessible ?? false; }
        //}


        public bool WaterStatus {
            get { return ArduinoStation?.WaterStatus ?? false; }
            set { ArduinoStation.WaterStatus = value; RaisePropertyChanged(); }
        }

        public string WaterStatusString {
            get {
                if (WaterStatus == true)
                {
                    return "Watering";
                }
                else if (WaterStatus == false)
                {
                    return "Waiting";
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }


        [PropertyChanged.AlsoNotifyFor(nameof(SpotLongitude), nameof(SpotLatitude))]
        public Position SpotLocation {
            get {
                Position p = null;
                if(!String.IsNullOrEmpty(ArduinoStation.Location))
                {
                    try
                    {
                        var ar = ArduinoStation.Location.Split(',').ToArray<string>();
                        if (ar!=null && ar.AsEnumerable<string>().Count() == 2)
                        {
                            double lat;
                            double lng;
                            if (double.TryParse(ar[0], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out lat) && double.TryParse(ar[1], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out lng))
                                p = new Position() { Latitude = lat, Longitude = lng};
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    
                }
                return p;
            }
            set {
                if (value != null && value.Latitude.HasValue && value.Longitude.HasValue)
                {
                    //var s = value.Latitude.ToString() + ", " + value.Longitude.ToString();
                    var s= value.Latitude.Value.ToString(CultureInfo.InvariantCulture) + ", " + value.Longitude.Value.ToString(CultureInfo.InvariantCulture);
                    ArduinoStation.Location = s;
                }
                else
                { ArduinoStation.Location = string.Empty; }
                
                //if (SaveDataCommand!=null && SaveDataCommand.CanExecute(this))
                //    SaveDataCommand.Execute(this);
            }
        }


        public string SpotLongitude {
            get {
                return SpotLocation?.Longitude?.ToString() ?? string.Empty;
            }
        }

        public string SpotLatitude {
            get {
                return SpotLocation?.Latitude?.ToString() ?? string.Empty;
            }
        }


        public ICommand LoadDataCommand { get {
                return new MvxAsyncCommand(LoadDataAsync);
            } }

        private async Task  LoadDataAsync()
        {
            if (IsBusy)
                return;

            Exception myEx = null;

            IsBusy = true;
            try
            {
                if (_arduinoStationID == null || !_arduinoStationID.HasValue || _arduinoStationID.Value == 0)
                {
                    ArduinoStation = new ArduinoStation() { Id = 0, Description = "arduino 1", Location = "37.973555, 23.680971", WaterStatus = false };
                }
                else
                {
                    var lst = await HttpService.GetArduinoStationList();
                    if (lst != null )
                    {
                        ArduinoStation = lst.FirstOrDefault(x => x.Id == _arduinoStationID.Value);
                        
                    }
                }

            }
            catch (Exception ex)
            {
                myEx = ex;
                //throw;
            }
            finally
            {
                Device.BeginInvokeOnMainThread(() => IsBusy = false);
                IsBusy = false;
            }

            if (myEx!=null)
            {
                try
                {
                    var p = new ContentPage();
                    await p.DisplayAlert("Σφάλμα", myEx.Message, "ΟΚ");
                }
                catch (Exception ex)
                {
                    Mvx.Exception(ex.Message);
                }
            }

            RaiseAllPropertiesChanged();
        }

        private async Task SaveDataAsync()
        {
            Device.BeginInvokeOnMainThread(() => IsBusy = true);
            Exception myEx = null;

            try
            {
                await Task.Delay(300);
            }
            catch (Exception ex)
            {
                myEx = ex;
                Mvx.Error(ex.Message);
            }
            finally
            {
                Device.BeginInvokeOnMainThread(() => IsBusy = false);
            }

            if (myEx != null)
            {
                var p = new ContentPage();
                await p.DisplayAlert("Σφάλμα κατά την αποθήκευση", myEx.Message, "ΟΚ");
            }
        }

        public ICommand SaveDataCommand { get { return new MvxAsyncCommand(SaveDataAsync); } }

        public ICommand StartWatering { get { return new MvxAsyncCommand(async () => { await ToggleWateringAsync(true); await LoadDataAsync(); }); } }

        public ICommand StopWatering { get { return new MvxAsyncCommand(async () => { await ToggleWateringAsync(false); await LoadDataAsync(); }); } }

        public async Task ToggleWateringAsync(bool status)
        {
            if (IsBusy) return;

            Device.BeginInvokeOnMainThread(() => IsBusy = true);
            Exception myEx = null;
            try
            {
                // api - start - stop
               
                 ArduinoStation.WaterStatus = status; //replace with load data from api

                var r = await HttpService.SendOpenCloseCommand(new OpenCloseCommandDto() { StationId = ArduinoStation.Id, StationStatus = status });
                WaterStatus = status;
                
            }
            catch (Exception ex)
            {
                myEx = ex;
                Mvx.Error(ex.Message);
            }
            finally
            {
                Device.BeginInvokeOnMainThread(() => IsBusy = false);
            }

            if (myEx!= null)
            {
                try
                {
                    var p = new ContentPage();
                    await p.DisplayAlert("Σφάλμα", myEx.Message, "ΟΚ");

                }
                catch (Exception)
                {
                    throw;
                }
            }
        }


        public class NavigationParams 
            {
            public int id { get; set; }
            }
    }
}
