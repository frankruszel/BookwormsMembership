using System.ComponentModel.DataAnnotations;

namespace BookwormsMembership.ViewModels
{
	public class ResetPassword
	{
		[Required]
		public string UserId { get; set; }
		[Required]
		public string Token { get; set; }
		[Required]
		[MinLength(12, ErrorMessage = "Enter at least a 12 characters password")]
		[DataType(DataType.Password)]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&]{12,}$", ErrorMessage = "Passwords must be at least 8 characters long and contain at least an upper case letter, lower case letter, digit and a symbol")]
		public string NewPassword { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Compare(nameof(NewPassword), ErrorMessage = "Password and confirmation password does not match")]
		public string ConfirmNewPassword { get; set; }
	}
}
