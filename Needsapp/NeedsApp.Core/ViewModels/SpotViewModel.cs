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
    public class SpotViewModel:BaseViewModel
    {

        private ILocationService _locationService;
        public ILocationService LocationService {
            get { return _locationService; }
            set { _locationService = value; }
        }

        private Spot _spot;

        public Spot Spot { get => _spot; set => _spot = value; }

        public SpotViewModel(ILocationService locationService)
        {
            LocationService = locationService;
            LoadDataCommand.Execute(null);
        }


        public ICommand GetLocationCommand {
            get {
                return new MvxAsyncCommand(DoSetLocationAsync);
            }
        }

        //public IPermissions Permissions { get; private set; }

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

        public String SpotName {
            get {
                return Spot?.Name ?? string.Empty;
            }
        }

        [PropertyChanged.AlsoNotifyFor(nameof(SpotLongitude), nameof(SpotLatitude))]
        public Position SpotLocation {
            get {
                return Spot?.SpotLocation;
            }
            set {
                Spot.SpotLocation = value;
            }
        }

        
        public string SpotLongitude {
            get {
                return Spot?.SpotLocation?.Longitude?.ToString() ?? string.Empty;
            }
        }

        public string SpotLatitude {
            get {
                return Spot?.SpotLocation?.Latitude?.ToString() ?? string.Empty;
            }
        }


        public ICommand LoadDataCommand { get
                {
                return new MvxAsyncCommand(LoadDataAsync);
            }; } 

        private async Task LoadDataAsync()
        {
            IsBusy = true;
            try
            {
                //if (id == 0)
                //{
                    Spot = await Task.Run(() => new Spot() { ID = 10, Name = "arduino1", SpotLocation = null });
                //}

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                IsBusy = false;
            }
           

        }


        public ICommand SetLocationCommand { get { return null; } }

        public ICommand SaveDataCommand { get { return null; } }

        public ICommand StartWatering { get { return null; } }

        public ICommand StopWatering { get { return null; } }

    }
}
