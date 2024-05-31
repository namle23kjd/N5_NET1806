using PetSpa.Models.Domain;

namespace PetSpa.Models.DTO.Service
{
    public class AddServiceRequest
    {
        public string? ServiceName { get; set; }

        public bool Status { get; set; }

        public string? ServiceDescription { get; set; }

        public byte[]? ServiceImage { get; set; }

        public Guid ComboId { get; set; }

    }
}
