using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Domain.Entities;

namespace Dsw2025Tpi.Application.Services
{
    public interface IProductsManagementService
    {
        Task<ProductModel.ProductResponse> AddProduct(ProductModel.ProductRequest request);
        Task<ProductModel.ProductResponseUpdate>? GetProductById(Guid id);
        Task<List<ProductModel.ProductResponseUpdate>?> GetProducts();
        Task<bool> DisableProductAsync(Guid id);
        Task<ProductModel.ProductResponseUpdate> UpdateProductAsync(ProductModel.ProductRequest request, Guid id);

    }
}
