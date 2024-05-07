using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Shared
{
    public class PaymentBase : ComponentBase
    {
        [Inject]
        public IPaymentService PaymentService { get; set; }

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        [Inject]
        public IWebAssemblyHostEnvironment Environment { get; set; }

        [Inject]
        public IAuthService AuthService { get; set; }

        [Inject]
        public IWatchlistService WatchlistService { get; set; }

        public PaymentModel paymentModel = new PaymentModel();

        private double _price;

        public string errorMessage;

        [Parameter]
		public string TitleId { get; set; }

		[Parameter]
        public double Price
        {
            get => _price;
            set
            {
                if (_price != value)
                {
                    _price = value;
                    paymentModel.Amount = value;
                }
            }
        }

        public async void HandleRegistration()
        {
            var userId = await AuthService.GetUserId();
            WatchlistModel watchlistModel = new WatchlistModel();
            if (userId == null)
            {
                errorMessage = "Failed to retrieve user ID.";
                StateHasChanged();
                return;
            }

            paymentModel.UserId = userId;
            watchlistModel.UserId = userId;

            if (string.IsNullOrEmpty(TitleId))
            {
                errorMessage = "Title ID is missing.";
                StateHasChanged();
                return;
            }

            paymentModel.TitleId = TitleId;
            watchlistModel.TitleId = TitleId;

            // Process payment
            var paymentResult = await PaymentService.ProcessPayment(paymentModel);
            if (!paymentResult.Successful)
            {
                errorMessage = $"Failed to process payment.";
                StateHasChanged();
                return;
            }

            // Remove title from watchlist
            var removeResult = await WatchlistService.RemoveTitleFromWatchlist(watchlistModel);
            if (!removeResult.Successful)
            {
                errorMessage = $"Failed to remove title from watchlist.";
                StateHasChanged();
                return;
            }

            // All operations successful, close modal
            await JSRuntime.InvokeVoidAsync("closeModal", "paymentForm");

            StateHasChanged();
        }

        public string amountString
        {
            get => paymentModel.Amount?.ToString()!;
            set
            {
                if (double.TryParse(value, out var result))
                {
                    paymentModel.Amount = result;
                }
                else
                {
                    paymentModel.Amount = null;
                }
            }
        }

        public string GetImagePath()
        {
            string webRootPath = Environment.BaseAddress;
            return Path.Combine(webRootPath, "cards-logo.png");
        }
    }
}
