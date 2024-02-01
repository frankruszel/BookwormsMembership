using System.ComponentModel.DataAnnotations;

namespace BookwormsMembership.ViewModels
{
	public class ConfirmationEmail
	{

		[Required]
		public string UserId { get; set; }
		[Required]
		public string Token { get; set; }
	}
}
