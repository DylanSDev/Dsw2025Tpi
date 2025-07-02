using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Validations.Rules;
using Swashbuckle.AspNetCore.Annotations;

namespace Dsw2025Tpi.Api.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderManagementService _orderManagementService;

        public OrdersController(IOrderManagementService orderManagementService)
        {
            _orderManagementService = orderManagementService;
        }

        [HttpPost]
        [Authorize(Roles = "dev,user")]
        [SwaggerOperation(Summary = "Crea una nueva orden")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<OrderModel.OrderResponse>> AddOrder([FromBody] OrderModel.OrderRequest request)
        {
            var response = await _orderManagementService.AddOrder(request);
            return CreatedAtAction(nameof(AddOrder), new { id = response.Id }, response);
        }
    }
}