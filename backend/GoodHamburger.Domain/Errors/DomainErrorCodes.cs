namespace GoodHamburger.Domain.Errors;

/// <summary>
/// Constantes com os códigos de erro de domínio.
/// Usados pelo backend para lançar DomainError e pelo frontend para tratar erros.
/// </summary>
public static class DomainErrorCodes
{
    public const string DuplicateSandwich = "DUPLICATE_SANDWICH";
    public const string DuplicateSide = "DUPLICATE_SIDE";
    public const string DuplicateDrink = "DUPLICATE_DRINK";
    public const string EmptyOrder = "EMPTY_ORDER";
    public const string InvalidMenuItem = "INVALID_MENU_ITEM";
}
