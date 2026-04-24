using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Interfaces;
using GoodHamburger.Infrastructure.Context;

namespace GoodHamburger.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly GoodHamburgerDbContext _context;

    public UserRepository(GoodHamburgerDbContext context)
    {
        _context = context;
    }

    public User? GetByEmail(string email)
    {
        return _context.Users.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
    }
}
