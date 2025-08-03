using Dsw2025Tpi.Application.Dtos;

namespace Dsw2025Tpi.Application.Services.Interfaces
{
    public interface IOrderManagementService
    {
        Task<OrderModel.OrderResponse> AddOrder(OrderModel.OrderRequest request);

        Task<List<OrderModel.OrderResponse>?> GetOrders(OrderModel.OrderFilter filter, OrderModel.PageFilter pagefilter);

        Task<OrderModel.OrderResponse> UpdateOrderState(OrderModel.UpdateOrderRequest request, Guid id);

        Task<OrderModel.OrderResponse> GetOrderById(Guid id);
    }
}