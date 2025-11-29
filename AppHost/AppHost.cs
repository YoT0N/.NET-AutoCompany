using Aspire.Hosting;
using Aspire.Hosting.MySql;
using Aspire.Hosting.MongoDB;

var builder = DistributedApplication.CreateBuilder(args);

var mysqlTechnicalServer = builder.AddMySql("mysql-technical")
    .WithDataVolume();

var transportServiceDb = mysqlTechnicalServer.AddDatabase("transportservicedb");

var mysqlRoutingServer = builder.AddMySql("mysql-routing")
    .WithDataVolume();

var routesDb = mysqlRoutingServer.AddDatabase("routesdb");

var mongoPersonnelServer = builder.AddMongoDB("mongo-personnel")
    .WithDataVolume();

var personnelDb = mongoPersonnelServer.AddDatabase("personneldb");


var technicalApi = builder.AddProject<Projects.TechnicalService_Api>("technicalservice-api")
    .WithReference(transportServiceDb);

var routingApi = builder.AddProject<Projects.RoutingService_API>("routing-api")
    .WithReference(routesDb);

var personnelApi = builder.AddProject<Projects.PersonnelService_API>("personnel-api")
    .WithReference(personnelDb);

builder.Build().Run();