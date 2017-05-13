using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using NeedsApp.Core.Model;
using NeedsApp.Core.Services;
using System;
using System.Collections.Generic;
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

        private Spot _spot;

        public Spot Spot { get { return _spot; } set { _spot = value; } }

        public SpotViewModel(ILocationService locationService)
        {
            LocationService = locationService;
        }
        private int? _spotID;

        public void Init(NavigationParams navigation)
        {
            _spotID = navigation.id;
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
                return Spot?.Title ?? string.Empty;
            }
            set {
                Spot.Title = value;

                //if (SaveDataCommand != null && SaveDataCommand.CanExecute(this))
                //    SaveDataCommand.Execute(this);
                Title = value;
                RaisePropertyChanged();
            }
        }


        public int Sensors {
            get {
                return Spot?.Sensors ?? 0;
            }
        }
        
        public string IsAccessibleString {
            get {
                if (IsAccessible)
                    return "Συνδέθηκε";
                else
                    return "Δεν βρέθηκε!";
            }
        }


        public bool IsAccessible {
            get { return Spot?.Accessible ?? false; }
        }


        public Spot.Status WaterStatus {
            get { return Spot?.WaterStatus ?? Spot.Status.closed; }
        }

        public string WaterStatusString {
            get {
                if (WaterStatus == Spot.Status.open)
                {
                    return "Ανοικτό";
                }
                else if (WaterStatus == Spot.Status.closed)
                {
                    return "Κλειστό";
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
                return Spot?.Position;
            }
            set {
                Spot.Position = value;
                //if (SaveDataCommand!=null && SaveDataCommand.CanExecute(this))
                //    SaveDataCommand.Execute(this);
            }
        }


        public string SpotLongitude {
            get {
                return Spot?.Position?.Longitude?.ToString() ?? string.Empty;
            }
        }

        public string SpotLatitude {
            get {
                return Spot?.Position?.Latitude?.ToString() ?? string.Empty;
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
                if (_spotID == null || !_spotID.HasValue || _spotID.Value == 0)
                {
                    Spot =  new Spot() { ID = 0, Title = "arduino1", Position = null , WaterStatus=Spot.Status.closed, Accessible = true, Sensors = 5};
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

        public ICommand StartWatering { get { return new MvxAsyncCommand(async() => await ToggleWateringAsync(true)); } }

        public ICommand StopWatering { get { return new MvxAsyncCommand(async () => await ToggleWateringAsync(false)); } }

        public async Task ToggleWateringAsync(bool start)
        {
            if (IsBusy) return;

            Device.BeginInvokeOnMainThread(() => IsBusy = true);
            Exception myEx = null;
            try
            {
                // api - start - stop
                if (start)
                {
                    Spot.WaterStatus = Spot.Status.open; //replace with load data from api

                }
                else
                {
                    Spot.WaterStatus = Spot.Status.closed; //replace with load data from api
                }

                
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
            public int? id;
            }
    }
}
