using System.ComponentModel.DataAnnotations;

namespace BusReservationERP.Models
{
    public class RouteStop
    {
        [Key]
        public int RouteStopId { get; set; }

        public int RouteId { get; set; }
        public BusRoute? Route { get; set; }

        public string StopName { get; set; } = string.Empty;

        // Ye batata hai ke raaste mein ye stop kis number pe aata hai
        // (1 = pehla stop / origin, 2 = agla, waghera)
        public int StopOrder { get; set; }
    }
}