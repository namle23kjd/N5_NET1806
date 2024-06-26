namespace PetSpa.Models.DTO.UserDTO
{
    public class ChangUserRoleDTO
    {
        public Guid Id { get; set; }
        public string NewRole { get; set; } = string.Empty;
    }
}
