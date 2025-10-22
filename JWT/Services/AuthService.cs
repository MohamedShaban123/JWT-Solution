using JWT.Data.Context;
using JWT.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWT.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _dbContext;

        public AuthService(IConfiguration configuration, AppDbContext dbContext)
        {
            _configuration = configuration;
            this._dbContext = dbContext;
        }



        public async Task<string> CreateTokenAsync(ApplicationUser user, UserManager<ApplicationUser> _userManager)
        {

            // private claims
            var privateClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim(ClaimTypes.GivenName,user.UserName),
                new Claim(ClaimTypes.Email,user.Email),
            };
            var userRols = await _userManager.GetRolesAsync(user);
            foreach (var role in userRols)
            {
                privateClaims.Add(new Claim(ClaimTypes.Role, role));
            }
            // Key
            var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));

            //
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JWT:AccessTokenExpiredInMinutes"])),
                claims: privateClaims,
                signingCredentials: new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256Signature)

                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public async Task<string> CreateRefreshTokenAsync(ApplicationUser user)
        {
            var randomString = JWT.Helper.Utility.GenerateRandomString();
            var refreshtoken = new RefreshToken
            {
                ApplicationUserId = user.Id,
                ExpiresOn = DateTime.UtcNow.AddDays(double.Parse(_configuration["JWT:RefreshTokenExpiredInDays"])),
                Value = randomString,
            };
            try
            {
                user.RefreshTokens.Add(refreshtoken);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }

            return randomString;
        }


    }
}
