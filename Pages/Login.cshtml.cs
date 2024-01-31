using BookwormsMembership.Model;
using BookwormsMembership.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace BookwormsMembership.Pages
{
    [ValidateAntiForgeryToken]
    public class LoginModel : PageModel
    {
		[BindProperty]
		public Login LModel { get; set; }

		private readonly SignInManager<ApplicationUser> signInManager;
		public LoginModel(SignInManager<ApplicationUser> signInManager)
		{
			this.signInManager = signInManager;
		}
		public void OnGet()
        {
        }


		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				var identityResult = await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password,
				LModel.RememberMe, false);
				if (identityResult.Succeeded)
				{	
					
					//Create the security context
					var claims = new List<Claim>
					{
						new Claim(ClaimTypes.Name, "c@c.com"),
						new Claim(ClaimTypes.Email,"c@c.com"),
                        
                    };
					//Response.Cookies.
					var i = new ClaimsIdentity(claims, "MyCookieAuth");
					ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(i);
					await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal);
					return RedirectToPage("Index");
				}
				ModelState.AddModelError("", "Username or Password incorrect");
			}
			return Page();
		}

	}
}
