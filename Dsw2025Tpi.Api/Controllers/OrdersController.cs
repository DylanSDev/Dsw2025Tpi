using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Application.Services;
using Dsw2025Tpi.Application.Services.Interfaces;
using Dsw2025Tpi.Domain.Entities;
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
        [Authorize(Roles = "admin,user")]
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

        [HttpGet]
        [Authorize(Roles = "admin")]
        [SwaggerOperation(Summary = "Obtener todas las ordenes del sistema")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderModel.OrderResponse>> GetOrders
            (
                [FromQuery] Guid? customerId,
                [FromQuery] string? status,
                [FromQuery] int? pageNumber,
                [FromQuery] int? pageSize
            )
        {
            var filter = new OrderModel.OrderFilter
            (
                CustomerId: customerId,
                Status: status,
                PageNumber: pageNumber,
                PageSize: pageSize
            );
            var orders = await _orderManagementService.GetOrders(filter);
            if (orders == null || !orders.Any())
            {
                return NoContent();
            }
            return Ok(orders);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "admin")]
        [SwaggerOperation(Summary = "Buscar una orden por id")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<OrderModel.OrderResponse>> GetOrderById(Guid id)
        {
            var order = await _orderManagementService.GetOrderById(id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }

        [HttpPut("{id:guid}/status")]
        [Authorize(Roles = "admin")]
        [SwaggerOperation(Summary = "Actualizar el estado de una orden")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<OrderModel.OrderResponse>> UpdateOrderState([FromBody] OrderModel.UpdateOrderRequest request,[FromRoute] Guid id)
        {
            var updatedOrder = await _orderManagementService.UpdateOrderState(request, id);
            return Ok(updatedOrder);
        }
    }
}