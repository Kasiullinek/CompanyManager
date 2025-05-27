using SharedLibrary.Dtos.GenericsModel;
using SharedLibrary.Dtos;
using SharedLibrary.Interfaces;
using static SharedLibrary.Dtos.ServiceResponse;
using SharedLibrary;
using Blazored.LocalStorage;
using System.Net.Http.Json;

namespace BlazorApp.Services
{
    public class AccountService(HttpClient httpClient, ILocalStorageService localStorage) : IUserAccount, IWeather
    {
        private const string BaseUrl = "api/Account";

        public async Task<GeneralResponse> CreateAccount(RegisterDto registerDto)
        {
            var response = await httpClient.PostAsync($"{BaseUrl}/register", Generics.GenerateStringContent(Generics.SerializeObj(registerDto)));

            if (!response.IsSuccessStatusCode)
            {
                return new GeneralResponse(false, "Error occured. Try again...");
            }

            var apiResponse = await response.Content.ReadAsStringAsync();
            return Generics.DeserializeJsonString<GeneralResponse>(apiResponse);
        }

        public async Task<LoginResponse> LoginAccount(LoginDto loginDto)
        {
            var response = await httpClient.PostAsync($"{BaseUrl}/login", Generics.GenerateStringContent(Generics.SerializeObj(loginDto)));

            if (!response.IsSuccessStatusCode)
            {
                return new LoginResponse(false, null!, "Error occured. Try again...");
            }

            var apiResponse = await response.Content.ReadAsStringAsync();
            var result = Generics.DeserializeJsonString<LoginResponse>(apiResponse);
            //httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.Token);
            // 📝 Zapisz token
            await localStorage.SetItemAsync("token", result.Token);

            return result;

        }

        private async Task SetAuthorizationHeaderAsync()
        {
            var token = await localStorage.GetItemAsync<string>("token");
            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<WeatherForecast[]> GetWeatherForecast()
        {
            await SetAuthorizationHeaderAsync();

            var response = await httpClient.GetAsync("api/weatherforecast/admin");

            if (!response.IsSuccessStatusCode)
            {
                return null!;
            }

            var result = await response.Content.ReadAsStringAsync();
            return [.. Generics.DeserializeJsonStringList<WeatherForecast>(result)];
        }

        public async Task<IEnumerable<UserDto>> GetAllUsers()
        {
            await SetAuthorizationHeaderAsync();

            var response = await httpClient.GetAsync($"{BaseUrl}/get-all-users");

            if (!response.IsSuccessStatusCode)
            {
                return Enumerable.Empty<UserDto>();
            }

            var result = await response.Content.ReadAsStringAsync();
            return Generics.DeserializeJsonStringList<UserDto>(result);
        }

        public async Task<UserDto> GetUserInfo()
        {
            await SetAuthorizationHeaderAsync();

            var response = await httpClient.GetAsync($"{BaseUrl}/get-user-info");

            if (!response.IsSuccessStatusCode)
            {
                return null!;
            }

            var result = await response.Content.ReadAsStringAsync();
            return Generics.DeserializeJsonString<UserDto>(result);
        }

        public async Task<GeneralResponse> DeleteAccount()
        {
            await SetAuthorizationHeaderAsync();
            var response = await httpClient.DeleteAsync($"{BaseUrl}/delete-account");

            if (!response.IsSuccessStatusCode)
            {
                return new GeneralResponse(false, "Error deleting account.");
            }    

            var result = await response.Content.ReadAsStringAsync();
            return Generics.DeserializeJsonString<GeneralResponse>(result);
        }

        public async Task<EditResponse> EditAccount(UserDto userDto)
        {
            await SetAuthorizationHeaderAsync();
            var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/edit-account", userDto);

            if (!response.IsSuccessStatusCode)
            {
                return new EditResponse(false, "", "Failed to Update the User Name.");
            }

            var resultStr = await response.Content.ReadAsStringAsync();
            var result = Generics.DeserializeJsonString<EditResponse>(resultStr);

            if (result.Flag && !string.IsNullOrWhiteSpace(result.Token))
            {
                await localStorage.SetItemAsync("token", result.Token);
            }

            return result;
        }



    }
}
