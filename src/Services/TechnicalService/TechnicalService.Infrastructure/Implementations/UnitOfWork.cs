using MySqlConnector;
using System.Data;
using TechnicalService.Dal.Interfaces;
using TechnicalService.Dal.Data;
using TechnicalService.Dal.Implementations.Dapper;
using TechnicalService.Dal.Implementations.AdoNet;

namespace TechnicalService.Dal.Implementations;

public class UnitOfWork : IUnitOfWork
{
    private readonly DapperContext _context;
    private IDbConnection? _connection;
    private IDbTransaction? _transaction;
    private bool _disposed;

    // Lazy initialization репозиторіїв
    private IBusRepository? _buses;
    private IExaminationRepository? _examinations;
    private IMaintenanceRepository? _maintenances;
    private IRepairPartRepository? _repairParts;

    public UnitOfWork(DapperContext context)
    {
        _context = context;
    }

    // Properties з lazy initialization
    public IBusRepository Buses =>
        _buses ??= new BusRepositoryAdoNet(_context);

    public IExaminationRepository Examinations =>
        _examinations ??= new ExaminationRepository(_context);

    public IMaintenanceRepository Maintenances =>
        _maintenances ??= new MaintenanceRepository(_context);

    public IRepairPartRepository RepairParts =>
        _repairParts ??= new RepairPartRepository(_context);

    // Властивості для доступу до з'єднання та транзакції
    public IDbConnection? Connection => _connection;
    public IDbTransaction? Transaction => _transaction;

    /// <summary>
    /// Розпочати транзакцію
    /// </summary>
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            throw new InvalidOperationException("Transaction already started");
        }

        _connection = _context.CreateConnection();

        if (_connection is MySqlConnection mySqlConnection)
        {
            await mySqlConnection.OpenAsync(cancellationToken);
        }
        else
        {
            _connection.Open();
        }

        _transaction = _connection.BeginTransaction();
    }

    /// <summary>
    /// Зафіксувати транзакцію
    /// </summary>
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("Transaction not started");
        }

        try
        {
            // Перевірка, чи транзакція все ще активна
            if (_transaction.Connection != null)
            {
                _transaction.Commit();
            }
        }
        catch
        {
            await RollbackAsync(cancellationToken);
            throw;
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    /// <summary>
    /// Відкатити транзакцію
    /// </summary>
    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            return; // Нічого відкочувати
        }

        try
        {
            // Перевірка, чи транзакція все ще активна
            if (_transaction.Connection != null)
            {
                _transaction.Rollback();
            }
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    /// <summary>
    /// Очистити ресурси транзакції та з'єднання
    /// </summary>
    private async Task DisposeTransactionAsync()
    {
        if (_transaction != null)
        {
            _transaction.Dispose();
            _transaction = null;
        }

        if (_connection != null)
        {
            if (_connection is MySqlConnection mySqlConnection)
            {
                await mySqlConnection.CloseAsync();
                await mySqlConnection.DisposeAsync();
            }
            else
            {
                _connection.Close();
                _connection.Dispose();
            }

            _connection = null;
        }
    }

    /// <summary>
    /// Dispose pattern
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            // Якщо транзакція все ще активна - відкотити
            if (_transaction?.Connection != null)
            {
                try
                {
                    _transaction.Rollback();
                }
                catch
                {
                    // Ігноруємо помилки при Dispose
                }
            }

            _transaction?.Dispose();
            _connection?.Dispose();
        }

        _disposed = true;
    }
}