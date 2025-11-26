using MySql.Data.MySqlClient;
using System.Data;
using TechnicalService.Core.Interfaces;
using TechnicalService.Dal.Data;

namespace TechnicalService.Dal.Implementations;

public class UnitOfWork : IUnitOfWork
{
    private readonly DapperContext _context;
    private IDbConnection? _connection;
    private IDbTransaction? _transaction;
    private bool _disposed;

    public IBusRepository Buses { get; }
    public IExaminationRepository Examinations { get; }
    public IMaintenanceRepository Maintenance { get; }
    public IRepairPartRepository RepairParts { get; }

    public UnitOfWork(DapperContext context)
    {
        _context = context;
        Buses = new BusRepository(context);
        Examinations = new ExaminationRepository(context);
        Maintenance = new MaintenanceRepository(context);
        RepairParts = new RepairPartRepository(context);
    }

    public async Task<int> SaveChangesAsync()
    {
        // Dapper не потребує явного SaveChanges, оскільки кожна операція автоматично зберігається
        return await Task.FromResult(0);
    }

    public async Task BeginTransactionAsync()
    {
        _connection = _context.CreateConnection();
        _connection.Open();
        _transaction = _connection.BeginTransaction();
        await Task.CompletedTask;
    }

    public async Task CommitAsync()
    {
        try
        {
            _transaction?.Commit();
        }
        catch
        {
            _transaction?.Rollback();
            throw;
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
            _connection?.Close();
            _connection?.Dispose();
            _connection = null;
        }

        await Task.CompletedTask;
    }

    public async Task RollbackAsync()
    {
        _transaction?.Rollback();
        _transaction?.Dispose();
        _transaction = null;
        _connection?.Close();
        _connection?.Dispose();
        _connection = null;

        await Task.CompletedTask;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _transaction?.Dispose();
            _connection?.Dispose();
        }
        _disposed = true;
    }
}