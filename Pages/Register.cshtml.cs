using BookwormsMembership.Model;
using BookwormsMembership.ViewModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Web;

namespace BookwormsMembership.Pages
{
    [ValidateAntiForgeryToken]
    //Initialize the build-in ASP.NET Identity
    public class RegisterModel : PageModel
    {
        private UserManager<ApplicationUser> userManager { get; }
        private SignInManager<ApplicationUser> signInManager { get; }

        [BindProperty]
        public Register RModel { get; set; }
        public RegisterModel(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        public void OnGet()
        { 
        }
        
        //Save data into the database
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
				RModel.Email = HttpUtility.HtmlEncode(RModel.Email);
                

                var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
                var protector = dataProtectionProvider.CreateProtector("MySecretKey");
                var user = new ApplicationUser()
                {
                    UserName = HttpUtility.HtmlEncode(RModel.Email),
                    FirstName = HttpUtility.HtmlEncode(RModel.FirstName),
                    LastName = HttpUtility.HtmlEncode(RModel.LastName),
                    MobileNo = HttpUtility.HtmlEncode(RModel.MobileNo),
                    CreditCard = HttpUtility.HtmlEncode(protector.Protect(RModel.CreditCard)),
                    BillingAddress = HttpUtility.HtmlEncode(RModel.BillingAddress),
                    ShippingAddress = HttpUtility.HtmlEncode(RModel.ShippingAddress),
                    Email = HttpUtility.HtmlEncode(RModel.Email)
                };
                
                var result = await userManager.CreateAsync(user, RModel.Password);
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, false);
                    return RedirectToPage("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return Page();
        }

    }
}
