using CleanArch.Application.Exceptions;
using CleanArch.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace CleanArch.API.Middleware;

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
        catch (NotFoundException ex)
        {
            await Write(context, StatusCodes.Status404NotFound, "Ressource introuvable", ex.Message);
        }
        catch (DomainException ex)
        {
            await Write(context, StatusCodes.Status400BadRequest, "Règle métier non respectée", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur inattendue");
            await Write(context, StatusCodes.Status500InternalServerError, "Erreur interne", "Une erreur interne est survenue.");
        }
    }

    private static Task Write(HttpContext context, int status, string title, string detail)
    {
        context.Response.StatusCode = status;
        return context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail
        });
    }
}