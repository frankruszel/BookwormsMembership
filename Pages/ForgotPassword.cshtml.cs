using BookwormsMembership.Model;
using BookwormsMembership.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using static BookwormsMembership.Pages.ForgotPasswordModel;
using System.Net.Mail;
using System.Net;
using System.Text;
using static System.Net.WebRequestMethods;
using System.Web;

namespace BookwormsMembership.Pages
{
    public class ForgotPasswordModel : PageModel
    {

		private UserManager<ApplicationUser> userManager { get; }
		private SignInManager<ApplicationUser> signInManager { get; }

		[BindProperty]
		public ForgotPassword FPModel { get; set; }
		public ForgotPasswordModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
		}

		//start

		
		//end
		public void OnGet()
        {
			
			ViewData["isSent"] = false;
		}

		public async Task<IActionResult> OnPostAsync()
		{

			if (ModelState.IsValid)
			{
				ViewData["isSent"] = true;
				//HttpUtility.HtmlEncode(uid);
				FPModel.Email = HttpUtility.HtmlEncode(FPModel.Email);
				
				var User = await userManager.FindByEmailAsync(FPModel.Email);
				if (User != null)
				{
					var token = await userManager.GeneratePasswordResetTokenAsync(User);

					//send token in users email id
					//"forgot-password?uid={uid}&token={token}http://localhost:7217/reset-password?uid={0}&token={1}

					//formatting email start
					string myUserEmail = User.Email;
					var myUserID = User.Id;
					string myLink = $"https://localhost:7217/ResetPassword?uid={myUserID}&token={token}";
					var myLinkHTML = $"<a href={myLink}>Verify here</a>";
					var myBodyHTML = $"Reset your password with the link below <br> {myLinkHTML}";
					var mySenderAddress = "no-reply@bookwormsmembership";
					var mySenderDisplayName = "Bookworms team";
					
					MailMessage myMail = new MailMessage
					{
						Subject = "Forgot Password",
						
						Body = myBodyHTML,
						From = new MailAddress(mySenderAddress, mySenderDisplayName),
						IsBodyHtml = true,
					};
					myMail.To.Add(myUserEmail);
					//formatting email end

					//sending email start
					var client = new SmtpClient("sandbox.smtp.mailtrap.io", 2525)
					{
						Credentials = new NetworkCredential("3278a2627e2535", "d605f0bd409b87"),
						EnableSsl = true
					};
					myMail.BodyEncoding = Encoding.UTF8;
					client.SendMailAsync(myMail);
					//sending email end

					
				}

			}
			return Page();
		}
	}
}
