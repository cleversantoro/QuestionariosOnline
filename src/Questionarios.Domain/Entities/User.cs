using System.Security.Cryptography;
using System.Text;

namespace Questionarios.Domain.Entities;

public class User
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public byte[] PasswordHash { get; private set; } = Array.Empty<byte>();

    private User() { }

    public User(string name, string email, string password)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password is required.", nameof(password));

        Name = name;
        Email = email;
        PasswordHash = HashPassword(password);
    }

    public void Update(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));

        Name = name;
        Email = email;
    }

    public void SetPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password is required.", nameof(password));

        PasswordHash = HashPassword(password);
    }

    public bool ValidatePassword(string password)
    {
        var candidate = HashPassword(password);
        return PasswordHash.SequenceEqual(candidate);
    }

    private static byte[] HashPassword(string password)
    {
        var bytes = Encoding.UTF8.GetBytes(password);
        return SHA256.HashData(bytes);
    }
}
