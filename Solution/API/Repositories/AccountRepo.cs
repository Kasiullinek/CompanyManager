using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.Dtos;
using static SharedLibrary.Dtos.ServiceResponse;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SharedLibrary.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class AccountRepo(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config, IHttpContextAccessor httpContextAccessor) : IUserAccount
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<GeneralResponse> CreateAccount(RegisterDto registerDto)
        {
            if (registerDto is null)
            {
                return new GeneralResponse(false, "Model is empty");
            }

            var newUser = new IdentityUser()
            {
                Email = registerDto.Email,
                PasswordHash = registerDto.Password,
                UserName = registerDto.Email,
            };

            var user = await userManager.FindByEmailAsync(newUser.Email);

            if (user is not null)
            {
                return new GeneralResponse(false, "This User is already registered");
            }

            var createUser = await userManager.CreateAsync(newUser!, registerDto.Password);

            if (!createUser.Succeeded)
            {
                return new GeneralResponse(false, "Error occured... Please try again");
            }

            var checkAdmin = await roleManager.FindByNameAsync("Admin");

            if (checkAdmin is null)
            {
                await roleManager.CreateAsync(new IdentityRole() { Name = "Admin" });
                await userManager.AddToRoleAsync(newUser, "Admin");
                return new GeneralResponse(true, "Account was Created!");
            }
            else
            {
                var checkUser = await roleManager.FindByNameAsync("User");
                if (checkUser is null)
                {
                    await roleManager.CreateAsync(new IdentityRole() { Name = "User" });
                }

                await userManager.AddToRoleAsync(newUser, "User");
                return new GeneralResponse(true, "Account was Created!");
            }
        }

        public async Task<LoginResponse> LoginAccount(LoginDto loginDto)
        {
            if (loginDto == null)
            {
                return new LoginResponse(false, null!, "Login Container is Empty");
            }
            var getUser = await userManager.FindByEmailAsync(loginDto.Email);

            if (getUser is null)
            {
                return new LoginResponse(false, null!, "User was not found");
            }

            bool checkUserPassword = await userManager.CheckPasswordAsync(getUser, loginDto.Password);

            if (!checkUserPassword)
            {
                return new LoginResponse(false, null!, "Email or Password is Invalid");
            }

            var getUserRole = await userManager.GetRolesAsync(getUser);
            var userSession = new UserSession(getUser.Id, getUser.UserName, getUser.Email, getUserRole.First());
            string token = GenerateToken(userSession);
            return new LoginResponse(true, token!, "Login is Completed!");
        }

        private string GenerateToken(UserSession userSession)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userSession.Id!),
                new Claim(ClaimTypes.Name, userSession.Name!),
                new Claim(ClaimTypes.Email, userSession.Email!),
                new Claim(ClaimTypes.Role,userSession.Role!)
            };
            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: userClaims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<IEnumerable<UserDto>> GetAllUsers()
        {
            var users = await userManager.Users.ToListAsync();
            var result = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                result.Add(new UserDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    UserName = user.UserName!,
                    Role = roles.FirstOrDefault() ?? "No Role"
                });
            }

            return result;
        }

        public async Task<UserDto> GetUserInfo()
        {
            var id = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await userManager.FindByIdAsync(id);
            if (user is null)
            {
                return null!;
            }

            var roles = await userManager.GetRolesAsync(user);

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                UserName = user.UserName!,
                Role = roles.FirstOrDefault() ?? "No Role"
            };
        }

        public async Task<GeneralResponse> DeleteAccount()
        {
            var id = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await userManager.FindByIdAsync(id);
            if (user is null)
            {
                return new GeneralResponse(false, "User not found.");
            }
                
            var result = await userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                return new GeneralResponse(false, "Error deleting account.");
            }

            return new GeneralResponse(true, "Account was deleted.");
        }

        public async Task<EditResponse> EditAccount(UserDto userDto)
        {
            var id = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userManager.FindByIdAsync(id);

            if (user is null)
            {
                return new EditResponse(false, "", "User not found.");
            }

            user.UserName = userDto.UserName;

            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return new EditResponse(false, "", "Failed to update user.");
            }

            var roles = await userManager.GetRolesAsync(user);
            var userSession = new UserSession(user.Id, user.UserName, user.Email, roles.FirstOrDefault());
            string newToken = GenerateToken(userSession);

            return new EditResponse(true, newToken, "Profile updated successfully.");
        }

    }
}
