using Microsoft.AspNetCore.Components;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Pages
{
    public class InboxBase : ComponentBase
    {
        [Inject]
        public IInboxService InboxService { get; set; }

        [Inject]
        public IAuthService AuthService { get; set; }

        public InboxResult Inbox { get; set; }

        private string userId = "";

        protected override async Task OnInitializedAsync()
        {
            AuthenticationResult authenticationResult = await AuthService.AuthenticateUser();
            if (!authenticationResult.Successful) return;
            userId = authenticationResult.AuthenticatedUser.UserId;

            Inbox = await InboxService.GetMessages(userId);
        }
    }
}
