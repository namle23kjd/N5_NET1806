namespace PetSpa.Models.DTO.Pet
{
    public class PetDTO
    {
        public Guid PetId { get; set; }
        public Guid CusId { get; set; }
        public string PetType { get; set; } = null!;
        public string PetName { get; set; } = null!;
        public string? Image { get; set; }
        public bool Status { get; set; }
        public DateTime? PetBirthday { get; set; }
        public decimal? PetWeight { get; set; }
        public decimal? PetHeight { get; set; }
    }
}
