namespace PetSpa.Models.DTO
{
    public class AccountDTO
    {
        public Guid AccId { get; set; }

        public string UserName { get; set; } = null!;

        public string PassWord { get; set; } = null!;

        public bool Status { get; set; }

        public string Role { get; set; } = null!;

        public string? ForgotPasswordToken { get; set; }
    }
}
