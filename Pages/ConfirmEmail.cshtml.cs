using BookwormsMembership.Model;
using BookwormsMembership.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Web;

namespace BookwormsMembership.Pages
{
    public class ConfirmEmailModel : PageModel
    {
		private UserManager<ApplicationUser> userManager { get; }
		private SignInManager<ApplicationUser> signInManager { get; }

		[BindProperty]
		public ConfirmationEmail CEModel { get; set; } = new ConfirmationEmail();
		public ConfirmEmailModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
		}
		public async Task OnGet(string uid, string token)
		{
            ViewData["isSuccess"] = false;
			
            //try catch maybe null
            CEModel.Token = token;
			CEModel.UserId = HttpUtility.HtmlEncode(uid);
			token = token.Replace(" ", "+");
            token = HttpUtility.HtmlEncode(token);
            if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(uid))
			{
				var identityResult = await userManager.ConfirmEmailAsync(await userManager.FindByIdAsync(uid), token);

				if(identityResult.Succeeded)
				{
					Console.WriteLine("Succeded");
					ViewData["isSuccess"] = true;

                }
				

			}


		}

		public async Task ConfirmEmail(string uid, string token)
        {
			
        }
    }
}
