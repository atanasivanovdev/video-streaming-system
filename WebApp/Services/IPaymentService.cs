using WebApp.Models;

namespace WebApp.Services
{
    public interface IPaymentService
    {
        public Task<PaymentResult> ProcessPayment(PaymentModel paymentModel);
    }
}
