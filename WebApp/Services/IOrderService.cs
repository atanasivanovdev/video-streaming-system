using WebApp.Models;

namespace WebApp.Services
{
    public interface IOrderService
    {
        public Task<OrderResult> GetOrders(string userId);
    }
}
