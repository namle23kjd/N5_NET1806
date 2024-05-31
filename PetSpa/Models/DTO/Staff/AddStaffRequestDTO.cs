using PetSpa.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace PetSpa.Models.DTO.Staff
{
    public class AddStaffRequestDTO
    {
        [Required]
        [MinLength(3, ErrorMessage = "FullName has to be a minimum of character 6")]
        public string FullName { get; set; }

        [Required]
        [RegularExpression("^(Male|Female)$", ErrorMessage = "Gender must be Male or Female")]
        public string Gender { get; set; }

        public Guid AccId { get; set; }

        public Guid StaffId { get; set; }

    }

}
