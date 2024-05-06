﻿using Google.Cloud.PubSub.V1;

namespace OrderService.Services
{
    public class PubSubHostedService : BackgroundService
    {
        private readonly OrderManagementService _orderService;

        public PubSubHostedService(OrderManagementService orderService)
        {
            _orderService = orderService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _orderService.SubscribeAsync(stoppingToken);
                    await Task.Delay(10000, stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }
    }
}