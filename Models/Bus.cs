namespace BusReservationERP.Models
{
    public class Bus
    {
        public int BusId { get; set; }
        public string BusNumber { get; set; } = string.Empty;   // e.g. "LEA-2024"
        public string BusType { get; set; } = string.Empty;     // "AC" / "Non-AC"
        public int TotalSeats { get; set; }
        public string Status { get; set; } = "Active";          // Active / Maintenance / Inactive
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property — ek Bus ke multiple Trips ho sakte hain
        public ICollection<Trip>? Trips { get; set; }
    }
}
