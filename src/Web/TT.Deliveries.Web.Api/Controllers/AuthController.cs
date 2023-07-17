using Microsoft.AspNetCore.Mvc;
using TT.Deliveries.Web.Api.Authentication;

namespace TT.Deliveries.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJwtTokenGenerator _tokenGenerator;

        public AuthController(IJwtTokenGenerator tokenGenerator)
        {
            _tokenGenerator = tokenGenerator;
        }

        [HttpPost("login")]
        public IActionResult Login(string userId, string role)
        {
            // Validate user credentials

            // Generate JWT token
            var token = _tokenGenerator.GenerateToken(userId, role);

            // Return token to the client
            return Ok(new { token });
        }
    }
}
