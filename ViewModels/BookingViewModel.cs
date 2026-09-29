using System.ComponentModel.DataAnnotations;

namespace BusReservationERP.ViewModels
{
    public class BookingViewModel
    {
        public int BookingId { get; set; }

        [Required(ErrorMessage = "The Selection Of Trip Is Required")]
        public int TripId { get; set; }

        [Required(ErrorMessage = "The Selection Of Boarding Stop Is Required")]
        public int BoardingStopId { get; set; }

        [Required(ErrorMessage = "The Selection Of Drop Stop Is Required")]
        public int DropStopId { get; set; }

        [Required(ErrorMessage = "The Passenger Name Is Required")]
        public string PassengerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "The Phone Number Is Required")]
        public string PassengerPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "The Seat Number Is Required")]
        [Range(1, 100, ErrorMessage = "Please enter a valid seat number ")]
        public int SeatNumber { get; set; }

        // Dropdown ke liye
        public List<BusReservationERP.Models.Trip>? TripList { get; set; }
    }
}