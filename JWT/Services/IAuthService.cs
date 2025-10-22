using JWT.Models;
using Microsoft.AspNetCore.Identity;

namespace JWT.Services
{
    public interface IAuthService
    {
        public Task<string> CreateTokenAsync(ApplicationUser user, UserManager<ApplicationUser> _userManager);
        public Task<string> CreateRefreshTokenAsync(ApplicationUser user);
    }
}
