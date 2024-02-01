using BookwormsMembership.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;



namespace BookwormsMembership.Pages
{
    public class LogoutModel : PageModel
    {
		private readonly SignInManager<ApplicationUser> signInManager;
        private readonly AuthDbContext _context;
        public LogoutModel(SignInManager<ApplicationUser> signInManager,AuthDbContext context)
		{
			this.signInManager = signInManager;
			this._context = context;
		}
		public void OnGet() { }
		public async Task<IActionResult> OnPostLogoutAsync()
		{

			var myUserName = User.Identity.Name;

			await signInManager.SignOutAsync();
			foreach (var cookie in Request.Cookies)
			{
				Response.Cookies.Delete(cookie.Key);
			}
			HttpContext.Session.Clear();
			await _context.logOutAsyncLog(myUserName, true);

            return RedirectToPage("Login");
		}
		public async Task<IActionResult> OnPostDontLogoutAsync()
		{
			return RedirectToPage("Index");
		}
	}
}
