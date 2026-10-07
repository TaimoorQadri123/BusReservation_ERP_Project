using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusReservationERP.Data;
using BusReservationERP.Models;

namespace BusReservationERP.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly AppDbContext _context;

        public PaymentController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Payment/Create?groupId=xxx
        public async Task<IActionResult> Create(Guid groupId)
        {
            var groupBookings = await _context.Bookings
                .Include(b => b.Trip)
                .Where(b => b.BookingGroupId == groupId && b.BookingStatus != "Cancelled")
                .ToListAsync();

            if (!groupBookings.Any())
            {
                return NotFound();
            }

            // Agar is group ka payment pehle se bana hua hai, to Edit pe bhej do
            var existingPayment = await _context.Payments
                .FirstOrDefaultAsync(p => p.BookingGroupId == groupId);

            if (existingPayment != null)
            {
                return RedirectToAction(nameof(Edit), new { id = existingPayment.PaymentId });
            }

            var trip = groupBookings.First().Trip;
            decimal suggestedAmount = (trip?.Fare ?? 0) * groupBookings.Count;

            var payment = new Payment
            {
                BookingGroupId = groupId,
                Amount = suggestedAmount,
                Method = "Cash",
                Status = "Pending"
            };

            ViewBag.PassengerName = groupBookings.First().PassengerName;
            ViewBag.SeatCount = groupBookings.Count;
            ViewBag.SeatNumbers = string.Join(", ", groupBookings.Select(b => b.SeatNumber).OrderBy(s => s));

            return View(payment);
        }

        // POST: /Payment/Create
        [HttpPost]
        public async Task<IActionResult> Create(Payment payment)
        {
            if (!ModelState.IsValid)
            {
                return View(payment);
            }

            if (payment.Status == "Paid" || payment.Status == "Partial")
            {
                payment.PaidAt = DateTime.Now;
            }

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Booking");
        }

        // GET: /Payment/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }

            var groupBookings = await _context.Bookings
                .Where(b => b.BookingGroupId == payment.BookingGroupId && b.BookingStatus != "Cancelled")
                .ToListAsync();

            ViewBag.PassengerName = groupBookings.FirstOrDefault()?.PassengerName ?? "";
            ViewBag.SeatCount = groupBookings.Count;
            ViewBag.SeatNumbers = string.Join(", ", groupBookings.Select(b => b.SeatNumber).OrderBy(s => s));

            return View(payment);
        }

        // POST: /Payment/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Payment payment)
        {
            if (id != payment.PaymentId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(payment);
            }

            if (payment.Status == "Paid" || payment.Status == "Partial")
            {
                payment.PaidAt = DateTime.Now;
            }
            else
            {
                payment.PaidAt = null;
            }

            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Booking");
        }
    }
}