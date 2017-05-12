using System.Threading.Tasks;
using Plugin.Geolocator;
using System.Threading;
using MvvmCross.Platform;
using Xamarin.Forms;
using System;
using System.Globalization;
using Plugin.Geolocator.Abstractions;
using NeedsApp.Core.Model;
//using Plugin.Permissions.Abstractions;

namespace NeedsApp.Core.Services
{
    public class LocationService : ILocationService
    {

        private readonly IGeolocator _geolocator;
        public LocationService()
        {
            _geolocator = CrossGeolocator.Current;
            _geolocator.StartListeningAsync(1, 1);
            //Permissions = permissions;
        }

        public Model.Position CurrentLocation()
        {
            if (_geolocator == null) return null;
            if (!_geolocator.IsGeolocationAvailable) return null;
            if (!_geolocator.IsGeolocationEnabled) return null;
            if (!_geolocator.IsListening)
            {
                _geolocator.StartListeningAsync(1, 1);
            }
            if (!_geolocator.IsListening) return null;

            if (!_geolocator.IsGeolocationAvailable)
            {
                try
                {
                    var page = new ContentPage();
                    var r = page.DisplayAlert("Προσοχή", "Η συσκευή δεν διαθέτει GPS ή άλλη αντίστοιχη λειτουργία τοποθεσίας.", "OK");
                }
                catch (Exception ex)
                {
                    if (ex != null && !String.IsNullOrEmpty(ex.Message))
                        Mvx.Error(ex.Message);
                }
                
                return null;
            }
            if (!_geolocator.IsGeolocationEnabled)
            {
                try
                {
                    var page = new ContentPage();
                    var r = page.DisplayAlert("Προσοχή", string.Format("Η λειτουργία τοποθεσίας είναι απενεργοποιημένη.{0}Παρακαλώ ενεργοποιήστε την τοποθεσία και δοκιμάστε ξανά.", System.Environment.NewLine), "OK");
                }
                catch (Exception ex)
                {
                    if (ex != null && !String.IsNullOrEmpty(ex.Message))
                        Mvx.Error(ex.Message);
                }
                return null;
            }
            Plugin.Geolocator.Abstractions.Position p = _geolocator.GetPositionAsync(5000).Result ?? null;
            if (!_geolocator.IsListening)
            {
                _geolocator.StopListeningAsync();
            }
            //return p;
            return new Model.Position() { Latitude = p.Latitude, Longitude = p.Longitude };
        }

        public async Task<Model.Position> GetCurrentLocationTask(CancellationToken _token)
        {
            var isListening = _geolocator.IsListening;
            if (!isListening)
            {
                isListening = await _geolocator.StartListeningAsync(1, 1);
            }
            if (!isListening) return null;

            if (!_geolocator.IsGeolocationAvailable)
            {
                try
                {
                    var p = new ContentPage();
                    var r = p.DisplayAlert("Προσοχή", "Η συσκευή δεν διαθέτει GPS ή άλλη αντίστοιχη λειτουργία τοποθεσίας.", "OK");
                    
                }
                catch (Exception ex)
                {
                    if (ex != null && !String.IsNullOrEmpty(ex.Message))
                        Mvx.Error(ex.Message);
                }
                return null;
                
            }
            if (!_geolocator.IsGeolocationEnabled)
            {
                try
                {
                    var p = new ContentPage();
                    var r = p.DisplayAlert("Προσοχή", string.Format("Η λειτουργία τοποθεσίας είναι απενεργοποιημένη.{0}Παρακαλώ ενεργοποιήστε την τοποθεσία και δοκιμάστε ξανά.", System.Environment.NewLine), "OK");
                    //await Permissions.RequestPermissionsAsync(Permission.Location);
                }
                catch (Exception ex)
                {
                    if (ex != null && !String.IsNullOrEmpty(ex.Message))
                        Mvx.Error(ex.Message);
                }
                return null;
            }
            Plugin.Geolocator.Abstractions.Position position = null;
            try
            {
                position = await _geolocator.GetPositionAsync(-1, _token);
            }
            catch (System.Exception _ex)
            {
                try
                {
                    var p = new ContentPage();
                    var r = p.DisplayAlert("Προσοχή", "Παρουσιάστηκε σφάλμα κατά τη λήψη της τοποθεσίας.", "OK");
                }
                catch (Exception ex)
                {
                    if (ex != null && !String.IsNullOrEmpty(ex.Message))
                        Mvx.Error(ex.Message);
                }
                Mvx.Error(_ex.Message);
            }

            if (!_geolocator.IsListening)
            {
                await _geolocator.StopListeningAsync();
            }
            //return  position??null;
            if (position != null)
            {
                return new Model.Position() { Latitude = position.Latitude, Longitude = position.Longitude };
            }
            else
            {
                return null;
            }

        }


        public const double EarthRadiusInMeters = 6367000.0;

        //public IPermissions Permissions { get; private set; }

        public static double ToRadian(double val) { return val * (Math.PI / 180); }
        public static double DiffRadian(double val1, double val2) { return ToRadian(val2) - ToRadian(val1); }

        //borrowed from http://stackoverflow.com/questions/2010466/how-can-i-determine-the-distance-between-two-sets-of-lattitude-longitude-coordin
        public double CalcDistance(double lat1, double lng1, double lat2, double lng2)
        {
            double radius = EarthRadiusInMeters;
            return radius * 2 * Math.Asin(Math.Min(1, Math.Sqrt((Math.Pow(Math.Sin((DiffRadian(lat1, lat2)) / 2.0), 2.0) + Math.Cos(ToRadian(lat1)) * Math.Cos(ToRadian(lat2)) * Math.Pow(Math.Sin((DiffRadian(lng1, lng2)) / 2.0), 2.0)))));
        }
        public double CalcDistance(string lat1, string lng1, string lat2, string lng2)
        {
            double _lat1, _lng1, _lat2, _lng2;
            if (String.IsNullOrEmpty(lat1) || String.IsNullOrEmpty(lat2) || String.IsNullOrEmpty(lng1) || String.IsNullOrEmpty(lng2))
                return double.MaxValue;

            if (!Double.TryParse(lat1, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out _lat1) || !Double.TryParse(lat2, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out _lat2)
                || !Double.TryParse(lng1, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out _lng1) || !Double.TryParse(lng2, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out _lng2))
            {
                return double.MaxValue;
            }

            return CalcDistance(_lat1, _lng1, _lat2, _lng2);

        }

    }




}
