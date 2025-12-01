using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Domain.Entities;

namespace Dsw2025Tpi.Application.Services.Interfaces
{
    public interface IDashboardManagementService
    {
        Task<DashboardModel.Summary?> SummaryCount();
    }
}