using System.Collections.Generic;
using System.Threading.Tasks;
using NeedsApp.Core.Model;

namespace NeedsApp.Core.Services
{
    public interface IHttpService
    {
        Task<List<ArduinoStation>> GetArduinoStationList();
        Task<bool> SendOpenCloseCommand(OpenCloseCommandDto openCloseCommandDto);
    }
}