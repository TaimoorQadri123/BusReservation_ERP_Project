using System.ComponentModel.DataAnnotations;

namespace BusReservationERP.Models
{
    public class BusRoute
    {
        [Key]
        public int RouteId { get; set; }

        public int OriginCityId { get; set; }
        public City? OriginCity { get; set; }

        public int DestinationCityId { get; set; }
        public City? DestinationCity { get; set; }

        public ICollection<Trip>? Trips { get; set; }
        public ICollection<RouteStop>? Stops { get; set; }
    }
}