namespace GoodHamburger.Domain.Errors;

/// <summary>
/// Erro de domínio com código e mensagem padronizados para o frontend consumir.
/// Interceptado pelo middleware da API e retornado como HTTP 400 com { code, message }.
/// </summary>
public class DomainError : Exception
{
    public string Code { get; }

    public DomainError(string code, string message) : base(message)
    {
        Code = code;
    }
}
