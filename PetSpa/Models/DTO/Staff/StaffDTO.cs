using PetSpa.Models.Domain;
using PetSpa.Models.DTO.Account;
using PetSpa.Models.DTO.Booking;
using PetSpa.Models.DTO.BookingDetail;
using PetSpa.Models.DTO.Job;

namespace PetSpa.Models.DTO.Staff
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
