namespace TechnicalService.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IBusRepository Buses { get; }
    IExaminationRepository Examinations { get; }
    IMaintenanceRepository Maintenance { get; }
    IRepairPartRepository RepairParts { get; }

    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}