using Moq;
using API.Controllers;
using SharedLibrary.Interfaces;
using SharedLibrary.Dtos;
using Microsoft.AspNetCore.Mvc;
using static SharedLibrary.Dtos.ServiceResponse;

namespace API.Tests.Controllers
{
    public class AccountControllerTests
    {
        private readonly Mock<IUserAccount> _userAccountMock;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            _userAccountMock = new Mock<IUserAccount>();
            _controller = new AccountController(_userAccountMock.Object);
        }

        [Fact]
        public async Task Register_ReturnsOkWithResponse()
        {
            var registerDto = new RegisterDto { Email = "test@test.com", Password = "123456" };
            var expectedResponse = new GeneralResponse(true, "Created");

            _userAccountMock.Setup(x => x.CreateAccount(registerDto)).ReturnsAsync(expectedResponse);

            var result = await _controller.Register(registerDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResponse, okResult.Value);
        }

        [Fact]
        public async Task Login_ReturnsOkWithToken()
        {
            var loginDto = new LoginDto { Email = "test@test.com", Password = "123456" };
            var loginResponse = new LoginResponse(true, "fake-jwt", "Login successful");

            _userAccountMock.Setup(x => x.LoginAccount(loginDto)).ReturnsAsync(loginResponse);

            var result = await _controller.Login(loginDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(loginResponse, okResult.Value);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsListOfUsers()
        {
            var users = new List<UserDto>
            {
                new UserDto { Id = "1", Email = "admin@test.com", UserName = "admin", Role = "Admin" }
            };

            _userAccountMock.Setup(x => x.GetAllUsers()).ReturnsAsync(users);

            var result = await _controller.GetAllUsers();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(users, okResult.Value);
        }

        [Fact]
        public async Task GetUserInfo_ReturnsUserDto()
        {
            var user = new UserDto { Id = "1", Email = "user@test.com", UserName = "user", Role = "User" };

            _userAccountMock.Setup(x => x.GetUserInfo()).ReturnsAsync(user);

            var result = await _controller.GetUserInfo();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(user, okResult.Value);
        }

        [Fact]
        public async Task DeleteAccount_ReturnsSuccessMessage()
        {
            var response = new GeneralResponse(true, "Account was deleted.");

            _userAccountMock.Setup(x => x.DeleteAccount()).ReturnsAsync(response);

            var result = await _controller.DeleteAccount();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(response, okResult.Value);
        }

        [Fact]
        public async Task EditAccount_ReturnsUpdatedToken()
        {
            var dto = new UserDto { Id = "1", UserName = "UpdatedName" };
            var response = new EditResponse(true, "updated-token", "Profile updated successfully.");

            _userAccountMock.Setup(x => x.EditAccount(dto)).ReturnsAsync(response);

            var result = await _controller.EditAccount(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(response, okResult.Value);
        }
    }
}
