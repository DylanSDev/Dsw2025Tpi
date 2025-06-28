using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;

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
            // 1. Validar la solicitud
            if (request == null ||
                request.CustomerId == Guid.Empty ||
                string.IsNullOrWhiteSpace(request.ShippingAddress) ||
                string.IsNullOrWhiteSpace(request.BillingAddress) ||
                request.OrderItems == null ||
                !request.OrderItems.Any())
            {
                throw new ArgumentException("Los datos de la orden no son válidos o están incompletos.");
            }

            // 2. Verificar que el cliente exista
            var customer = await _repository.GetById<Customer>(request.CustomerId);
            if (customer == null)
            {
                throw new EntityNotFoundException($"Cliente con ID {request.CustomerId} no encontrado.");
            }

            // 3. Verificar stock y calcular subtotales
            var orderItems = new List<OrderItem>();
            decimal totalAmount = 0;

            foreach (var itemRequest in request.OrderItems)
            {
                // 1. Buscamos el producto en el catálogo usando su ID
                var product = await _repository.GetById<Product>(itemRequest.ProductId);

                // 2. Verificamos que exista y esté activo
                if (product == null || !product.IsActive)
                {
                    throw new EntityNotFoundException($"Producto con ID {itemRequest.ProductId} no encontrado o inactivo.");
                }

                // 3. Validamos que la cantidad solicitada no exceda el stock disponible
                if (product.StockQuantity < itemRequest.Quantity)
                {
                    throw new ArgumentException($"Stock insuficiente para el producto {product.Name}. Cantidad disponible: {product.StockQuantity}");
                }

                // 4. Calculamos el subtotal utilizando el precio del producto del catálogo
                var subtotal = itemRequest.Quantity * product.CurrentUnitPrice;
                totalAmount += subtotal;

                // 5. Creamos el OrderItem con la información del producto obtenida del catálogo
                var orderItem = new OrderItem
                {
                    Quantity = itemRequest.Quantity,
                    UnitPrice = product.CurrentUnitPrice, // Usamos el precio del catálogo
                    Subtotal = subtotal,
                    ProductId = product.Id,
                    // No necesitas Name y Description en el OrderItem, ya que no son parte de la entidad de dominio.
                    // Si el TPI lo exige, puedes agregarlos a la entidad OrderItem.
                };
                orderItems.Add(orderItem);

                // 6. Decrementamos el stock del producto
                product.StockQuantity -= itemRequest.Quantity;
                await _repository.Update(product);
            }

            // 4. Creamos la orden
            var order = new Order(DateTime.Now, request.ShippingAddress, request.BillingAddress, request.Notes, totalAmount);
            order.CustomerId = customer.Id;
            order.OrderItems = orderItems;

            // 5. Guardamos la orden y sus items
            await _repository.Add(order);

            // 6. Mapeamos la respuesta
            var responseItems = order.OrderItems.Select(oi => new OrderModel.OrderItemResponse(
                oi.ProductId,
                oi.UnitPrice,
                oi.Quantity,
                oi.Subtotal
            )).ToList();

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
    }
}