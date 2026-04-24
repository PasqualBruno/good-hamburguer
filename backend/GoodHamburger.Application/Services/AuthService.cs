using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GoodHamburger.Application.DTOs;
using GoodHamburger.Application.Interfaces;
using GoodHamburger.Domain.Errors;
using GoodHamburger.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace GoodHamburger.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public LoginResponse Login(LoginRequest request)
    {
        var user = _userRepository.GetByEmail(request.Email);

        // Validação simples de senha (em prod usaríamos password hashing)
        if (user == null || user.PasswordHash != request.Password)
        {
            throw new DomainError(DomainErrorCodes.InvalidCredentials, "Email ou senha incorretos.");
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"] ?? "SecretKeyMuitoLongaESegura1234567890");
        
        var expiresAt = DateTime.UtcNow.AddDays(1);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, "Admin")
            }),
            Expires = expiresAt,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        
        return new LoginResponse
        {
            Token = tokenHandler.WriteToken(token),
            Name = user.Name,
            ExpiresAt = expiresAt
        };
    }
}
