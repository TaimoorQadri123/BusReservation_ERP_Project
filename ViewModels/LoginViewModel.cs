using System.ComponentModel.DataAnnotations;

namespace BusReservationERP.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is Required ")]
        [EmailAddress(ErrorMessage = "Email is Incorrect")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is Required ")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}