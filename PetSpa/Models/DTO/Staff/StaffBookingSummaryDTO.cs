namespace PetSpa.Models.DTO.Staff
{
    public class StaffBookingSummaryDTO
    {
        public Guid StaffId { get; set; }
        public string? StaffName { get; set; }
        public int TotalBooking { get; set; }
    }
}
