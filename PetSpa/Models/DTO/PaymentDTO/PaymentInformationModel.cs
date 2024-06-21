namespace PetSpa.Models.DTO.PaymentDTO
{
    public class PaymentInformationModel
    {
        public List<Guid> BookingIds { get; set; }
        public string OrderType { get; set; }
        public double Amount { get; set; }
        public string OrderDescription { get; set; }
        public string Name { get; set; }
    }
}
