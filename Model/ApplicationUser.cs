using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BookwormsMembership.Model
{
	public class ApplicationUser : IdentityUser
	{
        public string Photo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNo { get; set; }
        public string CreditCard { get; set; }
        public string BillingAddress { get; set; }
        public string ShippingAddress { get; set; }
        [Required]
        public bool PasswordExpired { get; set; } = false;
        
        



    }
}
