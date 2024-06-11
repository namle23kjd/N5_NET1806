using System.ComponentModel.DataAnnotations;

namespace PetSpa.Models.DTO.LoginDTO
{
    public class LoginGG
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public required string Email { get; set; }
    }
}
