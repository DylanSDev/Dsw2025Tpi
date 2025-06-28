using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos
{
    public class OrderModel
    {
        public record OrderRequest(Guid CustomerId, string ShippingAddress, string BillingAddress, string Notes, List<OrderItemModel> OrderItems);
        public record OrderItemModel(Guid ProductId, int Quantity);
        public record OrderResponse(Guid Id,
            Guid CustomerId,
            string ShippingAddress,
            string BillingAddress,
            string Notes,
            DateTime Date,
            decimal TotalAmount,
            List<OrderItemResponse> OrderItems,
            string Status);
        public record OrderItemResponse(Guid ProductId, decimal UnitPrice, int Quantity, decimal Subtotal);
    }
}