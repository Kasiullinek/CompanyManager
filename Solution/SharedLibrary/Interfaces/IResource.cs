using SharedLibrary.Dtos;
using static SharedLibrary.Dtos.ServiceResponse;

namespace SharedLibrary.Interfaces
{
    public interface IResource
    {
        Task<GeneralResponse> AddResource(ResourceDto resourceDto);
        Task<IEnumerable<ResourceDto>> GetAllResources();
        Task<IEnumerable<ResourceDto>> GetUserResources();
        Task<ResourceDto> GetResource(string Id);
        Task<GeneralResponse> EditResource(ResourceDto resourceDto);
        Task<GeneralResponse> DeleteResource(string Id);
    }
}
