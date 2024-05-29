using System.ComponentModel.DataAnnotations;

namespace PetSpa.Models.DTO.RegisterDTO
{
    public class RegisterPequestDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        


        [Required]

        [MaxLength(20, ErrorMessage = "PetType has to be a maximum of character 20")]



        public string FullName { get; set; } = null!;
        [Required]

        [MaxLength(20, ErrorMessage = "Gender has to be a maximum of character 20")]

        public string Gender { get; set; } = null!;

        [Required]

        [MaxLength(10, ErrorMessage = "PhoneNumber has to be a maximum of number 10")]

        public string? CusRank { get; set; } = null;
        public string PhoneNumber { get; set; } = null!;
    }
}
