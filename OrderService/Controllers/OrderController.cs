using Microsoft.AspNetCore.Mvc;
using OrderService.Models;
using OrderService.Services;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderManagementService _orderService;

        public OrderController(OrderManagementService orderService)
        {
            _orderService = orderService;
        }

        // Get api/order/
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetOrders(string userId)
        {
            try
            {
                List<Order> orders = await _orderService.GetOrdersByUserId(userId);
                List<OrderDTO> orderDTOs = orders.Select(order =>
                {
                    return new OrderDTO
                    {
                        Id = order.Id,
                        PlacedOn = order.PlacedOn,
                        Title = order.Title,
                        ImageURL = order.ImageURL,
                        Amount = order.Amount,
                        Status = order.Status,
                        LastCardDigits = order.LastCardDigits,
                        CardExpiration = order.CardExpiration
                    };
                }).ToList();

                return Ok(orderDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
