using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BookwormsMembership.ViewModels
{
    public class Register
    {

		
	[Required]
		[DataType(DataType.PhoneNumber)]
		[RegularExpression(@"^\d{8}$",ErrorMessage = "Please enter a valid mobile no")]
		public string MobileNo { get; set; }

		[Required]
		[DataType(DataType.Text)]
		public string BillingAddress { get; set; }

		[Required]
		[DataType(DataType.Text)]
		//(allow all special chars)
		[RegularExpression(@"^[a-zA-Z0-9\s!""#$%&'()*+,\-./:;<=>?@[\\\]^_`{|}~]+$")]
		public string ShippingAddress { get; set; }

		[Required]
		[DataType(DataType.Text)]

		public string FirstName { get; set; }

		[Required]
		[DataType(DataType.Text)]
		public string LastName { get; set; }

		[Required]
		[DataType(DataType.CreditCard)]
        [MinLength(14, ErrorMessage = "Enter valid credit card")]
        //(Must be unique)
        public string CreditCard { get; set; }

		[Required]
		[DataType(DataType.EmailAddress)]
		//(Must be unique)
		public string Email { get; set; }

		[Required]
		[MinLength(12, ErrorMessage = "Enter at least a 12 characters password")]
		[DataType(DataType.Password)]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&]{12,}$", ErrorMessage = "Passwords must be at least 8 characters long and contain at least an upper case letter, lower case letter, digit and a symbol")]
		public string Password { get; set; }

		[Required]
		[DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password and confirmation password does not match")]
        public string ConfirmPassword { get; set; }

		//[Required]
		//[DataType(DataType.Upload)]
		////(.JPG only)
		//public string Photo { get; set; }
	}
}
