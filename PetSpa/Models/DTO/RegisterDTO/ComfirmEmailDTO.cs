using System.ComponentModel.DataAnnotations;

namespace PetSpa.Models.DTO.RegisterDTO
{
    public class ComfirmEmailDTO
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public required string Email { get; set; }

        public required string Token { get; set; }
    }
}
