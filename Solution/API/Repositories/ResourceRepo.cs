using SharedLibrary.Dtos;
using SharedLibrary.Interfaces;
using API.Data;
using Microsoft.EntityFrameworkCore;
using static SharedLibrary.Dtos.ServiceResponse;
using System.Security.Claims;

namespace API.Repositories
{
    public class ResourceRepo(AppDbContext context, IHttpContextAccessor httpContextAccessor) : IResource
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<GeneralResponse> AddResource(ResourceDto dto)
        {
            var currentEmail = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);
            var isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole("Admin") ?? false;

            var assignedUser = await context.Users.FirstOrDefaultAsync(u => u.Email == dto.AssignedToEmail);
            if (assignedUser is null)
            {
                return new GeneralResponse(false, "Assigned user does not exist!");
            }

            if (isAdmin && dto.AssignedToEmail == currentEmail)
            {
                return new GeneralResponse(false, "You cannot assign resource to yourself!");
            }

            var model = new Resource
            {
                Id = Guid.NewGuid().ToString(),
                Name = dto.Name,
                Description = dto.Description,
                AssignedToEmail = dto.AssignedToEmail,
                IsCompleted = dto.IsCompleted
            };

            context.Resources.Add(model);
            await context.SaveChangesAsync();
            return new GeneralResponse(true, "Resource was created!");
        }

        public async Task<IEnumerable<ResourceDto>> GetAllResources()
        {
            return await context.Resources
                .Select(r => new ResourceDto
                {
                    Name = r.Name,
                    Description = r.Description,
                    AssignedToEmail = r.AssignedToEmail,
                    IsCompleted = r.IsCompleted,
                    Id = r.Id
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ResourceDto>> GetUserResources()
        {
            var currentEmail = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);

            return await context.Resources
                .Where(r => r.AssignedToEmail == currentEmail)
                .Select(r => new ResourceDto
                {
                    Name = r.Name,
                    Description = r.Description,
                    AssignedToEmail = r.AssignedToEmail,
                    IsCompleted = r.IsCompleted,
                    Id = r.Id
                })
                .ToListAsync();
        }


        public async Task<ResourceDto> GetResource(string Id)
        {
            var resource = await context.Resources.FirstOrDefaultAsync(r => r.Id == Id);
            if (resource == null)
            {
                return null!;
            }

            return new ResourceDto
            {
                Name = resource.Name,
                Description = resource.Description,
                AssignedToEmail = resource.AssignedToEmail,
                IsCompleted = resource.IsCompleted,
                Id = resource.Id
            };
        }

        public async Task<GeneralResponse> EditResource(ResourceDto dto)
        {
            var resource = await context.Resources.FirstOrDefaultAsync(r => r.Id == dto.Id);
            if (resource == null)
            {
                return new GeneralResponse(false, "Resource not found!");
            }

            var assignedUser = await context.Users.FirstOrDefaultAsync(u => u.Email == dto.AssignedToEmail);
            if (assignedUser is null)
            {
                return new GeneralResponse(false, "Assigned user does not exist!");
            }

            var currentEmail = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);
            var isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole("Admin") ?? false;
            if (isAdmin && dto.AssignedToEmail == currentEmail)
            {
                return new GeneralResponse(false, "You cannot assign a resource to an Admin!");
            }

            resource.Name = dto.Name;
            resource.Description = dto.Description;
            resource.AssignedToEmail = dto.AssignedToEmail;
            resource.IsCompleted = dto.IsCompleted;

            await context.SaveChangesAsync();
            return new GeneralResponse(true, "Resource was updated!");
        }

        public async Task<GeneralResponse> DeleteResource(string Id)
        {
            var resource = await context.Resources.FirstOrDefaultAsync(r => r.Id == Id);
            if (resource == null)
            {
                return new GeneralResponse(false, "Resource not found!");
            }

            context.Resources.Remove(resource);
            await context.SaveChangesAsync();
            return new GeneralResponse(true, "Resource deleted!");
        }
      
    }
}
