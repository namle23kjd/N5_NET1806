using PetSpa.Models.Domain;

namespace PetSpa.Models.DTO
{
    public class StaffDTO
    {
        public Guid AccId { get; set; }

        public Guid StaffID { get; set; }

        public string FullName { get; set; }

        public string Gender { get; set; } = null!;

        public AccountDTO Acc { get; set; }
        public virtual ICollection<BookingDTO> Bookings { get; set; } = new List<BookingDTO>();
        public virtual ICollection<JobDTO> Jobs { get; set; } = new List<JobDTO>();
        public virtual ICollection<BookingDetailDTO> BookingDetails { get; set; } = new List<BookingDetailDTO>();

    }
}
