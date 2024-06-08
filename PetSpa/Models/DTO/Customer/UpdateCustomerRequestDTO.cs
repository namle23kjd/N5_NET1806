namespace PetSpa.Models.DTO.Customer
{
    public class UpdateCustomerRequestDTO
    {
        public string FullName { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
    }
}
