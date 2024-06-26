using PetSpa.Models.DTO.PaymentDTO;

namespace PetSpa.Repositories.PaymentRepository
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(PaymentInformationModel model, HttpContext context, string transactionId);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}
