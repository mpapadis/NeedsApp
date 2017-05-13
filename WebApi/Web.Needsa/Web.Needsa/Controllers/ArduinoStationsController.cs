using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web.Needsa.Data;
using Web.Needsa.Models.Db;
using Web.Needsa.Models.Dto;
using Web.Needsa.Services;

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
        public IEnumerable<ArduinoStation> Get()
        {
            return _db.ArduinoStations.ToList();
        }

        // POST: api/ArduinoStations/OpenCloseCommand
        [HttpPost]
        [Route("OpenCloseCommand")]
        public async Task<bool> Post([FromBody]OpenCloseCommandDto openCloseCommandDto)
        {
            var arduinoService = new ArduinoService(_db);
            await arduinoService.SendOpenCLoseCommand(openCloseCommandDto);
            return true;
        }

        //// GET: api/ArduinoStations/[dsadasdd]
        //[HttpGet("{stringArray}", Name = "Get")]
        //public string Get(string stringArray)
        //{
        //    return "value";
        //}


        // GET: api/ArduinoStations/1
        [HttpGet("{id}", Name = "Get")]
        public ArduinoStation Get(int id)
        {
            return _db.ArduinoStations.FirstOrDefault(x => x.Id == id);
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
