using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Dsw2025Tpi.Api.Controllers
{
    [ApiController]
    [Route("api/authenticate")] // Añadir la ruta base para el controlador
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
            // Asegúrate de que el rol exista o asigna un rol predeterminado.
            // Si el usuario no tiene roles, GetRolesAsync puede devolver una colección vacía.
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "user";

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
            // La validación de request != null se podría hacer aquí, pero el modelo de enlace
            // de ASP.NET Core ya debería manejar esto y devolver un 400 si el cuerpo es inválido.
            // Si llega hasta aquí y request es null, el problema es más profundo en la configuración de Kestrel o de los formatters.
            if (request == null)
            {
                return BadRequest(new { message = "Los datos de registro son inválidos o incompletos." });
            }

            if (await _userManager.FindByEmailAsync(request.Email) != null)
            {
                return BadRequest(new { message = "El correo electrónico ya está en uso" });
            }

            var user = new IdentityUser
            {
                UserName = request.Username, // Usar el Username del request
                Email = request.Email
            };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                // Devolver errores más específicos de Identity
                var errors = result.Errors.Select(e => e.Description).ToList();
                return BadRequest(new { message = "Error al registrar el usuario", errors = errors });
            }
            await _userManager.AddToRoleAsync(user, "user"); // Asigna el rol 'user' por defecto

            // Después de crear el usuario, intenta loguearlo y generar el token inmediatamente
            var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "user";
            var token = _jwtTokenService.GenerateToken(user.Email, role);

            return Ok(new
            {
                message = "Usuario registrado exitosamente",
                token,
                user = new
                {
                    user.Id,
                    user.Email,
                    Role = role
                }
            });
        }
    }
}