namespace PetSpa.Models.Domain
{
    public class Merchant
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ContactInfo { get; set; }

        public virtual ICollection<PaymentT> Payments { get; set; }
    }
}
