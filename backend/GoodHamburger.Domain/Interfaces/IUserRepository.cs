using GoodHamburger.Domain.Entities;

namespace GoodHamburger.Domain.Interfaces;

public interface IUserRepository
{
    User? GetByEmail(string email);
}
