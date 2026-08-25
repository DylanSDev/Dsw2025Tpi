using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Dsw2025Tpi.Domain.Entities;

namespace Dsw2025Tpi.Application.Dtos
{
    public class OrderModel
    {
        public record OrderRequest
        (
            [property: JsonPropertyName("customerId")]
            Guid CustomerId,
            string ShippingAddress,
            string BillingAddress,
            string Notes,
            List<OrderItemModel> OrderItems
        );

        public record OrderItemModel
        (
            [property: JsonPropertyName("productId")]
            Guid ProductId,
            int Quantity
        );

        public record OrderResponse
        (
            Guid Id,
            Guid CustomerId,
            string ShippingAddress,
            string BillingAddress,
            string Notes,
            DateTime Date,
            decimal TotalAmount,
            List<OrderItemResponse> OrderItems,
            string Status
        );

        public record UpdateOrderRequest
        (
            string Status
        );

        public record OrderFilter
        (
            Guid? CustomerId,
            string? Status,
            int? PageNumber,
            int? PageSize
        );
        public record OrderItemResponse
        (
            Guid ProductId,
            decimal UnitPrice,
            int Quantity, 
            decimal Subtotal
        );
    }
}
