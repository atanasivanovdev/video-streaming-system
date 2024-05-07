using CustomerService.Models;
using System.Net.Http.Json;
using WebApp.Models;

namespace WebApp.Services
{
    public class InboxService : IInboxService
    {
        private readonly HttpClient _httpClient;

        public InboxService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<InboxResult> GetMessages(string userId)
        {
            var result = await _httpClient.GetAsync($"gateway/inbox/{userId}");

            InboxResult inboxResult = new InboxResult();

            inboxResult.Successful = result.IsSuccessStatusCode;

            if (!inboxResult.Successful)
            {
                var error = await result.Content.ReadAsStringAsync();
                inboxResult.Error = error;
                return inboxResult;
            }

            inboxResult.Messages = await result.Content.ReadFromJsonAsync<List<Message>>();

            return inboxResult;
        }

        public async Task<InboxResult> PublishUpcoming(UpcomingVideo upcomingVideo)
        {
            var result = await _httpClient.PostAsJsonAsync($"gateway/inbox/upcoming", upcomingVideo);

            InboxResult inboxResult = new InboxResult();

            inboxResult.Successful = result.IsSuccessStatusCode;

            if (!inboxResult.Successful)
            {
                var error = await result.Content.ReadAsStringAsync();
                inboxResult.Error = error;
                return inboxResult;
            }

            return inboxResult;
        }
    }
}
