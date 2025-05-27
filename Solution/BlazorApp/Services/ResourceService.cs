using SharedLibrary.Dtos.GenericsModel;
using SharedLibrary.Dtos;
using static SharedLibrary.Dtos.ServiceResponse;
using Blazored.LocalStorage;
using SharedLibrary.Interfaces;
using System.Net.Http.Json;

namespace BlazorApp.Services
{
    public class ResourceService(HttpClient httpClient, ILocalStorageService localStorage) : IResource
    {
        private const string BaseUrl = "api/Resource";

        private async Task SetAuthorizationHeaderAsync()
        {
            var token = await localStorage.GetItemAsync<string>("token");
            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }
        public async Task<GeneralResponse> AddResource(ResourceDto resourceDto)
        {
            await SetAuthorizationHeaderAsync();
            var response = await httpClient.PostAsJsonAsync($"{BaseUrl}/add-resource", resourceDto);

            if (!response.IsSuccessStatusCode)
            {
                return new GeneralResponse(false, "Error Adding Resource.");
            }

            var resultStr = await response.Content.ReadAsStringAsync();
            return Generics.DeserializeJsonString<GeneralResponse>(resultStr);
        }


        public async Task<IEnumerable<ResourceDto>> GetAllResources()
        {
            await SetAuthorizationHeaderAsync();
            var response = await httpClient.GetAsync($"{BaseUrl}/get-all-resources");

            if (!response.IsSuccessStatusCode)
            {
                return Enumerable.Empty<ResourceDto>();
            }

            var result = await response.Content.ReadAsStringAsync();
            return Generics.DeserializeJsonStringList<ResourceDto>(result);
        }

        public async Task<IEnumerable<ResourceDto>> GetUserResources()
        {
            await SetAuthorizationHeaderAsync();
            var response = await httpClient.GetAsync($"{BaseUrl}/get-user-resources");

            if (!response.IsSuccessStatusCode)
            {
                return Enumerable.Empty<ResourceDto>();
            }

            var result = await response.Content.ReadAsStringAsync();
            return Generics.DeserializeJsonStringList<ResourceDto>(result);
        }


        public async Task<ResourceDto> GetResource(string Id)
        {
            await SetAuthorizationHeaderAsync();
            var response = await httpClient.GetAsync($"{BaseUrl}/get-resource/Id={Id}");

            if (!response.IsSuccessStatusCode)
            {
                return null!;
            }

            var result = await response.Content.ReadAsStringAsync();
            return Generics.DeserializeJsonString<ResourceDto>(result);
        }

        public async Task<GeneralResponse> EditResource(ResourceDto resourceDto)
        {
            await SetAuthorizationHeaderAsync();
            var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/edit-resource", resourceDto);

            if (!response.IsSuccessStatusCode)
            {
                return new GeneralResponse(false, "Failed to update resource.");
            }

            var result = await response.Content.ReadAsStringAsync();
            return Generics.DeserializeJsonString<GeneralResponse>(result);
        }

        public async Task<GeneralResponse> DeleteResource(string Id)
        {
            await SetAuthorizationHeaderAsync();
            var response = await httpClient.DeleteAsync($"{BaseUrl}/delete-resource/Id={Id}");

            if (!response.IsSuccessStatusCode)
            {
                return new GeneralResponse(false, "Failed to delete resource.");
            }

            var result = await response.Content.ReadAsStringAsync();
            return Generics.DeserializeJsonString<GeneralResponse>(result);
        }

    }
}
