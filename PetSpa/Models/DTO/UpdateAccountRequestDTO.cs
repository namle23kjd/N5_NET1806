using System.ComponentModel.DataAnnotations;

namespace PetSpa.Models.DTO
{
    public class UpdateAccountRequestDTO
    {
        [Required]
        [MinLength(3, ErrorMessage = "UserName has to be a minimum of character 5")]
        public string UserName { get; set; }

        [Required]
        [MinLength(3, ErrorMessage = "Password has to be a minimum of character 6")]
        public string PassWord { get; set; }
        [Required]
        [RegularExpression("^(Admin|Staff|Customer|Manager)$", ErrorMessage = "Role must be either 'Admin', 'Staff', 'Customer', or 'Manager'")]
        public string Role { get; set; } = null!;
    }
}
