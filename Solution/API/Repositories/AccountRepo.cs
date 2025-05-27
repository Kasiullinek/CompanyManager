using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.Dtos;
using static SharedLibrary.Dtos.ServiceResponse;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SharedLibrary.Interfaces;

namespace API.Repositories
{
    public class AccountRepo(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config) : IUserAccount
    {
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
    }
}
