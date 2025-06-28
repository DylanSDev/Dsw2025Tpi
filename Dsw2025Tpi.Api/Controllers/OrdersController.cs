using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2025Tpi.Api.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderManagementService _orderManagementService;

        public OrdersController(IOrderManagementService orderManagementService)
        {
            _orderManagementService = orderManagementService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderModel.OrderResponse>> AddOrder([FromBody] OrderModel.OrderRequest request)
        {
            try
            {
                var response = await _orderManagementService.AddOrder(request);
                return CreatedAtAction(nameof(AddOrder), new { id = response.Id }, response); // 201 Created
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // 400 Bad Request
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message); // 404 Not Found
            }
            catch (Exception)
            {
                // Devolver 500 Internal Server Error para errores inesperados
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error inesperado al procesar la orden.");
            }
        }
    }
}