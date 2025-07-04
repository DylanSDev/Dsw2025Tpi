using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2025Tpi.Api.Controllers
{
    public class AuthenticateController : ControllerBase
    {
        private readonly JwtTokenService _jwtTokenService;

        public AuthenticateController(JwtTokenService jwtTokenService)
        {
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel request)
        {
            if (request.Email == "test@dev.com" && request.Password == "123456")
            {
                var token = _jwtTokenService.GenerateToken(request.Email, "dev");
                return Ok(new { token });
            }
            return Unauthorized(new { message = "Credenciales inválidas" });
        }
    }
}