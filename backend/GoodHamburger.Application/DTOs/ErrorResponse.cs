namespace GoodHamburger.Application.DTOs;

/// <summary>
/// Resposta padronizada de erro para o frontend.
/// Retornada quando um DomainError é lançado (HTTP 400).
/// </summary>
public class ErrorResponse
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public ErrorResponse() { }

    public ErrorResponse(string code, string message)
    {
        Code = code;
        Message = message;
    }
}
