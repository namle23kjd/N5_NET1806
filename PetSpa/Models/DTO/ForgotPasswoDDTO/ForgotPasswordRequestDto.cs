using System.ComponentModel.DataAnnotations;

namespace PetSpa.Models.DTO.ForgotPasswoDDTO
{
    public class ForgotPasswordRequestDto
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; } 
    }
}
