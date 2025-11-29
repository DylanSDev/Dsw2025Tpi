using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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

        [HttpPost]
        [Authorize(Roles = "admin")]
        [SwaggerOperation(Summary = "Crea un nuevo producto")]
        [ProducesResponseType(typeof(ProductModel.ProductResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ProductModel.ProductResponse>> AddProduct([FromBody] ProductModel.ProductRequest request)
        {
            var response = await _productsManagementService.AddProduct(request);
            return CreatedAtAction(nameof(AddProduct), new { id = response.Id }, response);
        }

        [HttpGet("admin")]
        [Authorize(Roles = "admin")]
        [SwaggerOperation(Summary = "Buscar Productos por Filtro")]

        public async Task<IActionResult> GetAuthProducts([FromQuery] ProductModel.FilterProduct request)
        {
            var products = await _productsManagementService.GetProductsFiltered(request);
            if (products == null)
            {
                Response.Headers.Append("X-Message", "No hay productos activos");
                return NoContent();
            }
            return Ok(products);
        }


        [HttpGet]
        [SwaggerOperation(Summary = "Lista todos los productos")]
        [ProducesResponseType(typeof(IEnumerable<ProductModel.ProductResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<List<ProductModel.ProductResponse>>> GetProducts()
        {
            var products = await _productsManagementService.GetProducts();
            if (products == null || !products.Any())
            {
                return NoContent();
            }
            return Ok(products);
        }

        [HttpGet("{id:guid}")]
        [SwaggerOperation(Summary = "Busca un producto por Id")]
        [ProducesResponseType(typeof(ProductModel.ProductResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductModel.ProductResponse>> GetProductById(Guid id)
        {
            var product = await _productsManagementService.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPut("{id:guid}")]
        [SwaggerOperation(Summary = "Actualiza un producto")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(typeof(ProductModel.ProductResponseUpdate), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ProductModel.ProductResponseUpdate>> UpdateProduct([FromRoute] Guid id, [FromBody] ProductModel.ProductRequest request)
        {
            var updatedProduct = await _productsManagementService.UpdateProductAsync(request, id);
            return Ok(updatedProduct);
        }

        [HttpPatch("{id:guid}")]
        [SwaggerOperation(Summary = "Deshabilita un producto")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DisableProduct(Guid id)
        {
            await _productsManagementService.DisableProductAsync(id);
            return NoContent();
        }
    }
}