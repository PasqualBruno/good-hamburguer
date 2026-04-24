namespace GoodHamburger.Application.DTOs;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
