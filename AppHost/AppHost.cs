using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// === DATABASES ===
// MySQL для TechnicalService
var mysqlTechnical = builder.AddMySql("mysql-technical")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEnvironment("MYSQL_ROOT_PASSWORD", "windowcloud")
    .WithEnvironment("MYSQL_DATABASE", "transportservicedb");

var transportServiceDb = mysqlTechnical.AddDatabase("transportservicedb");

// MySQL для RoutingService
var mysqlRouting = builder.AddMySql("mysql-routing")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEnvironment("MYSQL_ROOT_PASSWORD", "windowcloud")
    .WithEnvironment("MYSQL_DATABASE", "routesdb");

var routesDb = mysqlRouting.AddDatabase("routesdb");

// MongoDB для PersonnelService
var mongoPersonnel = builder.AddMongoDB("mongo-personnel")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var personnelDb = mongoPersonnel.AddDatabase("personneldb");

// === MICROSERVICES ===
var technicalApi = builder.AddProject<Projects.TechnicalService_Api>("technicalservice-api")
    .WithReference(transportServiceDb)
    .WaitFor(transportServiceDb);

var routingApi = builder.AddProject<Projects.RoutingService_API>("routing-api")
    .WithReference(routesDb)
    .WaitFor(routesDb);

var personnelApi = builder.AddProject<Projects.PersonnelService_API>("personnel-api")
    .WithReference(personnelDb)
    .WaitFor(personnelDb);

// === AGGREGATOR SERVICE ===
var aggregator = builder.AddProject<Projects.AggregatorService>("aggregator")
    .WithReference(technicalApi)
    .WithReference(routingApi)
    .WithReference(personnelApi)
    .WaitFor(technicalApi)
    .WaitFor(routingApi)
    .WaitFor(personnelApi);

// === API GATEWAY ===
// ВИПРАВЛЕННЯ: використовуємо WithHttpEndpoint БЕЗ параметра name
// або задаємо УНІКАЛЬНЕ ім'я (не "http")
var gateway = builder.AddProject<Projects.ApiGateway>("gateway")
    .WithHttpEndpoint(port: 5000, name: "gateway-http")  // ← Унікальне ім'я!
    .WithReference(technicalApi)
    .WithReference(routingApi)
    .WithReference(personnelApi)
    .WithReference(aggregator)
    .WaitFor(aggregator);

builder.Build().Run();