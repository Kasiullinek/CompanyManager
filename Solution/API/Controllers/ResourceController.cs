using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Dtos;
using SharedLibrary.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceController(IResource resource) : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpPost("add-resource")]
        public async Task<IActionResult> AddResource(ResourceDto resourceDto)
        {
            var response = await resource.AddResource(resourceDto);
            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("get-all-resources")]
        public async Task<ActionResult<IEnumerable<ResourceDto>>> GetAllResources()
        {
            var response = await resource.GetAllResources();
            return Ok(response);
        }

        [Authorize(Roles = "User")]
        [HttpGet("get-user-resources")]
        public async Task<ActionResult<IEnumerable<ResourceDto>>> GetUserResources()
        {
            var response = await resource.GetUserResources();
            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("get-resource/Id={Id}")]
        public async Task<ActionResult<ResourceDto>> GetResource(string Id)
        {
            var response = await resource.GetResource(Id);
            return Ok(response);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPut("edit-resource")]
        public async Task<IActionResult> EditResource(ResourceDto resourceDto)
        {
            var result = await resource.EditResource(resourceDto);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete-resource/Id={Id}")]
        public async Task<IActionResult> DeleteResource(string Id)
        {
            var result = await resource.DeleteResource(Id);
            return Ok(result);
        }
        
    }
}
