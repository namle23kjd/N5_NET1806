using System.ComponentModel.DataAnnotations;

namespace PetSpa.Models.DTO
{
    public class AddAccountRequestDTO
    {
        [Required]
        [MinLength(3, ErrorMessage = "UserName has to be a minimum of character 5")]
        public string UserName { get; set; }

        [Required]
        [MinLength(3, ErrorMessage = "Password has to be a minimum of character 6")]
        public string PassWord { get; set; }

        public bool Status { get; set; }

        public string Role { get; set; }

        public string? ForgotPasswordToken { get; set; }
    }
}
