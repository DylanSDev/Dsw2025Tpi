using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Services;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Dsw2025Tpi.Api.Controllers
{
    [ApiController]
    [Route("api/authenticate")]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _sigInManager;
        private readonly JwtTokenService _jwtTokenService;
        private readonly IRepository _repository;

        public AuthenticateController(JwtTokenService jwtTokenService, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> sigInManager, IRepository repository)
        {
            _userManager = userManager;
            _sigInManager = sigInManager;
            _jwtTokenService = jwtTokenService;
            _repository = repository;
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
            if (request == null)
            {
                return BadRequest(new { message = "Los datos de registro son inválidos o incompletos." });
            }

            if (await _userManager.FindByEmailAsync(request.Email) != null)
            {
                return BadRequest(new { message = "El correo electrónico ya está en uso" });
            }

            string roleToAssign = "user";

            if (User.Identity.IsAuthenticated && User.IsInRole("admin"))
            {
                roleToAssign = "admin";
            }

            var user = new IdentityUser
            {
                UserName = request.Username, 
                Email = request.Email
            };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return BadRequest(new { message = "Error al registrar el usuario", errors = errors });
            }
            await _userManager.AddToRoleAsync(user, roleToAssign);

            if (roleToAssign == "user")
            {
                try
                {
                    if (Guid.TryParse(user.Id, out Guid userGuid))
                    {
                        var newCustomer = new Customer(userGuid, request.Username, request.Email, "xxx xxx-xxxx");

                        await _repository.Add(newCustomer);
                    }

                }
                catch (Exception ex)
                {
                    await _userManager.DeleteAsync(user);
                    return StatusCode(500, new { message = "Error creando el perfil de cliente.", error = ex.Message });
                }
            }
            var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "user";
            var token = _jwtTokenService.GenerateToken(user.Email, role);

            return Ok
            (
                new
                {
                    message = "Usuario registrado exitosamente",
                    token,
                    user = new
                    {
                        user.Id,
                        user.Email,
                        Role = role
                    }
                }
            );
        }
    }
}