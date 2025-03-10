using Microsoft.EntityFrameworkCore;
using MyFinance.Models;

namespace MyFinance.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
                if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                {
                    return null;
                }
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("Authentication failed", ex);
            }
        }

        public async Task<User> RegisterAsync(string username, string password, string currency)
        {
            try
            {
                if (await _context.Users.AnyAsync(u => u.Username == username))
                {
                    throw new Exception("Username already exists");
                }

                var user = new User
                {
                    Username = username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                    Currency = currency
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("Registration failed", ex);
            }
        }
    }
}
