using PetSpa.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace PetSpa.Models.DTO.Customer
{
    public class AddCusRequestDTO
    {

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
