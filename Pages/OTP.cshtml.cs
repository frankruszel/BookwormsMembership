using BookwormsMembership.Model;
using BookwormsMembership.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using System.Web;

namespace BookwormsMembership.Pages
{
    public class OTPModel : PageModel
    {
		private UserManager<ApplicationUser> userManager { get; }
		private SignInManager<ApplicationUser> signInManager { get; }
        private readonly AuthDbContext _context;

        [BindProperty]
		public OTP TFAModel { get; set; } = new OTP();
		public OTPModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, AuthDbContext context)
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
			this._context = context;
		}
		public void OnGet(string uid)
        {
			ViewData["isSuccess"] = false;

			//try catch maybe null
			TFAModel.UserId = uid;
		}

		public async Task<IActionResult> OnPostAsync()
		{

			if (ModelState.IsValid)
			{
				TFAModel.UserId = HttpUtility.HtmlEncode(TFAModel.UserId);
				TFAModel.Code = HttpUtility.HtmlEncode(TFAModel.Code);
				string myCode = TFAModel.Code;
				var identityResult = await signInManager.TwoFactorSignInAsync("Email", myCode, false,false);

                await _context.logInAsyncLog(userManager.FindByIdAsync(TFAModel.UserId).Result.Email, identityResult.Succeeded);

                if (identityResult.Succeeded)
				{
                    
                    Console.WriteLine("succeeded");
					ViewData["isSuccess"] = true;
					//Create the security context
					var claims = new List<Claim>
						{
							new Claim(ClaimTypes.Name, "c@c.com"),
							new Claim(ClaimTypes.Email,"c@c.com"),

						};
					//Response.Cookies.
					var i = new ClaimsIdentity(claims, "MyCookieAuth");
					ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(i);

					string guid = Guid.NewGuid().ToString();

					HttpContext.Session.SetString("AuthToken", guid);
					HttpContext.Response.Cookies.Append("AuthToken", guid);

					await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal); //cookie timeout for this

					return Page();
				}
				else
				{
					Console.WriteLine("failed");
					ModelState.AddModelError("", "OTP does not match");
				}
			


			}
			return Page();
		}


	}
}
