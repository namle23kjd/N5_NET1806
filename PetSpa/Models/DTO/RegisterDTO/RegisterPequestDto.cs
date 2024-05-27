using System.ComponentModel.DataAnnotations;

namespace PetSpa.Models.DTO.RegisterDTO
{
    public class RegisterPequestDto
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } 
        public string[] Roles { get; set; }
    }
}
