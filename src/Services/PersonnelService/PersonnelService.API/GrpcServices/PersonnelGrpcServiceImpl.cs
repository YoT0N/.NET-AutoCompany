using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using PersonnelService.Application.TodoPersonnel.Queries.GetAllPersonnel;
using PersonnelService.Application.TodoPersonnel.Queries.GetPersonnelById;
using PersonnelService.Application.TodoWorkShifts.Queries.GetWorkShiftsByPersonnel;
using PersonnelService.Grpc;
using Google.Protobuf.WellKnownTypes;

namespace PersonnelService.API.GrpcServices;

public class PersonnelGrpcServiceImpl : PersonnelGrpcService.PersonnelGrpcServiceBase
{
    private readonly IMediator _mediator;
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<PersonnelGrpcServiceImpl> _logger;

    public PersonnelGrpcServiceImpl(
        IMediator mediator,
        IMemoryCache memoryCache,
        IDistributedCache distributedCache,
        ILogger<PersonnelGrpcServiceImpl> logger)
    {
        _mediator = mediator;
        _memoryCache = memoryCache;
        _distributedCache = distributedCache;
        _logger = logger;
    }

    public override async Task<PersonnelResponse> GetPersonnelById(
        GetPersonnelByIdRequest request,
        ServerCallContext context)
    {
        try
        {
            _logger.LogInformation("gRPC: GetPersonnelById {PersonnelId}", request.PersonnelId);

            // Перевіряємо кеш перед запитом до бази
            var cacheKey = $"personnel:id:{request.PersonnelId}";

            // L1 Cache
            if (_memoryCache.TryGetValue(cacheKey, out PersonnelResponse? cached))
            {
                _logger.LogInformation("gRPC Cache HIT (L1) for {CacheKey}", cacheKey);
                context.ResponseTrailers.Add("X-Cache", "HIT-L1");
                return cached!;
            }

            // Запит до бази через MediatR
            var personnel = await _mediator.Send(
                new GetPersonnelByIdQuery(request.PersonnelId.ToString()),
                context.CancellationToken);

            if (personnel == null)
            {
                throw new RpcException(new Status(
                    StatusCode.NotFound,
                    $"Personnel with ID {request.PersonnelId} not found"));
            }

            var response = new PersonnelResponse
            {
                Id = personnel.Id,
                PersonnelId = personnel.PersonnelId,
                FullName = personnel.FullName,
                BirthDate = personnel.BirthDate.ToString("yyyy-MM-dd"),
                Position = personnel.Position,
                Status = personnel.Status,
                Phone = personnel.Phone,
                Email = personnel.Email,
                Address = personnel.Address
            };

            // Записуємо в кеш
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                Size = 1
            };
            _memoryCache.Set(cacheKey, response, cacheOptions);

            context.ResponseTrailers.Add("X-Cache", "MISS");
            return response;
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in gRPC GetPersonnelById");
            throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
        }
    }

    public override async Task<PersonnelListResponse> GetAllPersonnel(
        GetAllPersonnelRequest request,
        ServerCallContext context)
    {
        try
        {
            _logger.LogInformation("gRPC: GetAllPersonnel");

            var query = new GetAllPersonnelQuery
            {
                SearchText = request.SearchText,
                Position = request.Position,
                Status = request.Status,
                Skip = request.Skip,
                Limit = request.Limit == 0 ? 10 : request.Limit
            };

            var personnelList = await _mediator.Send(query, context.CancellationToken);

            var response = new PersonnelListResponse();
            foreach (var person in personnelList)
            {
                response.Personnel.Add(new PersonnelResponse
                {
                    Id = person.Id,
                    PersonnelId = person.PersonnelId,
                    FullName = person.FullName,
                    BirthDate = person.BirthDate.ToString("yyyy-MM-dd"),
                    Position = person.Position,
                    Status = person.Status,
                    Phone = person.Phone,
                    Email = person.Email,
                    Address = person.Address
                });
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in gRPC GetAllPersonnel");
            throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
        }
    }

    public override async Task<WorkShiftListResponse> GetWorkShiftsByPersonnel(
        GetWorkShiftsByPersonnelRequest request,
        ServerCallContext context)
    {
        try
        {
            _logger.LogInformation("gRPC: GetWorkShiftsByPersonnel {PersonnelId}", request.PersonnelId);

            // Кешування
            var cacheKey = $"workshifts:personnel:{request.PersonnelId}";

            if (_memoryCache.TryGetValue(cacheKey, out WorkShiftListResponse? cached))
            {
                _logger.LogInformation("gRPC Cache HIT for {CacheKey}", cacheKey);
                context.ResponseTrailers.Add("X-Cache", "HIT");
                return cached!;
            }

            var workShifts = await _mediator.Send(
                new GetWorkShiftsByPersonnelQuery(request.PersonnelId),
                context.CancellationToken);

            var response = new WorkShiftListResponse();
            foreach (var shift in workShifts)
            {
                response.WorkShifts.Add(new WorkShiftResponse
                {
                    Id = shift.Id,
                    PersonnelId = shift.PersonnelId,
                    ShiftDate = shift.ShiftDate.ToString("yyyy-MM-dd"),
                    StartTime = shift.StartTime,
                    EndTime = shift.EndTime,
                    BusNumber = shift.BusNumber,
                    BusBrand = shift.BusBrand ?? "",
                    RouteNumber = shift.RouteNumber,
                    DistanceKm = shift.DistanceKm,
                    Status = shift.Status
                });
            }

            // Кешуємо на 2 хвилини
            _memoryCache.Set(cacheKey, response, TimeSpan.FromMinutes(2));

            context.ResponseTrailers.Add("X-Cache", "MISS");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in gRPC GetWorkShiftsByPersonnel");
            throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
        }
    }
}