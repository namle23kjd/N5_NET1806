namespace PetSpa.Models.DTO.Customer
{
    public class CustomerDTO
    {
        public Guid AccId { get; set; }

        public string UserName { get; set; } = null!;

        public string PassWord { get; set; } = null!;

        public bool Status { get; set; }

        public string Role { get; set; } = null!;
    }
}
