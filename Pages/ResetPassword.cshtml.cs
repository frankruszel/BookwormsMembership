using BookwormsMembership.Model;
using BookwormsMembership.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Web;

namespace BookwormsMembership.Pages
{
    public class ResetPasswordModel : PageModel
    {
		private UserManager<ApplicationUser> userManager { get; }
		private SignInManager<ApplicationUser> signInManager { get; }
        private readonly AuthDbContext _context;


        [BindProperty]
		public ResetPassword RPModel { get; set; } = new ResetPassword();
		public ResetPasswordModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, AuthDbContext _context)
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
			this._context = _context;
		}
		public void OnGet(string uid, string token,string? errorMsg)
        {
			ViewData["isSuccess"] = false;
			
			//try catch maybe null
			RPModel.Token = token;
			RPModel.UserId = uid;

			if (!String.IsNullOrEmpty(errorMsg) )
			{
				ModelState.AddModelError("", errorMsg);
			}
			



		}
		public async Task<IActionResult> OnPostAsync()
		{
			
			if (ModelState.IsValid)
			{
				//HttpUtility.HtmlEncode()
				RPModel.NewPassword = HttpUtility.HtmlEncode(RPModel.NewPassword);
				RPModel.UserId = HttpUtility.HtmlEncode(RPModel.UserId);


				//RPModel.Token.Replace(' ', '+');
				var myUserID = RPModel.UserId;
				RPModel.Token = RPModel.Token.Replace(' ', '+');
				RPModel.Token = RPModel.Token;
				var myToken =RPModel.Token;
				var myNewPassword = RPModel.NewPassword;
				var myUser = userManager.FindByIdAsync(myUserID).Result; //trycatch


				
				var identityResult = await userManager.ResetPasswordAsync(myUser, myToken, myNewPassword);

                
                
				
                if (identityResult.Succeeded)
				{
					//Contniue to change password

					//Check if password is used before
					//PasswordVerificationResult hash = UserManager.PasswordHasher.VerifyHashedPassword("AHxwg0FI8y7t4owLIedK8YjQjayBntAwp9T2Oqm69SqdRBaN/9hSGe1gmv6Jy6VajA==", "HelloWorld");

					//1)Retrieve PasswordHistroy(String ",")
					//2)Split into array
					//3)Find last 2 index and loop through and verify current password
					var myPasswords = _context.PasswordHistory.FirstOrDefault(x => x.UserId == myUser.Id);
					var myPasswordHistoryList = myPasswords.PasswordHistory.Split(",").ToList();

					var lastTwo = myPasswordHistoryList.Skip(myPasswordHistoryList.Count - 2).ToList();

					bool isReused = false;

					foreach (var password in lastTwo)
					{
						PasswordVerificationResult hash = userManager.PasswordHasher.VerifyHashedPassword(myUser, password, myNewPassword);
						if (hash.HasFlag(PasswordVerificationResult.Success))
						{
							isReused = true;
						}
					}

					if (isReused)
					{
						ModelState.AddModelError("", $"Sorry, you cannot reuse your last two passwords.\n Please choose a new password");
						var token = await userManager.GeneratePasswordResetTokenAsync(myUser);
						return RedirectToPage("ResetPassword", new { uid = myUser.Id, token = token, errorMsg= "Sorry, you cannot reuse your last two passwords.\n Please choose a new password" });


					}

					else
					{
						await _context.passwordChange(myUserID, userManager.FindByIdAsync(myUserID).Result.PasswordHash);
						await _context.passwordChangeAsyncLog(myUser.UserName, identityResult.Succeeded);

						signInManager.PasswordSignInAsync(myUser, myNewPassword, false, true);
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


						var myRetrievedUser = _context.Users.FirstOrDefault(x => x.Id == myUserID);
						myRetrievedUser.PasswordExpired = false;
						
						ViewData["isSuccess"] = true;


						_context.SaveChanges();
						return Page();
					}



					
				}
				foreach (var error in identityResult.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}


			}
			return Page();
		}
	}
}
