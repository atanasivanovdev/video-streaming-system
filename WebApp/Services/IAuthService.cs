using WebApp.Models;

namespace WebApp.Services
{
    public interface IAuthService
    {
        Task<LoginResult> Login(LoginModel loginModel);
        Task Logout();
        Task<RegisterResult> Register(RegisterModel registerModel);
        Task<AuthenticationResult> AuthenticateAdmin();
        Task<string?> GetUserId();
        Task<UserResult> GetUser(string userId);
    }
}
