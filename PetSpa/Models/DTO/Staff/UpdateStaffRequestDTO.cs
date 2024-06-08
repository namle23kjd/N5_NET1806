namespace PetSpa.Models.DTO.Staff
{
    public class UpdateStaffRequestDTO
    {
        public string? FullName { get; set; }
        public string? Gender { get; set; }

        public Guid ManagerManaId { get; set; }
    }
}
