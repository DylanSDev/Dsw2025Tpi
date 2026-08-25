using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Application.Services.Interfaces;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;

namespace Dsw2025Tpi.Application.Services;

public class DashboardManagementService : IDashboardManagementService
{
    private readonly IRepository _repository;

    public DashboardManagementService(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<DashboardModel.Summary?> SummaryCount()
    {
        var productCount = await _repository.CountAsync<Product>();
        var orderCount = await _repository.CountAsync<Order>();

        return new DashboardModel.Summary(productCount, orderCount);
    }
}