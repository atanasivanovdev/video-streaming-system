using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class PaymentModel
    {
        public string TitleId { get; set; }

        public string UserId { get; set; }

		[Required]
		[Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
		public double? Amount { get; set; }

        [Required]
		[RegularExpression("^[0-9]{16}$", ErrorMessage = "The card number must be exactly 16 digits.")]
		[Display(Name = "Card number")]
        public string CardNumber { get; set; }

		[Required]
		[RegularExpression(@"^(0[1-9]|1[0-2])\/?([0-9]{2})$", ErrorMessage = "Expiration date must be in MM/YY format.")]
		[Display(Name = "Expiration Date")]
		public string ExpirationDate { get; set; }

		[Required]
		[RegularExpression("^[0-9]{3}$", ErrorMessage = "The card number must be exactly 3 digits.")]
		public string CVC { get; set; }
	}
}
