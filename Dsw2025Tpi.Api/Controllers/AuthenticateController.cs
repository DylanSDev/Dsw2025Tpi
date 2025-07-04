using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Dsw2025Tpi.Api.Controllers
{
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _sigInManager;
        private readonly JwtTokenService _jwtTokenService;

        public AuthenticateController(JwtTokenService jwtTokenService, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> sigInManager)
        {
            _userManager = userManager;
            _sigInManager = sigInManager;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("login")]
        [SwaggerOperation(Summary = "Se utiliza para iniciar sesión")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginModel request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Unauthorized(new { message = "Credenciales inválidas" });
            }

            var result = await _sigInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
            {
                return Unauthorized(new { message = "Credenciales inválidas" });
            }
            var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "user";
            var token = _jwtTokenService.GenerateToken(user.Email, role);
            return Ok(new
            {
                token,
                user = new
                {
                    user.Id,
                    user.Email,
                    Role = role
                }
            });
        }

        [HttpPost("register")]
        [SwaggerOperation(Summary = "Se utiliza para registrar un nuevo usuario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterModel request)
        {
            if (await _userManager.FindByEmailAsync(request.Email) != null)
            {
                return BadRequest(new { message = "El correo electrónico ya está en uso" });
            }
            var user = new IdentityUser
            {
                UserName = request.Username,
                Email = request.Email
            };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new { message = "Error al registrar el usuario", errors = result.Errors });
            }
            await _userManager.AddToRoleAsync(user, "user");
            return Ok(new { message = "Usuario registrado exitosamente" });
        }
    }
}