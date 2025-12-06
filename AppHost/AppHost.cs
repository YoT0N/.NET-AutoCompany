using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// === DATABASES ===
var mysqlTechnical = builder.AddMySql("mysql-technical")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var transportServiceDb = mysqlTechnical.AddDatabase("transportservicedb");

var mysqlRouting = builder.AddMySql("mysql-routing")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var routesDb = mysqlRouting.AddDatabase("routesdb");

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
var gateway = builder.AddProject<Projects.ApiGateway>("gateway")
    .WithHttpEndpoint(port: 5000, name: "http")
    .WithReference(technicalApi)
    .WithReference(routingApi)
    .WithReference(personnelApi)
    .WithReference(aggregator)
    .WaitFor(aggregator);

builder.Build().Run();