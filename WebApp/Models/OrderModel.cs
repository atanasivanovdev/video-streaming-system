﻿namespace WebApp.Models
{
    public class OrderModel
    {
        public string Id { get; set; }
        public DateTime PlacedOn { get; set; }

        public string Title { get; set; }

        public string ImageURL { get; set; }

        public double Amount { get; set; }

        public string Status { get; set; }

        public string LastCardDigits { get; set; }

        public string CardExpiration { get; set; }
    }
}
