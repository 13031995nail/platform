namespace Tmobis.Business.Process.Documents.Providers;

using Tmobis.Business.Process.Documents.Models;

/// <summary>
/// Получение ролевой сводки с проверкой доступа текущего пользователя.
/// </summary>
public interface IWorkspaceDashboardProvider
{
    Task<WorkspaceDashboard> GetDashboard(WorkspaceDashboardQuery query);
}
