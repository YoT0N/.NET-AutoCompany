using PersonnelService.Application.Interfaces;
using PersonnelService.Application.Services;
using PersonnelService.Core.Interfaces;
using PersonnelService.Infrastructure.Data;
using PersonnelService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure MongoDB
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.AddSingleton<MongoDbContext>();

// Register Repositories
builder.Services.AddScoped<IPersonnelRepository, PersonnelRepository>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IExaminationRepository, ExaminationRepository>();
builder.Services.AddScoped<IWorkShiftRepository, WorkShiftRepository>();

// Register Services
builder.Services.AddScoped<IPersonnelService, PersonnelService.Application.Services.PersonnelService>();
builder.Services.AddScoped<IWorkShiftService, PersonnelService.Application.Services.WorkShiftService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IExaminationService, ExaminationService>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();