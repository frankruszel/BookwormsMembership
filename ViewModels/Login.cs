using System.ComponentModel.DataAnnotations;

namespace BookwormsMembership.ViewModels
{
    public class Login
    {
		[Required]
		public string Token { get; set; }
		[Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }

    }
}
