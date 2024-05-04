using Microsoft.AspNetCore.Mvc;
using PaymentService.Models;
using PaymentService.Services;

namespace PaymentService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentOrderService _paymentService;

        public PaymentController(PaymentOrderService paymentService)
        {
            _paymentService = paymentService;
        }

        // POST /payments
        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentDTO payment)
        {
            try
            {
                var processedPayment = await _paymentService.ProcessPayment(payment);
                return Ok(processedPayment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }

        // GET /payments/{paymentId}
        [HttpGet("{paymentId}")]
        public async Task<IActionResult> GetPayment(string paymentId)
        {
            try
            {
                var payment = await _paymentService.GetPaymentById(paymentId);
                if (payment == null)
                {
                    return NotFound($"Payment with ID {paymentId} not found.");
                }
                return Ok(payment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
