using System.Net;
using System.Text.Json;
using GoodHamburger.Application.DTOs;
using GoodHamburger.Domain.Errors;

namespace GoodHamburger.API.Middlewares;

/// <summary>
/// Middleware global que intercepta DomainError e retorna HTTP 400 com { code, message } padronizado.
/// </summary>
public class DomainErrorMiddleware
{
    private readonly RequestDelegate _next;

    public DomainErrorMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DomainError ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";

            var errorResponse = new ErrorResponse(ex.Code, ex.Message);
            var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
}
