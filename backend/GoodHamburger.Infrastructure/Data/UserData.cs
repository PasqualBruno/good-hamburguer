using GoodHamburger.Domain.Entities;

namespace GoodHamburger.Infrastructure.Data;

public static class UserData
{
    public static List<User> GetAll() =>
    [
        new User(
            Guid.Parse("d9b3b0b0-0b0b-0b0b-0b0b-d9b3b0b0b0b0"),
            "admin@goodhamburger.com",
            "admin123", // Em um projeto real, isso seria um hash (ex: BCrypt)
            "Administrador"
        )
    ];
}
