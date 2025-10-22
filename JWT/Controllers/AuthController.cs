using JWT.Data.Context;
using JWT.Dtos;
using JWT.Models;
using JWT.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAuthService _authService;
        private readonly AppDbContext _dbContext;

        public AuthController(UserManager<ApplicationUser> userManager
                                , SignInManager<ApplicationUser> signInManager
                                , RoleManager<IdentityRole> roleManager
                                , IAuthService authService
                                , AppDbContext dbContext
                                )
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._roleManager = roleManager;
            this._authService = authService;
            this._dbContext = dbContext;
        }



        [HttpGet("getUsers")]
        public IActionResult GetUsers()
        {
            var users = _userManager.Users.ToList();
            var mappedUsers = users.Select(u => new { Username = u.UserName, Email = u.Email, Phone = u.PhoneNumber });
            return Ok(mappedUsers);
        }



        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {

            var existingUser = await _userManager.FindByNameAsync(model.UserName);
            if (existingUser is not null)
            {

                return Ok("Username is already taken.");
            }

            var existingEmail = await _userManager.FindByEmailAsync(model.Email);
            if (existingEmail is not null)
            {
                return Ok("Email is already registered.");
            }

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {

                return Ok("User Register Successfully.");

            }
            else
            {

                return Ok("User registration failed.");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            var existingUser = await _userManager.FindByNameAsync(model.UserName);
            if (existingUser == null)
            {
                return Ok("Invalid username.");
            }
            var result = await _signInManager.CheckPasswordSignInAsync(existingUser, model.Password, false);
            if (!result.Succeeded)
            {
                return Ok("Incorrect password.");
            }
            var accessToken = await _authService.CreateTokenAsync(existingUser, _userManager);
            var refreshToken = await _authService.CreateRefreshTokenAsync(existingUser);
            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }



        [Authorize(Roles = "Admin")]
        [HttpPost("addRole")]
        public async Task<IActionResult> AddRole(string roleName)
        {
            if (await _roleManager.RoleExistsAsync(roleName))
            {
                return Ok($"The role '{roleName}' already exists in the system.");
            }
            else
            {
                var role = new IdentityRole { Name = roleName };
                var result = await _roleManager.CreateAsync(role);
                if (!result.Succeeded)
                {
                    return Ok("Failed to create role.");
                }
                else
                {
                    return Ok("Role created successfully.");
                }
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("assignRole")]
        public async Task<IActionResult> AssignRoleToSpecificUser(string username, string roleName)
        {
            var existingUser = await _userManager.FindByNameAsync(username);
            if (existingUser == null)
            {
                return Ok("User not found.");
            }
            var isRoleExist = await _roleManager.RoleExistsAsync(roleName);
            if (!isRoleExist)
            {
                return Ok($"The role '{roleName}' does not exist in the system.");
            }

            var isUserAsignToThisRole = await _userManager.IsInRoleAsync(existingUser, roleName);
            if (isUserAsignToThisRole)
            {
                return Ok($"The user '{username}' is already assigned to the role '{roleName}'.");
            }


            var result = await _userManager.AddToRoleAsync(existingUser, roleName);
            if (!result.Succeeded)
            {
                return Ok($"An unexpected error occurred while assigning the role '{roleName}' to user '{existingUser.UserName}'. Please try again later.");
            }
            else
            {
                return Ok($"User '{username}' is now assigned to the role '{roleName}'.");
            }
        }



        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshToken(RefreshTokenDto model)
        {
            if (string.IsNullOrEmpty(model.RefreshToken))
            {
                return BadRequest(new { Message = "Refresh token is required" });
            }

            var user = await _dbContext.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.RefreshTokens.Any(rt =>
                    rt.Value == model.RefreshToken && (rt.ExpiresOn > DateTime.UtcNow)
                ));
            if (user == null)
                return Unauthorized(new { Message = "Invalid or expired refresh token" });

            var refreshTokenExist = user.RefreshTokens.FirstOrDefault(rt => rt.Value == model.RefreshToken);
            if (refreshTokenExist is null)
            {
                return Ok(new { Message = "Invalid Refresh Token" });
            }
            if (!refreshTokenExist.IsExpired)
            {
                var accessToken = await _authService.CreateTokenAsync(user, _userManager);
                var refreshToken = await _authService.CreateRefreshTokenAsync(user);
                return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
            }
            else
            {
                return BadRequest("Your session has expired. Please log in again to continue.");
            }

        }

    }
}
