using CustomerService.Models;
using CustomerService.Services;
using Microsoft.AspNetCore.Mvc;

namespace CustomerService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InboxController : ControllerBase
    {
        private readonly InboxService _inboxService;
        public InboxController(InboxService inboxService)
        {
            _inboxService = inboxService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetMessages(string userId)
        {
            try
            {
                List<Message> messages = await _inboxService.GetMessages(userId);

                return Ok(messages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
