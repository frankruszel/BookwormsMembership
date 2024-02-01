using System.ComponentModel.DataAnnotations;

namespace BookwormsMembership.ViewModels
{
	public class OTP
	{
		[Required]
		public string UserId { get; set; }
		[Required]
		public string Code { get; set; }
	}
}
