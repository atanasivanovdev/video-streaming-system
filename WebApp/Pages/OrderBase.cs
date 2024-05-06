﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Pages
{
    public class OrderBase : ComponentBase
    {
        [Inject]
        public IOrderService OrderService { get; set; }

        [Inject]
        public IAuthService AuthService { get; set; }

        [Inject]
        public IWebAssemblyHostEnvironment Environment { get; set; }

        public OrderResult Orders { get; set; }
        private string userId = "";

        protected override async Task OnInitializedAsync()
        {
            AuthenticationResult authenticationResult = await AuthService.AuthenticateUser();
            if (!authenticationResult.Successful) return;
            userId = authenticationResult.AuthenticatedUser.UserId;

            Orders = await OrderService.GetOrders(userId);
        }

        public string GetImagePath()
        {
            string webRootPath = Environment.BaseAddress;
            return Path.Combine(webRootPath, "no-image.jpg");
        }
    }
}