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
            userId = await AuthService.GetUserId();
            if (userId == null) return;

            Inbox = await InboxService.GetMessages(userId);
        }
    }
}
