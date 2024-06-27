namespace PetSpa.Models.DTO.UserDTO
{
    public class UpdateUserDTO
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty ;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

    }
}
