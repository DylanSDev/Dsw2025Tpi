using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2025Tpi.Api.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsManagementService _productsManagementService;

        
        public ProductsController(IProductsManagementService productsManagementService)
        {
            _productsManagementService = productsManagementService;
        }

        //Este endpoint es para agregar un producto.
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

        //Endpoint para obtener un producto por ID.
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ProductModel.ProductResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<ProductModel.ProductResponse>> GetProductById(Guid id)
        {
            try
            {
                var product = await _productsManagementService.GetProductById(id);
                if (product == null)
                {
                    return NotFound();
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                // Manejo de errores
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error inesperado.");
            }
        }

        // Endpoint para actualizar un producto.
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ProductModel.ProductResponseUpdate), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<ProductModel.ProductResponseUpdate>> UpdateProduct([FromRoute]Guid id, [FromBody] ProductModel.ProductRequest request)
        {
            try
            {
                var updatedProduct = await _productsManagementService.UpdateProductAsync(request, id);
                if (updatedProduct == null)
                {
                    return NotFound();
                }
                return Ok(updatedProduct);
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

        // Endpoint para deshabilitar un producto.
        [HttpPatch("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> DisableProduct(Guid id)
        {
            try
            {
                await _productsManagementService.DisableProductAsync(id);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // Manejo de errores
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error inesperado.");
            }
        }




    }
}