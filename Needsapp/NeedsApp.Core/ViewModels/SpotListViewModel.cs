using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using NeedsApp.Core.Model;
using NeedsApp.Core.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace NeedsApp.Core.ViewModels
{
   public class SpotListViewModel:BaseViewModel
    {
        private IHttpService _httpService;
        public IHttpService HttpService {
            get { return _httpService; }
            set { _httpService = value; }
        }

        public SpotListViewModel(IHttpService httpService)
        {
            HttpService = httpService;
        }

        public void Init()
        {
           // LoadData().Wait();
        }



        public ICommand RefreshDataCommand {
            get { return new MvxAsyncCommand(LoadData); }
        }




        private ObservableCollection<ArduinoStation> _arduinoStations = new ObservableCollection<ArduinoStation>();
        public ObservableCollection<ArduinoStation> ArduinoStations {
            get {
                return _arduinoStations;
            }
            set {
                _arduinoStations = value;
                RaiseAllPropertiesChanged();
            }
        }

        private async Task LoadData()
        {
            if (IsBusy)
                return ;

            IsBusy = true;

            try
            {
                var lst = await HttpService.GetArduinoStationList();
                if (lst!=null)
                {
                    _arduinoStations = new ObservableCollection<ArduinoStation>(lst);
                }
                else
                {
                    _arduinoStations = new ObservableCollection<ArduinoStation>();
                }
            }
            catch (Exception ex)
            {
                Mvx.Error(ex.Message);
                return ;
            }
            finally
            {
                IsBusy = false;
            }
            RaiseAllPropertiesChanged();
            
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
                        ShowViewModel<SpotViewModel>(new SpotViewModel.NavigationParams() { id = SelectedItem?.Id??1 });
                        Task.Delay(300).Wait();
                        Device.BeginInvokeOnMainThread(() => IsBusy = false);
                    });
                });
                return _viewSpotCommand;
            }
        }

        public ArduinoStation SelectedItem { get; set; }
    }
}
