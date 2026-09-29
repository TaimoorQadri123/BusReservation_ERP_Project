using System.ComponentModel.DataAnnotations;

namespace BusReservationERP.ViewModels
{
    public class TripViewModel
    {
        public int TripId { get; set; }

        [Required(ErrorMessage = "The Selection of Bus is Required ")]
        public int BusId { get; set; }

        [Required(ErrorMessage = "The Selection of Route is Required")]
        public int RouteId { get; set; }

        [Required(ErrorMessage = "Date is Required")]
        [DataType(DataType.Date)]
        public DateTime DepartureDate { get; set; }

        [Required(ErrorMessage = "Time is Required")]
        [DataType(DataType.Time)]
        public TimeSpan DepartureTime { get; set; }

        [Required(ErrorMessage = "Fare is Required")]
        [Range(1, 100000, ErrorMessage = "Please Proper fare write")]
        public decimal Fare { get; set; }

        // Dropdown lists ke liye — form ko pata hona chahiye konse Buses/Routes available hain
        public List<BusReservationERP.Models.Bus>? BusList { get; set; }
        public List<BusReservationERP.Models.BusRoute>? RouteList { get; set; }
    }
}