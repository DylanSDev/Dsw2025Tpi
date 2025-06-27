using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos
{
    public record ProductModel
    {
        public record ProductRequest(string Sku, string Name, decimal Price, string InternalCode, string Descripcion, int Stock);

        public record ProductResponse(Guid Id, string Sku, string Name, decimal Price, string InternalCode, string Descripcion, int Stock);

        public record ProductResponseUpdate(Guid Id, string Sku, string Name, decimal Price, string InternalCode, string Descripcion, int Stock, bool IsActive);
        public record ProductResponseID(Guid Id);
    }
}
