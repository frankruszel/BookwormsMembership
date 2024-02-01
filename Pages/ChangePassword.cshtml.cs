using BookwormsMembership.Model;
using BookwormsMembership.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NanoidDotNet;
using System;
using System.Web;

namespace BookwormsMembership.Pages
{
	[Authorize]
    public class ChangePasswordModel : PageModel
    {
		private UserManager<ApplicationUser> userManager { get; }
		private SignInManager<ApplicationUser> signInManager { get; }
        private readonly AuthDbContext _context;

        [BindProperty]
		public ChangePassword CPModel { get; set; }
		public ChangePasswordModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, AuthDbContext context)
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
			this._context = context;
		}
		public void OnGet()
        {
			ViewData["isSuccess"] = false;
		}

		public async Task<IActionResult> OnPostAsync()
		{

			if (ModelState.IsValid)
			{
                
                CPModel.NewPassword = HttpUtility.HtmlEncode(CPModel.NewPassword);
				CPModel.CurrentPassword = HttpUtility.HtmlEncode(CPModel.CurrentPassword);
                var myUser = userManager.GetUserAsync(User).Result; //trycatch

				//1)Retrieve LastChange Password
				var myUserPasswordHistory = _context.PasswordHistory.FirstOrDefault(x => x.UserId == myUser.Id);
				//2)Check whether its passed the minimum password age
				DateTime myRetrievedLastChanged = myUserPasswordHistory.LastChanged;
				// Add the minimum minutes from the lastChanged
				DateTime minimumTimingToPass = myRetrievedLastChanged.AddMinutes(1); //1min is the sample
				var timeNow = DateTime.Now;
				//Check if current time has passed the minimum
				bool hasPassedMinimum = timeNow > minimumTimingToPass;

				if (!hasPassedMinimum)
				{
					//Never pass minimum, tell user to wait __ amount of time
					var needToWait = minimumTimingToPass - timeNow;
					needToWait = needToWait.Duration();
					string formattedDuration = "";

					if (needToWait.Hours > 0)
					{
						formattedDuration += $"{needToWait.Hours}h ";
					}

					if (needToWait.Minutes > 0)
					{
						formattedDuration += $"{needToWait.Minutes}m ";
					}
					if (needToWait.Seconds > 0 && needToWait.Minutes == 0) // Only include seconds if there are no minutes
					{
						formattedDuration += $"{needToWait.Seconds}s";
					}
					ModelState.AddModelError("", $"Please wait {formattedDuration} more to change your password.");
					return Page();
				}
				else
				{
					//Contniue to change password

					//Check if password is used before
					//PasswordVerificationResult hash = UserManager.PasswordHasher.VerifyHashedPassword("AHxwg0FI8y7t4owLIedK8YjQjayBntAwp9T2Oqm69SqdRBaN/9hSGe1gmv6Jy6VajA==", "HelloWorld");

					//1)Retrieve PasswordHistroy(String ",")
					//2)Split into array
					//3)Find last 2 index and loop through and verify current password
					var myPasswords = _context.PasswordHistory.FirstOrDefault(x =>x.UserId == myUser.Id);
					var myPasswordHistoryList = myPasswords.PasswordHistory.Split(",").ToList();

					var lastTwo = myPasswordHistoryList.Skip(myPasswordHistoryList.Count - 2).ToList();

					bool isReused = false;

					foreach(var password in lastTwo)
					{
						PasswordVerificationResult hash = userManager.PasswordHasher.VerifyHashedPassword(myUser,password, CPModel.NewPassword);
						if (hash.HasFlag(PasswordVerificationResult.Success))
						{
							isReused = true;
						}
					}

					if (isReused)
					{
						ModelState.AddModelError("", $"Sorry, you cannot reuse your last two passwords.\n Please choose a new password");
					}

					else
					{
						var identityResult = await userManager.ChangePasswordAsync(myUser, CPModel.CurrentPassword, CPModel.NewPassword);
						await _context.passwordChangeAsyncLog(myUser.UserName, identityResult.Succeeded);
						myUser.PasswordExpired = false;
						if (identityResult.Succeeded)
						{
							await _context.passwordChange(myUser.Id, userManager.FindByIdAsync(myUser.Id).Result.PasswordHash);
							_context.SaveChanges();
							ViewData["isSuccess"] = true;
							return Page();
						}
						foreach (var error in identityResult.Errors)
						{
							ModelState.AddModelError("", error.Description);
						}
					}




					
				}

				


			}
			return Page();
		}
	}
}
