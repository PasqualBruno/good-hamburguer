using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Interfaces;
using GoodHamburger.Infrastructure.Data;

namespace GoodHamburger.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly List<User> _users = UserData.GetAll();

    public User? GetByEmail(string email)
    {
        return _users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }
}
