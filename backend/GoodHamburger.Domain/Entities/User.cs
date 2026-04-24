namespace GoodHamburger.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public User() { }

    public User(Guid id, string email, string passwordHash, string name)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        Name = name;
    }
}
