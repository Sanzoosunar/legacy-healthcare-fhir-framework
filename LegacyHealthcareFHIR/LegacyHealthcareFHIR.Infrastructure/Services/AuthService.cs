using LegacyHealthcareFHIR.Core.Models;
using LegacyHealthcareFHIR.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LegacyHealthcareFHIR.Infrastructure.Services;

public class AuthService
{
    private readonly AppDbContext _dbContext;
    private readonly PasswordHasher<AppUser> _passwordHasher = new();

    public AuthService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AppUser> Login(string username, string password)
    {
        var user = await _dbContext.Users
            .Include(u => u.Hospital)
            .SingleOrDefaultAsync(u => u.Username == username.Trim()) ?? throw new Exception("Username does not exist.");

        var result = _passwordHasher.VerifyHashedPassword(user,user.PasswordHash,password);

        if (result == PasswordVerificationResult.Failed)
        {
            throw new Exception("Password does not match.");
        }

        if (result == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = _passwordHasher.HashPassword(
                user,
                password);

            await _dbContext.SaveChangesAsync();
        }

        return user;
    }
}
