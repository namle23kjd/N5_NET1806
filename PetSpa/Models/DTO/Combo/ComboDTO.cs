using PetSpa.Models.Domain;
using PetSpa.Models.DTO.BookingDetail;
using PetSpa.Models.DTO.Service;

namespace PetSpa.Models.DTO.Combo
{
    public class ComboDTO
    {
        public Guid ComboId { get; set; }

        public string ComboType { get; set; } = null!;

        public decimal Price { get; set; }

        public bool Status { get; set; }

        public virtual ICollection<BookingDetailDTO> BookingDetails { get; set; } = new List<BookingDetailDTO>();

        public virtual ICollection<ServiceDTO> Services { get; set; } = new List<ServiceDTO>();

    }
}
