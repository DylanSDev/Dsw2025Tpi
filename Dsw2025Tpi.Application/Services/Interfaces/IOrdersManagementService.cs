using Dsw2025Tpi.Application.Dtos;

namespace Dsw2025Tpi.Application.Services.Interfaces
{
    public interface IOrderManagementService
    {
        Task<OrderModel.OrderResponse> AddOrder(OrderModel.OrderRequest request);
    }
}