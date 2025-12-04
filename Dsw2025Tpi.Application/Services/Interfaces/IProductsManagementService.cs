using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Domain.Entities;

namespace Dsw2025Tpi.Application.Services.Interfaces
{
    public interface IProductsManagementService
    {
        Task<ProductModel.ProductResponse> AddProduct(ProductModel.ProductRequest request);

        Task<ProductModel.ProductResponseUpdate>? GetProductById(Guid id);

        Task<List<ProductModel.ProductResponseUpdate>?> GetProducts();

        Task<ProductModel.ResponsePaginationAdmin?> GetProductsFiltered(ProductModel.FilterProduct request);

        Task<ProductModel.ResponsePagination?> GetProductsFilteredClient(ProductModel.FilterProductClient? request);

        Task<bool> ToggleProductStatustAsync(Guid id);

        Task<ProductModel.ProductResponseUpdate> UpdateProductAsync(ProductModel.ProductRequest request, Guid id);
    }
}