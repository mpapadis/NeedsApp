using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web.Needsa.Data;
using Web.Needsa.Models.Db;
using System.Net.Http;
using Web.Needsa.Services;

namespace Web.Needsa.Controllers
{
    public class ArduinoStationsMvcController : Controller
    {
        private readonly ApplicationDbContext _context;
        
        //private readonly string url = "http://192.168.0.177/arduino/webserver/";

        public ArduinoStationsMvcController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: ArduinoStationsMvc
        public async Task<IActionResult> Index()
        {
            return View(await _context.ArduinoStations.ToListAsync());
        }

        // GET: ArduinoStationsMvc/Details/5
        public async Task<IActionResult> Details(int? id, [FromQuery]bool? waterStatus)
        {
            if (id == null)
            {
                return NotFound();
            }

            var arduinoStation = await _context.ArduinoStations.AsNoTracking().SingleOrDefaultAsync(m => m.Id == id);
            if (waterStatus.HasValue) {
                var arduinoService = new ArduinoService(_context);
                var sss = new Models.Dto.OpenCloseCommandDto() { StationId = id.Value, StationStatus = waterStatus.Value };
                await arduinoService.SendOpenCLoseCommand(sss);
            }
            if (arduinoStation == null)
            {
                return NotFound();
            }

            return View(arduinoStation);
        }

        // GET: ArduinoStationsMvc/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ArduinoStationsMvc/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Uri,Location,Description,Id")] ArduinoStation arduinoStation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(arduinoStation);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(arduinoStation);
        }

        // GET: ArduinoStationsMvc/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var arduinoStation = await _context.ArduinoStations.SingleOrDefaultAsync(m => m.Id == id);
            if (arduinoStation == null)
            {
                return NotFound();
            }
            return View(arduinoStation);
        }

        // POST: ArduinoStationsMvc/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Uri,Location,Description,Id")] ArduinoStation arduinoStation)
        {
            if (id != arduinoStation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(arduinoStation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArduinoStationExists(arduinoStation.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(arduinoStation);
        }
        
        
        // GET: ArduinoStationsMvc/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var arduinoStation = await _context.ArduinoStations
                .SingleOrDefaultAsync(m => m.Id == id);
            if (arduinoStation == null)
            {
                return NotFound();
            }

            return View(arduinoStation);
        }

        // POST: ArduinoStationsMvc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var arduinoStation = await _context.ArduinoStations.SingleOrDefaultAsync(m => m.Id == id);
            _context.ArduinoStations.Remove(arduinoStation);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ArduinoStationExists(int id)
        {
            return _context.ArduinoStations.Any(e => e.Id == id);
        }
    }
}
