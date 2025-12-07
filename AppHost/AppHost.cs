using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);


var mysqlTechnical = builder.AddMySql("mysql-technical")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("technical-mysql-data");  

var transportServiceDb = mysqlTechnical.AddDatabase("transportservicedb");

var mysqlRouting = builder.AddMySql("mysql-routing")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("routing-mysql-data");

var routesDb = mysqlRouting.AddDatabase("routesdb");

var mongoPersonnel = builder.AddMongoDB("mongo-personnel")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("personnel-mongo-data");

var personnelDb = mongoPersonnel.AddDatabase("personneldb");

var technicalApi = builder.AddProject<Projects.TechnicalService_Api>("technicalservice-api")
    .WithReference(transportServiceDb)
    .WaitFor(transportServiceDb);

var routingApi = builder.AddProject<Projects.RoutingService_API>("routing-api")
    .WithReference(routesDb)
    .WaitFor(routesDb);

var personnelApi = builder.AddProject<Projects.PersonnelService_API>("personnel-api")
    .WithReference(personnelDb)
    .WaitFor(personnelDb);

var aggregator = builder.AddProject<Projects.AggregatorService>("aggregator")
    .WithReference(technicalApi)
    .WithReference(routingApi)
    .WithReference(personnelApi)
    .WaitFor(technicalApi)
    .WaitFor(routingApi)
    .WaitFor(personnelApi);

var gateway = builder.AddProject<Projects.ApiGateway>("gateway")
    .WithReference(technicalApi)
    .WithReference(routingApi)
    .WithReference(personnelApi)
    .WithReference(aggregator)
    .WaitFor(aggregator);

builder.Build().Run();