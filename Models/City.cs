using System.ComponentModel.DataAnnotations;

namespace BusReservationERP.Models
{
    public class City
    {
        [Key]
        public int CityId { get; set; }

        public string CityName { get; set; } = string.Empty;
    }
}