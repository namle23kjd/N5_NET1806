namespace PetSpa.Models.DTO
{
    public class UpdateManagerRequestDTO
    {
        public string FullName { get; set; }

        public string Gender { get; set; }

        public string PhoneNumber { get; set; } = null!;
    }
}
