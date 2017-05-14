using NeedsApp.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NeedsApp.Core.Services
{
    public class HttpService : IHttpService
    {
        string serviceUrl = "http://10.16.248.133/api/";
        HttpClient httpClient; /// = "http://10.16.248.133/api/";

        public HttpService()
        {
            //api/ArduinoStations
            httpClient = new HttpClient();
        }

        public async Task<List<ArduinoStation>> GetArduinoStationList()
        {
            var uri = new Uri(serviceUrl + "ArduinoStations");
            try
            {
                var rsp =  httpClient.GetAsync(uri).Result;
                var cnt = await rsp.Content.ReadAsStringAsync();
                var lst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ArduinoStation>>(cnt);
                return lst;
            }
#pragma warning disable CS0168 // Variable is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // Variable is declared but never used
            {
                
                return null;
            }
        }

        public async Task<bool> SendOpenCloseCommand(OpenCloseCommandDto openCloseCommandDto)
        {
            var uri = new Uri(serviceUrl + "ArduinoStations/OpenCloseCommand");
            try
            {
                var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(openCloseCommandDto), Encoding.UTF8, "application/json");
                var rsp = await httpClient.PostAsync(uri, content);
                var cnt = await rsp.Content.ReadAsStringAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //ArduinoStations/OpenCloseCommand
    }
}
