using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = "Admin")]
        [HttpGet("get-all-users")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            var response = await userAccount.GetAllUsers();
            return Ok(response);
        }

        [Authorize(Roles = "User")]
        [HttpGet("get-user-info")]
        public async Task<ActionResult<UserDto>> GetUserInfo()
        {
            var response = await userAccount.GetUserInfo();
            return Ok(response);
        }

        [Authorize(Roles = "User")]
        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount()
        {
            var result = await userAccount.DeleteAccount();
            return Ok(result);
        }

        [Authorize(Roles = "User")]
        [HttpPut("edit-account")]
        public async Task<IActionResult> EditAccount(UserDto userDto)
        {
            var result = await userAccount.EditAccount(userDto);
            return Ok(result);
        }
    }
}
