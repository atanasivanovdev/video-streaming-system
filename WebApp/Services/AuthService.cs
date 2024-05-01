using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using WebApp.Models;

namespace WebApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorage;

        public AuthService(HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _authenticationStateProvider = authenticationStateProvider;
            _localStorage = localStorage;
        }

        public async Task<RegisterResult> Register(RegisterModel registerModel)
        {
            var result = await _httpClient.PostAsJsonAsync("gateway/user/register", registerModel);

            RegisterResult registerResult = new RegisterResult();

			registerResult.Successful = result.IsSuccessStatusCode;

			if (!registerResult.Successful)
			{
                var error = await result.Content.ReadAsStringAsync();
				registerResult.Errors = new List<string>() { error };
				return registerResult;
			}


            return registerResult;
        }

        public async Task<LoginResult> Login(LoginModel loginModel)
        {
            var result = await _httpClient.PostAsJsonAsync("gateway/user/login", loginModel);

            LoginResult loginResult = new LoginResult();

            loginResult.Successful = result.IsSuccessStatusCode;

            if (!loginResult.Successful)
            {
                loginResult.Error = await result.Content.ReadAsStringAsync();
				return loginResult;
            }

            loginResult.Token = await result.Content.ReadAsStringAsync();

            await _localStorage.SetItemAsync("authToken", loginResult.Token);

            ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(loginModel.Email);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.Token);

            return loginResult;
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}
