using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Application.Services.Interfaces;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using static Dsw2025Tpi.Application.Dtos.OrderModel;

namespace Dsw2025Tpi.Application.Services
{
    public class OrdersManagementService : IOrderManagementService
    {
        private readonly IRepository _repository;

        public OrdersManagementService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<OrderModel.OrderResponse> AddOrder(OrderModel.OrderRequest request)
        {
            if (request == null
                || request.CustomerId == Guid.Empty
                || string.IsNullOrWhiteSpace(request.ShippingAddress)
                || string.IsNullOrWhiteSpace(request.BillingAddress)
                || request.OrderItems == null
                || !request.OrderItems.Any())
            {
                throw new ArgumentException("Los datos de la orden no son válidos o están incompletos.");
            }

            var customer = await _repository.GetById<Customer>(request.CustomerId);
            if (customer == null)
            {
                throw new EntityNotFoundException($"Cliente con ID {request.CustomerId} no encontrado.");
            }

            var orderItems = new List<OrderItem>();
            decimal totalAmount = 0;

            foreach (var itemRequest in request.OrderItems)
            {
                var product = await _repository.GetById<Product>(itemRequest.ProductId);
                if (product == null || !product.IsActive)
                {
                    throw new EntityNotFoundException($"Producto con ID {itemRequest.ProductId} no encontrado o inactivo.");
                }

                if (product.StockQuantity < itemRequest.Quantity)
                {
                    throw new ArgumentException($"Stock insuficiente para el producto {product.Name}. Cantidad disponible: {product.StockQuantity}");
                }

                var subtotal = itemRequest.Quantity * product.CurrentUnitPrice;
                totalAmount += subtotal;

                var orderItem = new OrderItem
                {
                    Quantity = itemRequest.Quantity,
                    UnitPrice = product.CurrentUnitPrice,
                    Subtotal = subtotal,
                    ProductId = product.Id,
                };
                orderItems.Add(orderItem);

                product.StockQuantity -= itemRequest.Quantity;
                await _repository.Update(product);
            }

            var order = new Order(DateTime.Now, request.ShippingAddress, request.BillingAddress, request.Notes, totalAmount);
            order.CustomerId = customer.Id;
            order.OrderItems = orderItems;

            await _repository.Add(order);

            var responseItems = order.OrderItems.Select(oi => new OrderModel.OrderItemResponse(oi.ProductId,
                                                                                               oi.UnitPrice,
                                                                                               oi.Quantity,
                                                                                               oi.Subtotal)).ToList();

            return new OrderModel.OrderResponse(
                order.Id,
                order.CustomerId,
                order.ShippingAddress,
                order.BillingAddress,
                order.Notes,
                order.CreateDate,
                order.TotalAmount,
                responseItems,
                order.Status.ToString()
            );
        }
        
        public async Task<OrderModel.OrderResponse> GetOrderById (Guid id)
        {
            var order = await _repository.First<Order>(p => p.Id == id);
            if (order == null) throw new EntityNotFoundException("Orden no encontrada");

            return new OrderModel.OrderResponse(
                order.Id,
                order.CustomerId,
                order.ShippingAddress,
                order.BillingAddress,
                order.Notes,
                order.CreateDate,
                order.TotalAmount,
                (List<OrderModel.OrderItemResponse>)order.OrderItems,
                order.Status.ToString()
                );
        }

        public async Task<List<OrderModel.OrderResponse>?> GetOrders()

        {
            var orders = await _repository.GetAll<Order>();
            if (orders == null) throw new EntityNotFoundException("No se encontraron ordenes.");
            return orders.Select(p => new OrderModel.OrderResponse(
                p.Id,
                p.CustomerId,
                p.ShippingAddress,
                p.BillingAddress,
                p.Notes,
                p.CreateDate,
                p.TotalAmount,
               p.OrderItems.Select(oi => new OrderItemResponse(
                    oi.ProductId,
                    oi.UnitPrice,
                    oi.Quantity,
                    oi.Subtotal
                    )).ToList(),
                p.Status.ToString())).ToList();
        }

        public async Task<OrderModel.OrderResponse> UpdateOrderState(OrderModel.OrderRequest request, Guid id)
        {
            var order = await _repository.GetById<Order>(id);
            if (order == null) throw new EntityNotFoundException("Orden no encontrada");

            return new OrderModel.OrderResponse(
                order.Id,
                order.CustomerId,
                order.ShippingAddress,
                order.BillingAddress,
                order.Notes,
                order.CreateDate,
                order.TotalAmount,
                (List<OrderModel.OrderItemResponse>)order.OrderItems,
                order.Status+1.ToString()
                );
        }
    }
}