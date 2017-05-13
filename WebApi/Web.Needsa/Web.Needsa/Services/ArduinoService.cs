using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Web.Needsa.Data;
using Web.Needsa.Models.Dto;

namespace Web.Needsa.Services
{
    public class ArduinoService
    {
        private ApplicationDbContext _db;

        public ArduinoService(ApplicationDbContext applicationDbContext)
        {
            _db = applicationDbContext;
        }

        public async Task<bool> SendOpenCLoseCommand(OpenCloseCommandDto openCloseCommandDto)
        {
            bool waterStatus = openCloseCommandDto.StationStatus;
            var arduinoStation = _db.ArduinoStations.FirstOrDefault(x => x.Id == openCloseCommandDto.StationId);
            if (arduinoStation.WaterStatus != waterStatus)
            {
                arduinoStation.WaterStatus = waterStatus;
                _db.SaveChanges();
                // ... Use HttpClient.
                var urlWaterCommand = "";
                if (arduinoStation.WaterStatus)
                {
                    urlWaterCommand = $"{arduinoStation.Uri}wateron/";
                }
                else
                {
                    urlWaterCommand = $"{arduinoStation.Uri}wateroff/";
                }
                using (HttpClient client = new HttpClient())
                    using (HttpResponseMessage response = await client.GetAsync(urlWaterCommand))
                        using (HttpContent content = response.Content)
                        {
                            // ... Read the string.
                            string result = await content.ReadAsStringAsync();

                            // ... Display the result.
                            if (result != null &&
                                result.Length >= 50)
                            {
                                Console.WriteLine(result.Substring(0, 50) + "...");
                            }
                        }
            }

            return true;
        }
    }
}
