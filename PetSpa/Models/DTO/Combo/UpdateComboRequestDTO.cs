using PetSpa.Models.Domain;

namespace PetSpa.Models.DTO.Combo
{
    public class UpdateComboRequestDTO
    {
        public string ComboType { get; set; } = null!;

        public decimal Price { get; set; }

        public bool Status { get; set; }
    }
}
