using System.Net.Http.Json;
using WebApp.Models;

namespace WebApp.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly HttpClient _httpClient;

        public PaymentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PaymentResult> ProcessPayment(PaymentModel paymentModel)
        {
            var result = await _httpClient.PostAsJsonAsync("gateway/payment", paymentModel);

            PaymentResult paymentResult = new PaymentResult();

            paymentResult.Successful = result.IsSuccessStatusCode;

            if (!paymentResult.Successful)
            {
                var error = await result.Content.ReadAsStringAsync();
                paymentResult.Error = error;
                return paymentResult;
            }

            paymentResult.Payment = await result.Content.ReadFromJsonAsync<PaymentModel>();

            return paymentResult;
        }
    }
}
