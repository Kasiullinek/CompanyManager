using Moq;
using API.Controllers;
using SharedLibrary.Interfaces;
using SharedLibrary.Dtos;
using Microsoft.AspNetCore.Mvc;
using static SharedLibrary.Dtos.ServiceResponse;

namespace API.Tests.Controllers
{
    public class ResourceControllerTests
    {
        private readonly Mock<IResource> _resourceMock;
        private readonly ResourceController _controller;

        public ResourceControllerTests()
        {
            _resourceMock = new Mock<IResource>();
            _controller = new ResourceController(_resourceMock.Object);
        }

        [Fact]
        public async Task AddResource_ReturnsOk()
        {
            var dto = new ResourceDto { Name = "Test" };
            var response = new GeneralResponse(true, "Resource created");

            _resourceMock.Setup(r => r.AddResource(dto)).ReturnsAsync(response);

            var result = await _controller.AddResource(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(response, okResult.Value);
        }

        [Fact]
        public async Task GetAllResources_ReturnsList()
        {
            var list = new List<ResourceDto> { new ResourceDto { Name = "Test" } };

            _resourceMock.Setup(r => r.GetAllResources()).ReturnsAsync(list);

            var result = await _controller.GetAllResources();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(list, okResult.Value);
        }

        [Fact]
        public async Task GetUserResources_ReturnsUserList()
        {
            var list = new List<ResourceDto> { new ResourceDto { Name = "User Resource" } };

            _resourceMock.Setup(r => r.GetUserResources()).ReturnsAsync(list);

            var result = await _controller.GetUserResources();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(list, okResult.Value);
        }

        [Fact]
        public async Task GetResource_ReturnsResource()
        {
            var resource = new ResourceDto { Id = "1", Name = "R1" };

            _resourceMock.Setup(r => r.GetResource("1")).ReturnsAsync(resource);

            var result = await _controller.GetResource("1");

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(resource, okResult.Value);
        }

        [Fact]
        public async Task EditResource_ReturnsOk()
        {
            var dto = new ResourceDto { Id = "1", Name = "Edit" };
            var response = new GeneralResponse(true, "Updated");

            _resourceMock.Setup(r => r.EditResource(dto)).ReturnsAsync(response);

            var result = await _controller.EditResource(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(response, okResult.Value);
        }

        [Fact]
        public async Task DeleteResource_ReturnsOk()
        {
            var response = new GeneralResponse(true, "Deleted");

            _resourceMock.Setup(r => r.DeleteResource("1")).ReturnsAsync(response);

            var result = await _controller.DeleteResource("1");

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(response, okResult.Value);
        }
    }
}
