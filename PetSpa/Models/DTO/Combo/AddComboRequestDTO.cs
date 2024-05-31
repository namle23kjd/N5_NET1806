namespace PetSpa.Models.DTO.Combo
{
    public class AddComboRequestDTO
    {
        public string ComboType { get; set; } = null!;

        public decimal Price { get; set; }

        public bool Status { get; set; }
    }
}
