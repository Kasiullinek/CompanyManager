using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Dtos;
using SharedLibrary.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(IUserAccount userAccount) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var response = await userAccount.CreateAccount(registerDto);
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var response = await userAccount.LoginAccount(loginDto);
            return Ok(response);
        }
    }
}
