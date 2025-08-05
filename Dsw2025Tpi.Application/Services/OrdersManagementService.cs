using System.Linq.Expressions;
using Azure.Core;
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
                if(itemRequest.Quantity <= 0)
                    throw new ArgumentException("La cantidad de un productos en la orden no puede ser 0.");
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

            var order = new Order
                (
                    DateTime.Now, 
                    request.ShippingAddress, 
                    request.BillingAddress, 
                    request.Notes, 
                    totalAmount
                );
            order.CustomerId = customer.Id;
            order.OrderItems = orderItems;

            await _repository.Add(order);

            var responseItems = order.OrderItems.Select
            (
                oi => new OrderModel.OrderItemResponse
                (
                    oi.ProductId,
                    oi.UnitPrice,
                    oi.Quantity,
                    oi.Subtotal
                )
            ).ToList();

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
            var order = await _repository.First<Order>(p => p.Id == id, include: "OrderItems");
            if (order == null) throw new EntityNotFoundException("Orden no encontrada");

            return new OrderModel.OrderResponse(
                order.Id,
                order.CustomerId,
                order.ShippingAddress,
                order.BillingAddress,
                order.Notes,
                order.CreateDate,
                order.TotalAmount,
                order.OrderItems.Select
                (oi => new OrderItemResponse
                       (
                            oi.ProductId,
                            oi.UnitPrice,
                            oi.Quantity,
                            oi.Subtotal
                       )
                ).ToList(),
                order.Status.ToString()
                );
        }

        public async Task<List<OrderModel.OrderResponse>?> GetOrders(OrderModel.OrderFilter? filter = null)
        {
            IEnumerable<Order>? orders;
            OrderStatus? filterStatus = null;

            if (filter.Status != null)
            {
                if (!Enum.TryParse<OrderStatus>(filter.Status, ignoreCase: true, out var parsedStatus)
                    || !Enum.IsDefined(typeof(OrderStatus), parsedStatus))
                    throw new InvalidStateException("El estado ingresado no es valido");
                else
                    filterStatus = parsedStatus;
            }
            Expression<Func<Order, bool>> predicate = o =>
            (filter.CustomerId == null || o.CustomerId == filter.CustomerId)
             && (filterStatus == null || o.Status.Equals (filterStatus));

            if (filter.CustomerId != null
                 || filter.Status != null)
            {
                orders = await _repository.GetFiltered<Order>(predicate, include: "OrderItems");
            }
            else
            {
                orders = await _repository.GetAll<Order>(include: "OrderItems");
            }

            if (orders == null || !orders.Any())
            {
                throw new EntityNotFoundException("No se encontraron ordenes.");
            }

            int pageNumber = filter.PageNumber ?? 1;
            int pageSize = filter.PageSize ?? 10;   

            pageNumber = Math.Max(1, pageNumber); 
            pageSize = Math.Clamp(pageSize, 1, 100); 
            
            orders = orders
                     .Skip((pageNumber - 1) * pageSize)
                     .Take(pageSize);

            return orders.Select
            (
                p => new OrderModel.OrderResponse
                (
                    p.Id,
                    p.CustomerId,
                    p.ShippingAddress,
                    p.BillingAddress,
                    p.Notes,
                    p.CreateDate,
                    p.TotalAmount,
                    p.OrderItems.Select
                    (oi => new OrderItemResponse
                        (
                            oi.ProductId,
                            oi.UnitPrice,
                            oi.Quantity,
                            oi.Subtotal
                        )
                    ).ToList(),
                    p.Status.ToString()
                )
            ).ToList();
        }
        
        public async Task<OrderModel.OrderResponse> UpdateOrderState(OrderModel.UpdateOrderRequest request, Guid id)
        {
            var order = await _repository.GetById<Order>(id, include: "OrderItems");
            if (order == null) throw new EntityNotFoundException("Orden no encontrada");
            if (!Enum.TryParse<OrderStatus>(request.Status, ignoreCase: true, out var NewStatus)
                || !Enum.IsDefined(typeof(OrderStatus), NewStatus))
                throw new InvalidStateException("El estado ingresado no es valido");
            if (NewStatus == order.Status)
                throw new InvalidStateException("No se puede ingresar el mismo estado en el que ya esta");
            order.Status = NewStatus;
            await _repository.Update(order);

            return new OrderModel.OrderResponse
            (
                order.Id,
                order.CustomerId,
                order.ShippingAddress,
                order.BillingAddress,
                order.Notes,
                order.CreateDate,
                order.TotalAmount,
                order.OrderItems.Select
                (oi => new OrderItemResponse
                       (
                            oi.ProductId,
                            oi.UnitPrice,
                            oi.Quantity,
                            oi.Subtotal
                       )
                ).ToList(),
                order.Status.ToString()
            );
        }
    }
}