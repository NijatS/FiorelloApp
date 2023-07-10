using System.ComponentModel.DataAnnotations;

namespace Fir.App.ViewModels
{
    public class ResetPasswordVM
    {
        [Required]
        public string Token { get; set; }
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string? ConfirmPassword { get; set; }
    }
}
