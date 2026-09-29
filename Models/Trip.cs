namespace BusReservationERP.Models
{
    public class Trip
    {
        public int TripId { get; set; }

        public int BusId { get; set; }
        public Bus? Bus { get; set; }

        public int RouteId { get; set; }
        public BusRoute? Route { get; set; }

        public DateTime DepartureDate { get; set; }
        public TimeSpan DepartureTime { get; set; }

        public decimal Fare { get; set; }
        public int AvailableSeats { get; set; }

        public string Status { get; set; } = "Scheduled"; // Scheduled / Full / Departed / Cancelled / Completed

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property — ek Trip ki multiple Bookings ho sakti hain
        public ICollection<Booking>? Bookings { get; set; }

    }
}
