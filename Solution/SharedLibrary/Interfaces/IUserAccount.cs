using SharedLibrary.Dtos;
using static SharedLibrary.Dtos.ServiceResponse;

namespace SharedLibrary.Interfaces
{
    public interface IUserAccount
    {
        Task<GeneralResponse> CreateAccount(RegisterDto registerDto);
        Task<LoginResponse> LoginAccount(LoginDto loginDto);
    }
}
