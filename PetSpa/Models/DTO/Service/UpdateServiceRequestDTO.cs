namespace PetSpa.Models.DTO.Service
{
    public class UpdateServiceRequestDTO
    {
        public string? ServiceName { get; set; }

        public bool Status { get; set; }

        public string? ServiceDescription { get; set; }

        public byte[]? ServiceImage { get; set; }

        public Guid ComboId { get; set; }
    }
}
