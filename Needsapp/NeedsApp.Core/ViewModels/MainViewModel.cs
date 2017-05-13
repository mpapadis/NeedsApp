using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using NeedsApp.Core.Model;
using NeedsApp.Core.Services;
//using Plugin.Permissions.Abstractions;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace NeedsApp.Core.ViewModels
{
    //[PropertyChanged.ImplementPropertyChanged]
    public class MainViewModel : BaseViewModel
    {
        private ILocationService _locationService;
        public ILocationService LocationService {
            get { return _locationService; }
            set { _locationService = value; }
        }

        public MainViewModel(ILocationService locationService)
        {
            LocationService = locationService;
            //Permissions = Mvx.Resolve<IPermissions>();
        }

        [PropertyChanged.AlsoNotifyFor(nameof(Latitude), nameof(Longitude))]
        public Position MyLocation { get; set; }

        public string Latitude { get { return MyLocation?.Latitude?.ToString(CultureInfo.InvariantCulture) ?? string.Empty; } }

        public string Longitude { get { return MyLocation?.Longitude?.ToString(CultureInfo.InvariantCulture) ?? string.Empty; } }

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
            MyLocation = location;
        }

        ICommand _viewSpotListCommand;
        public ICommand ViewSpotListCommand {
            get {
                if (IsBusy)
                    return null;
                _viewSpotListCommand = _viewSpotListCommand ?? new MvxAsyncCommand(async () =>
                 {
                     if (IsBusy) return;
                     Device.BeginInvokeOnMainThread(() => IsBusy = true);
                     await Task.Run(() =>
                     {
                         ShowViewModel<SpotListViewModel>();
                         Task.Delay(300).Wait();
                         Device.BeginInvokeOnMainThread(() => IsBusy = false);
                     });
                 });
                return _viewSpotListCommand;
            }
        }


        ICommand _viewSpotCommand;
        public ICommand ViewSpotCommand {
            get {
                if (IsBusy) return null;
                _viewSpotCommand = _viewSpotCommand ?? new MvxAsyncCommand(async () =>
                {
                    if (IsBusy) return;
                    Device.BeginInvokeOnMainThread(() => IsBusy = true);
                    
                    await Task.Run(() =>
                    {
                        ShowViewModel<SpotViewModel>(new SpotViewModel.NavigationParams() { id = 1 });
                        Task.Delay(300).Wait();
                        Device.BeginInvokeOnMainThread(() => IsBusy = false);
                    });
                });
                return _viewSpotCommand;
            }
        }
    }
}
