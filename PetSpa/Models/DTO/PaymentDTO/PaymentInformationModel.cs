namespace PetSpa.Models.DTO.PaymentDTO
{
    public class PaymentInformationModel
    {
        public Guid CusId { get; set; }
        public List<Guid> BookingIds { get; set; }
        public string OrderType { get; set; }
        public double Amount { get; set; }
        public string OrderDescription { get; set; }
        public string Name { get; set; }
        public string ReturnUrl { get; set; } // Thêm thuộc tính này
    }
}
