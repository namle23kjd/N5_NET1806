namespace PetSpa.Models.DTO.Invoice
{
    public class InvoiceDTO
    {
        public Guid BookingId { get; set; }

        public Guid InvoiceId { get; set; }

        public decimal Price { get; set; }
    }
}
