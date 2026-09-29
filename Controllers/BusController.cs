using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusReservationERP.Data;
using BusReservationERP.Models;

namespace BusReservationERP.Controllers
{
    [Authorize]
    public class BusController : Controller
    {
        private readonly AppDbContext _context;

        public BusController(AppDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            var buses = await _context.Buses.ToListAsync();
            return View(buses);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Bus bus)
        {
            if (!ModelState.IsValid)
            {
            return View(bus);

            }

            bool exists = await _context.Buses.AnyAsync(b => b.BusNumber == bus.BusNumber);
            if (exists)
            {
                ModelState.AddModelError("BusNumber", "This Bus has already registered.");
                return View(bus);
            }

            _context.Buses.Add(bus);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        //    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
        //ViewBag.DebugErrors = string.Join(" | ", errors);
        //return View(bus);

        
        }

        // GET: /Bus/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var bus = await _context.Buses.FindAsync(id);
            if (bus == null)
            {
                return NotFound();
            }
            return View(bus);
        }

        // POST: /Bus/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Bus bus)
        {
            if (id != bus.BusId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(bus);
            }

            _context.Buses.Update(bus);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
