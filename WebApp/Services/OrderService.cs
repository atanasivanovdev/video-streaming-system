using System.Net.Http.Json;
using WebApp.Models;

namespace WebApp.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;

        public OrderService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<OrderResult> GetOrders(string userId)
        {
            var result = await _httpClient.GetAsync($"gateway/order/{userId}");

            OrderResult ordersResult = new OrderResult();

            ordersResult.Successful = result.IsSuccessStatusCode;

            if (!ordersResult.Successful)
            {
                var error = await result.Content.ReadAsStringAsync();
                ordersResult.Error = error;
                return ordersResult;
            }

            ordersResult.Orders = await result.Content.ReadFromJsonAsync<List<OrderModel>>();

            return ordersResult;
        }
    }
}
