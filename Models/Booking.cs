namespace BusReservationERP.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        public int TripId { get; set; }
        public Trip? Trip { get; set; }

        public int BoardingStopId { get; set; }
        public RouteStop? BoardingStop { get; set; }

        public int DropStopId { get; set; }
        public RouteStop? DropStop { get; set; }

        public string PassengerName { get; set; } = string.Empty;
        public string PassengerPhone { get; set; } = string.Empty;
        public int SeatNumber { get; set; }

        public string BookingStatus { get; set; } = "Booked"; // Booked / Cancelled / NoShow

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property — ek Booking ka ek Payment hoga
        public Payment? Payment { get; set; }
    }
}