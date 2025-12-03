using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos
{
    public record ProductModel
    {
        public record ProductRequest
        (
            string Sku,
            string Name, 
            decimal CurrentUnitPrice,
            string InternalCode,
            string Description,
            int StockQuantity
        );

        public record ProductResponse
        (
            Guid Id, 
            string Sku, 
            string Name, 
            decimal CurrentUnitPrice, 
            string InternalCode, 
            string Description, 
            int StockQuantity
        );

        public record ProductResponseUpdate
        (
            Guid Id, 
            string Sku, 
            string Name, 
            decimal CurrentUnitPrice, 
            string InternalCode, 
            string Description, 
            int StockQuantity, 
            bool IsActive
        );

        public record ResponsePaginationAdmin
        (
            List<ProductResponseUpdate> ProductsItems,
            int Total
        );
        public record ProductResponseID(Guid Id);

        public record ProductPaginated
        (
            Guid Id,
            string Sku,
            string Name,
            decimal CurrentUnitPrice,
            string InternalCode,
            string Description,
            int StockQuantity
        );
        public record ResponsePagination
        (
            List<ProductPaginated> ProductsItems,
            int Total
        );
        
        public record FilterProduct
        (
            string? Status,
            string? Search,
            int? PageNumber,
            int? PageSize
        );
        public record FilterProductClient
        (
            string? Search,
            int? PageNumber,
            int? PageSize
        );
    }
}
