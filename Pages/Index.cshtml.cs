using BookwormsMembership.Model;
using BookwormsMembership.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookwormsMembership.Pages
{
 
    [Authorize]
    public class IndexModel : PageModel
    {
        private UserManager<ApplicationUser> userManager { get; }
        private SignInManager<ApplicationUser> signInManager { get; }


        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public async void OnGet()
        {
			_logger.LogInformation("this is my authtoken Cookie {d}", Request.Cookies["AuthToken"]);
			_logger.LogInformation("this is my authtoken Session {d}", HttpContext.Session.GetString("AuthToken"));
			//Cookies sessions r not to be the same
			if (HttpContext.Request.Cookies["AuthToken"] != null &&  null != HttpContext.Session.Keys.FirstOrDefault(x => x == "AuthToken" ))
            {
				
				if (!HttpContext.Session.GetString("AuthToken").Equals(HttpContext.Request.Cookies["AuthToken"].ToString()))
                {
                    //Session fixation prevention
					_logger.LogInformation("Does not match", DateTime.UtcNow.ToString());

					await signInManager.SignOutAsync();

                    HttpContext.Session.Clear();
                    foreach (var cookie in Request.Cookies)
                    {
                        Response.Cookies.Delete(cookie.Key);
                    }

					Response.Redirect("/Login");
				}
                else
                {
                    //Continue session
                    _logger.LogInformation("Home Page visited {DT}", DateTime.UtcNow.ToString());
                }
            }
            else
            {
				await signInManager.SignOutAsync();
				foreach (var cookie in Request.Cookies)
				{
					Response.Cookies.Delete(cookie.Key);
				}
				HttpContext.Session.Clear();
				Response.Redirect("/Login");
			}


			
        }
        public async Task<IActionResult> OnPostAsync()
        {
            
            var myUser = userManager.GetUserAsync(User).Result; //trycatch
            bool isAuthenticated = myUser.TwoFactorEnabled;
            
            if (isAuthenticated == true)
            {
                isAuthenticated = false;
                
                
            }
            else
            {
                isAuthenticated = true;
                
            }
            var identityResult = await userManager.SetTwoFactorEnabledAsync(myUser, isAuthenticated);
   
                return Page();
            
           
           

        }
    }
}