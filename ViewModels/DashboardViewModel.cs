namespace BusReservationERP.ViewModels
{
    public class DashboardViewModel
    {
        // Stat cards
        public int TodayTripsCount { get; set; }
        public int TodayBookingsCount { get; set; }
        public decimal TodayRevenue { get; set; }
        public int AvailableSeatsToday { get; set; }

        // Today's trips table
        public List<DashboardTripRow> TodayTrips { get; set; } = new();

        // Recent bookings feed
        public List<DashboardRecentBooking> RecentBookings { get; set; } = new();

        // Payment breakdown
        public int PaidCount { get; set; }
        public decimal PaidAmount { get; set; }
        public int PendingCount { get; set; }
        public decimal PendingAmount { get; set; }
        public int PartialCount { get; set; }
        public decimal PartialAmount { get; set; }

        // Fleet status
        public int ActiveBuses { get; set; }
        public int MaintenanceBuses { get; set; }
        public int InactiveBuses { get; set; }
        public int TotalBuses { get; set; }
    }

    public class DashboardTripRow
    {
        public int TripId { get; set; }
        public string BusNumber { get; set; } = "";
        public string Route { get; set; } = "";
        public string DepartureTime { get; set; } = "";
        public int BookedSeats { get; set; }
        public int TotalSeats { get; set; }
        public string Status { get; set; } = "";
    }

    public class DashboardRecentBooking
    {
        public string PassengerName { get; set; } = "";
        public int SeatCount { get; set; }
        public string Route { get; set; } = "";
        public string BusNumber { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}