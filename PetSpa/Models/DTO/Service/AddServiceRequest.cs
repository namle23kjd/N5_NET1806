using PetSpa.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace PetSpa.Models.DTO.Service
{
    public class AddServiceRequest
    {
        public string? ServiceName { get; set; }

        public bool Status { get; set; }

        public string? ServiceDescription { get; set; }

        public byte[]? ServiceImage { get; set; }

        [Required]
        public string? Duration { get; set; }
        public decimal Price { get; set; }

        public Guid? ComboId { get; set; }

    }
}
