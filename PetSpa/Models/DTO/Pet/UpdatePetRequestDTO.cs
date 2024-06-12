using System;
using System.ComponentModel.DataAnnotations;

namespace PetSpa.Models.DTO.Pet
{
    public class UpdatePetRequestDTO
    {
        [Required]
        [MaxLength(20, ErrorMessage = "PetType has to be a maximum of 20 characters")]
        public string PetType { get; set; } = null!;

        [Required]
        [MaxLength(20, ErrorMessage = "PetName has to be a maximum of 20 characters")]
        public string PetName { get; set; } = null!;

        public string? Image { get; set; }
        public DateTime? PetBirthday { get; set; }
        public decimal? PetWeight { get; set; }
        public decimal? PetHeight { get; set; }
    }
}