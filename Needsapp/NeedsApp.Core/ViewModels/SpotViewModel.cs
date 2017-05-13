using NeedsApp.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NeedsApp.Core.ViewModels
{
    [PropertyChanged.ImplementPropertyChanged]
    public class SpotViewModel:BaseViewModel
    {
        private Spot _spot;

        public Spot Spot { get => _spot; set => _spot = value; }

        public String SpotName {
            get {
                return Spot?.Name ?? string.Empty;
            }
        }

        public Position SpotLocation {
            get {
                return Spot?.SpotLocation;
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


        public ICommand LoadDataCommand { get { return null; } }

        public ICommand SetLocationCommand { get { return null; } }

        public ICommand SaveDataCommand { get { return null; } }

        public ICommand StartWatering { get { return null; } }

        public ICommand StopWatering { get { return null; } }

    }
}
