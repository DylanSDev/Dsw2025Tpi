using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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
        [SwaggerOperation(Summary = "Se utiliza para iniciar sesión")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Login([FromBody] LoginModel request)
        {
            if (request.Email == "test@admin.com" && request.Password == "123456")
            {
                var token = _jwtTokenService.GenerateToken(request.Email, "admin");
                return Ok(new { token });
            }

            if (request.Email == "test@user.com" && request.Password == "123456")
            {
                var token = _jwtTokenService.GenerateToken(request.Email, "user");
                return Ok(new { token });
            }
            return Unauthorized(new { message = "Credenciales inválidas" });
        }
    }
}