using Microsoft.EntityFrameworkCore;
using FintcsApi.Data;
using FintcsApi.Models;
using System.Text.Json;

namespace FintcsApi.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            var users = await _context.Users.ToListAsync();
            return users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<User> CreateUserAsync(string username, string password, string email, string phone, string[] roles)
        {
            var validRoles = new[] { "user", "admin" };
            var validatedRoles = roles?.Where(role => validRoles.Contains(role.ToLower())).ToArray() ?? new[] { "user" };

            if (!validatedRoles.Any())
            {
                validatedRoles = new[] { "user" };
            }

            var details = new UserDetails
            {
                email = email,
                phone = phone ?? string.Empty,
                role = "user"
            };

            var user = new User
            {
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Details = JsonSerializer.Serialize(details)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public Task<bool> ValidatePasswordAsync(User user, string password)
        {
            return Task.FromResult(BCrypt.Net.BCrypt.Verify(password, user.PasswordHash));
        }

        public async Task<User> UpdateUserRolesAsync(int userId, string[] roles)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            var validRoles = new[] { "user", "admin" };
            var validatedRoles = roles?.Where(role => validRoles.Contains(role.ToLower())).ToArray() ?? new[] { "user" };

            if (!validatedRoles.Any())
            {
                validatedRoles = new[] { "user" };
            }

            var currentDetails = JsonSerializer.Deserialize<UserDetails>(user.Details) ?? new UserDetails();
            currentDetails.role = validatedRoles.FirstOrDefault() ?? "user";
            user.Details = JsonSerializer.Serialize(currentDetails);

            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<List<User>> GetUsersByRoleAsync(string role)
        {
            var users = await _context.Users.ToListAsync();
            return users.Where(u => u.Role.Equals(role, StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }
}
