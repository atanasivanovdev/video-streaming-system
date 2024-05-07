using CustomerService.Models;
using WebApp.Models;

namespace WebApp.Services
{
    public interface IInboxService
    {
        Task<InboxResult> GetMessages(string userId);
        Task<InboxResult> PublishUpcoming(UpcomingVideo upcomingVideo);
    }
}
