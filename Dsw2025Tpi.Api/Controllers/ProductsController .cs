using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2025Tpi.Api.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsManagementService _productsManagementService;

        //Este endpoint es para agregar un producto.
        public ProductsController(IProductsManagementService productsManagementService)
        {
            _productsManagementService = productsManagementService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ProductModel.ProductResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductModel.ProductResponse>> AddProduct([FromBody] ProductModel.ProductRequest request)
        {
            try
            {
                var response = await _productsManagementService.AddProduct(request);
                return CreatedAtAction(nameof(AddProduct), new { id = response.Id }, response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Manejo de otros errores
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error inesperado.");
            }
        }


        // Este endpoint es para obtener todos los productos.

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductModel.ProductResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]

        public async Task<ActionResult<List<ProductModel.ProductResponse>>> GetProducts()
        {
            try
            {
                var products = await _productsManagementService.GetProducts();
                if (products == null || !products.Any())
                {
                    return NoContent();
                }
                return Ok(products);
            }
            catch (Exception ex)
            {
                // Manejo de errores
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error inesperado.");
            }
        }



    }
}