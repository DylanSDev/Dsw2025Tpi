using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Services;
using Dsw2025Tpi.Application.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Dsw2025Tpi.Api.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardManagementService _dashboardService;

        public DashboardController(IDashboardManagementService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("summary")]
        [SwaggerOperation(Summary = "Se utiliza para contar la cantidad de productos y ordenes en el sistema")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<DashboardModel.Summary>> GetSummary()
        {
            var summary = await _dashboardService.SummaryCount();
            return Ok(summary);
        }
    }
}