using System.ComponentModel.DataAnnotations;

namespace PetSpa.Models.DTO.Pet
{
    public class AddPetRequestDTO
    {

        [Required]
       
        [MaxLength(20, ErrorMessage = "PetType has to be a maximum of character 6")]
        public string PetType { get; set; } = null!;
        [Required]
        
        [MaxLength(20, ErrorMessage = "PetName has to be a maximum of character 6")]
        public string PetName { get; set; } = null!;


        

  
        public decimal? PetWeight { get; set; }

        public decimal? PetHeight { get; set; }
        


    }
}
