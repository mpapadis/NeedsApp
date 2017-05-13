using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web.Needsa.Data;
using Web.Needsa.Models.Db;

namespace Web.Needsa.Controllers
{
    [Produces("application/json")]
    [Route("api/ArduinoStations")]
    public class ArduinoStationsController : Controller
    {
        private ApplicationDbContext _db;

        public ArduinoStationsController(ApplicationDbContext applicationDbContext)
        {
            _db = applicationDbContext;
        }

        // GET: api/ArduinoStations
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/ArduinoStations/[dsadasdd]
        [HttpGet("{stringArray}", Name = "Get")]
        public string Get(string stringArray)
        {
            return "value";
        }
        
        // POST: api/ArduinoStations
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }
        
        // PUT: api/ArduinoStations/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }


        // POST: api/ArduinoStations
        [HttpPost]
        [Route("PostArduinoStationVariable")]
        public ArduinoStationVariable PostArduinoStationVariable([FromBody]ArduinoStationVariable value)
        {
            return value;
        }
    }
}
