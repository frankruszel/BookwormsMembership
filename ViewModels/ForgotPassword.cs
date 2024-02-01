using System.ComponentModel.DataAnnotations;

namespace BookwormsMembership.ViewModels
{
	public class ForgotPassword
	{
		[Required, EmailAddress]
		[DataType(DataType.EmailAddress)]
		public string Email { get; set; }	
		public bool EmailSent { get; set; }
	}
}
