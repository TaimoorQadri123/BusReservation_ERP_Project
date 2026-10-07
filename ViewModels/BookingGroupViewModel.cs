namespace BusReservationERP.ViewModels
{
    public class BookingGroupViewModel
    {
        public Guid BookingGroupId { get; set; }
        public string PassengerName { get; set; } = string.Empty;
        public string PassengerPhone { get; set; } = string.Empty;
        public List<int> SeatNumbers { get; set; } = new List<int>();
        public BusReservationERP.Models.Trip? Trip { get; set; }
        public BusReservationERP.Models.RouteStop? BoardingStop { get; set; }
        public BusReservationERP.Models.RouteStop? DropStop { get; set; }
        public string BookingStatus { get; set; } = string.Empty;
        public int SampleBookingId { get; set; }

        // Naya: Payment ki info
        public string PaymentStatus { get; set; } = "Not Recorded";
        public decimal? PaymentAmount { get; set; }
    }
}