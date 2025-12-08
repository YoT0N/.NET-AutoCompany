using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// Redis для кешування
var redis = builder.AddRedis("redis")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("redis-data");

// MySQL для TechnicalService
var mysqlTechnical = builder.AddMySql("mysql-technical")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("technical-mysql-data");

var transportServiceDb = mysqlTechnical.AddDatabase("transportservicedb");

// MySQL для RoutingService
var mysqlRouting = builder.AddMySql("mysql-routing")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("routing-mysql-data");

var routesDb = mysqlRouting.AddDatabase("routesdb");

// MongoDB для PersonnelService
var mongoPersonnel = builder.AddMongoDB("mongo-personnel")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("personnel-mongo-data");

var personnelDb = mongoPersonnel.AddDatabase("personneldb");

// TechnicalService API
var technicalApi = builder.AddProject<Projects.TechnicalService_Api>("technicalservice-api")
    .WithReference(transportServiceDb)
    .WaitFor(transportServiceDb);

// RoutingService API з Redis
var routingApi = builder.AddProject<Projects.RoutingService_API>("routing-api")
    .WithReference(routesDb)
    .WithReference(redis)
    .WaitFor(routesDb)
    .WaitFor(redis);

// PersonnelService API
var personnelApi = builder.AddProject<Projects.PersonnelService_API>("personnel-api")
    .WithReference(personnelDb)
    .WaitFor(personnelDb);

// AggregatorService з Redis та посиланнями на інші сервіси
var aggregator = builder.AddProject<Projects.AggregatorService>("aggregator")
    .WithReference(technicalApi)
    .WithReference(routingApi)
    .WithReference(personnelApi)
    .WithReference(redis)
    .WaitFor(technicalApi)
    .WaitFor(routingApi)
    .WaitFor(personnelApi)
    .WaitFor(redis);

// API Gateway
var gateway = builder.AddProject<Projects.ApiGateway>("gateway")
    .WithReference(technicalApi)
    .WithReference(routingApi)
    .WithReference(personnelApi)
    .WithReference(aggregator)
    .WaitFor(aggregator);

builder.Build().Run();