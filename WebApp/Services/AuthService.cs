using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System;
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

        public async Task<AuthenticationResult> AuthenticateAdmin()
        {
            var authToken = await _localStorage.GetItemAsync<string>("authToken");

            AuthenticationResult authenticationResult = new AuthenticationResult();

            if (string.IsNullOrEmpty(authToken))
            {
                authenticationResult.Successful = false;
                authenticationResult.Error = "No authentication token found.";
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            var response = await _httpClient.GetAsync("gateway/user/authenticate");
            authenticationResult.Successful = response.IsSuccessStatusCode;

            if (!authenticationResult.Successful)
            {
                authenticationResult.Error = await response.Content.ReadAsStringAsync();
            }

            var authenticatedUser = await response.Content.ReadFromJsonAsync<bool>();
            authenticationResult.IsAdmin = authenticatedUser;

            return authenticationResult;
        }

        public async Task<string?> GetUserId()
        {
            var userState = await ((ApiAuthenticationStateProvider)_authenticationStateProvider).GetAuthenticationStateAsync();
            var userId = userState.User.FindFirst(c => c.Type.Equals("userId"))?.Value;

            return userId;
        }

        public async Task<UserResult> GetUser(string userId)
        {
            var result = await _httpClient.GetAsync($"gateway/user/{userId}");
            UserResult userResult = new UserResult();

            userResult.Successful = result.IsSuccessStatusCode;

            if (!userResult.Successful)
            {
                userResult.Error = await result.Content.ReadAsStringAsync();
                return userResult;
            }

            userResult.User = await result.Content.ReadFromJsonAsync<User>();

            return userResult;
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}
