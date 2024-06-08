namespace PetSpa.Models.DTO.Staff
{
    public class AddStaffRequestDTO
    {
        public string? FullName { get; set; }
        public string? Gender { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Guid ManagerManaId { get; set; }

    }
}
