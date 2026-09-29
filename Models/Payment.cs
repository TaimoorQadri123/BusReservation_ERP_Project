namespace BusReservationERP.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }

        public int BookingId { get; set; }
        public Booking? Booking { get; set; }

        public decimal Amount { get; set; }
        public string Method { get; set; } = "Cash";     // Cash / Card / Online
        public string Status { get; set; } = "Pending";  // Paid / Pending / Partial / Refunded

        public DateTime? PaidAt { get; set; }

    }
}
