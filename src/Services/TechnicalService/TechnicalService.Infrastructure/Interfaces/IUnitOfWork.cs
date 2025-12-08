using System.Data;

namespace TechnicalService.Dal.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IBusRepository Buses { get; }
    IExaminationRepository Examinations { get; }
    IMaintenanceRepository Maintenances { get; }
    IRepairPartRepository RepairParts { get; }

    // Доступ до з'єднання та транзакції (для складних операцій)
    IDbConnection? Connection { get; }
    IDbTransaction? Transaction { get; }

    // Управління транзакціями
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}