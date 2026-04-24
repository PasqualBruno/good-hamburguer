using GoodHamburger.Application.DTOs;

namespace GoodHamburger.Application.Interfaces;

public interface IAuthService
{
    LoginResponse Login(LoginRequest request);
}
