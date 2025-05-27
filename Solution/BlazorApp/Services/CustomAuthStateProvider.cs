using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using SharedLibrary.Dtos.GenericsModel;
using System.Security.Claims;

namespace BlazorApp.Services
{
    public class CustomAuthStateProvider(ILocalStorageService localStorageService) : AuthenticationStateProvider
    {
        private ClaimsPrincipal anonymous = new(new ClaimsIdentity());
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var strToken = await localStorageService.GetItemAsStringAsync("token");

                if (string.IsNullOrWhiteSpace(strToken))
                {
                    return await Task.FromResult(new AuthenticationState(anonymous));
                }

                var claims = Generics.GetClaimsFromToken(strToken);

                var claimsPrincipal = Generics.SetClaimPrincipal(claims);
                return await Task.FromResult(new AuthenticationState(claimsPrincipal));

            }
            catch
            {
                return await Task.FromResult(new AuthenticationState(anonymous));
            }
        }

        public async Task UpdateAuthenticationState(string? token)
        {
            ClaimsPrincipal claimsPrincipal = new();

            if (!string.IsNullOrWhiteSpace(token))
            {
                var userSession = Generics.GetClaimsFromToken(token);
                claimsPrincipal = Generics.SetClaimPrincipal(userSession);
                await localStorageService.SetItemAsStringAsync("token", token);
            }
            else
            {
                claimsPrincipal = anonymous;
                await localStorageService.RemoveItemAsync("token");
            }
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }
    }
}
