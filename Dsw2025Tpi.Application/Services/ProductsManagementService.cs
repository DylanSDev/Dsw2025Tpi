using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;

namespace Dsw2025Tpi.Application.Services;

public class ProductsManagementService : IProductsManagementService
{
    private readonly IRepository _repository;

    public ProductsManagementService(IRepository repository)
    {
        _repository = repository;
    }

    // Obtener un producto por ID
    public async Task<ProductModel.ProductResponseUpdate>? GetProductById(Guid id)
    {
        var product = await _repository.First<Product>(p => p.Id == id && p.IsActive == true);
        if (product == null) throw new EntityNotFoundException("Producto no encontrado");

        return new ProductModel.ProductResponseUpdate(
            product.Id,
            product.Sku,
            product.Name,
            product.CurrentUnitPrice,
            product.InternalCode,
            product.Description,
            product.StockQuantity,
            product.IsActive
        );
    }

    // Obtener todos los productos
    public async Task<List<ProductModel.ProductResponseUpdate>?> GetProducts()
    {
        var products = await _repository.GetAll<Product>();

        if (products == null || !products.Any() || products.Where(p => p.IsActive) == null)
        {
            throw new EntityNotFoundException("No se encontraron productos activos.");
        }

        return products.Where(p => p.IsActive == true).Select(p => new ProductModel.ProductResponseUpdate(
        p.Id,
        p.Sku,
        p.Name,
        p.CurrentUnitPrice,
        p.InternalCode,
        p.Description,
        p.StockQuantity,
        p.IsActive
    )).ToList();
    }

    // Agregar un nuevo producto
    public async Task<ProductModel.ProductResponse> AddProduct(ProductModel.ProductRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Sku) ||
            string.IsNullOrWhiteSpace(request.InternalCode) ||
            string.IsNullOrWhiteSpace(request.Descripcion) ||
            string.IsNullOrWhiteSpace(request.Name) ||

            request.Stock < 0)
        {
            throw new ArgumentException("Valores para el producto no validos");
        }
        //VALIDACIONES
        if (request.Price <= 0) throw new PriceNullException("El precio del producto no puede ser cero o menor.");
        var exist = await _repository.First<Product>(p => p.Sku == request.Sku);
        if (exist != null) throw new DuplicatedEntityException($"Ya existe un producto con el Sku {request.Sku}");

        var product = new Product(request.Sku, request.InternalCode, request.Name, request.Descripcion, request.Price, request.Stock);

        await _repository.Add(product);
        return new ProductModel.ProductResponse(product.Id, product.Sku, product.Name, product.CurrentUnitPrice, product.InternalCode, product.Description, product.StockQuantity);
    }

    // Deshabilitar un producto por ID
    public async Task<bool> DisableProductAsync(Guid id)
    {
        var product = await _repository.GetById<Product>(id);

        if (product is null || !product.IsActive)
            throw new EntityNotFoundException("Producto no encontrado o ya deshabilitado.");

        product.IsActive = false;
        await _repository.Update(product);
        return true;
    }

    // Actualizar un producto por ID
    public async Task<ProductModel.ProductResponseUpdate> UpdateProductAsync(ProductModel.ProductRequest request, Guid id)
    {
        var product = await _repository.GetById<Product>(id);
        if (product == null)
            throw new EntityNotFoundException($"Producto con ID {id} no encontrado.");
        if (request == null ||
                string.IsNullOrWhiteSpace(request.Sku) ||
                string.IsNullOrWhiteSpace(request.InternalCode) ||
                string.IsNullOrWhiteSpace(request.Name) ||
                request.Price <= 0)
            throw new ArgumentException("Valores para el producto no validos");

        // Actualiza los campos de la entidad
        product.Sku = request.Sku;
        product.InternalCode = request.InternalCode;
        product.Name = request.Name;
        product.Description = request.Descripcion;
        product.CurrentUnitPrice = request.Price;
        product.StockQuantity = request.Stock;

        // Guarda los cambios
        await _repository.Update(product);

        // Mapea a DTO de respuesta
        return new ProductModel.ProductResponseUpdate(
            product.Id,
            product.Sku,
            product.Name,
            product.CurrentUnitPrice,
            product.InternalCode,
            product.Description,
            product.StockQuantity,
            product.IsActive
        );
    }
}