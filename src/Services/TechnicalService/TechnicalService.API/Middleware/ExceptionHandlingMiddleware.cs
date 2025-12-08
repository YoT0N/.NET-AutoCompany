using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TechnicalService.Domain.Exceptions;

namespace TechnicalService.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Необроблений виняток: {Message}", exception.Message);

        var problemDetails = exception switch
        {
            NotFoundException notFound => new ProblemDetails
            {
                Status = (int)HttpStatusCode.NotFound,
                Title = "Ресурс не знайдено",
                Detail = notFound.Message,
                Instance = context.Request.Path
            },
            BusinessConflictException conflict => new ProblemDetails
            {
                Status = (int)HttpStatusCode.Conflict,
                Title = "Бізнес-конфлікт",
                Detail = conflict.Message,
                Instance = context.Request.Path
            },
            ValidationException validation => new ValidationProblemDetails(validation.Errors)
            {
                Status = (int)HttpStatusCode.BadRequest,
                Title = "Помилка валідації",
                Instance = context.Request.Path
            },
            _ => new ProblemDetails
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Title = "Внутрішня помилка сервера",
                Detail = "Виникла неочікувана помилка",
                Instance = context.Request.Path
            }
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;

        var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}