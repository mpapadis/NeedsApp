
using NeedsApp.Core.Model;
using System.Threading;
using System.Threading.Tasks;

namespace NeedsApp.Core.Services
{
    public interface ILocationService
    {
  
        Position CurrentLocation();
        Task<Position> GetCurrentLocationTask(CancellationToken _token);

        double CalcDistance(double lat1, double lng1, double lat2, double lng2);
        double CalcDistance(string lat1, string lng1, string lat2, string lng2);
    }
}
