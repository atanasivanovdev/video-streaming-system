using CustomerService.Models;
using CustomerService.Services;
using Microsoft.AspNetCore.Mvc;
using Middleware;

namespace CustomerService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IJwtBuilder _jwtBuilder;
        private readonly IEncryptor _encryptor;

        public UserController(UserService userService, IJwtBuilder jwtBuilder, IEncryptor encryptor)
        {
            _userService = userService;
            _jwtBuilder = jwtBuilder;
            _encryptor = encryptor;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            var u = await _userService.GetByEmailAsync(registerDTO.Email);

            if (u != null)
            {
                return BadRequest("User already exists");
            }

            var user = new User
            {
                FirstName = registerDTO.FirstName,
                LastName = registerDTO.LastName,
                Email = registerDTO.Email
            };

            user.SetPassword(registerDTO.Password, _encryptor);

            await _userService.CreateAsync(user);

            return Ok();
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var u = await _userService.GetByEmailAsync(loginDTO.Email);

            if (u == null)
            {
                return BadRequest("User not found");
            }

            var isValid = u.ValidatePassword(loginDTO.Password, _encryptor);

            if (!isValid)
            {
                return BadRequest("Invalid login attempt");
            }

            var token = _jwtBuilder.GetToken(u.Id);

            return Ok(token);
        }

        [HttpGet("validate")]
        public async Task<ActionResult> Validate([FromQuery(Name = "email")] string email,
            [FromQuery(Name = "token")] string token)
        {
            var u = await _userService.GetByEmailAsync(email);

            if (u == null)
            {
                return BadRequest("User not found");
            }

            var userId = _jwtBuilder.ValidateToken(token);

            if (userId != u.Id)
            {
                return BadRequest("Token is not valid");
            }

            return Ok(userId);
        }

        [HttpGet("id")]
        public ActionResult GetUserId()
        {

            if (!HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
            {
                return BadRequest("Authorization header is missing.");
            }

            var token = authorizationHeader.ToString().Split(' ').Last();
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is not provided.");
            }

            var userId = _jwtBuilder.ValidateToken(token);
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Token is not valid");
            }

            return Ok(userId);
        }
    }
}
