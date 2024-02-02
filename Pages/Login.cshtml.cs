using BookwormsMembership.Model;
using BookwormsMembership.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Text.Json.Serialization;
using static System.Net.WebRequestMethods;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace BookwormsMembership.Pages
{
	

    [ValidateAntiForgeryToken]
    public class LoginModel : PageModel
    {
		[BindProperty]
		public Login LModel { get; set; }

		private readonly SignInManager<ApplicationUser> signInManager;
        private UserManager<ApplicationUser> userManager { get; }
		private readonly ILogger<IndexModel> _logger;
		private readonly AuthDbContext _context;
        public LoginModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, AuthDbContext context, ILogger<IndexModel> logger)
		{
			this.signInManager = signInManager;
            this.userManager = userManager;
			this._context = context;
            _logger = logger;
        }

        //reCaptchav3 Start
        public class MyObject
        {
            public bool success { get; set; }
            public double score { get; set; }
            public List<string> ErrorMessage { get; set; }
        }
        public async Task<bool> VerifyToken(string token)
        {
            try
            {
                var url = $"https://www.google.com/recaptcha/api/siteverify?secret=6LcFoGEpAAAAAJTXBl5vjMwyZHXLa6GBmbaVS_yo&response={token}";

                using (var client = new HttpClient())
                {
                    var httpResult = await client.GetAsync(url);
                    if (httpResult.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        return false;

                    }
                    var responseString = await httpResult.Content.ReadAsStringAsync();

                    var googleResult = JsonConvert.DeserializeObject<MyObject>(responseString);

                    return googleResult.success && googleResult.score >= 0.5;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
        //reCaptchav3 End
        public void OnGet()
        {

        }


		public async Task<IActionResult> OnPostAsync()
		{
			
			LModel.Email = HttpUtility.HtmlEncode(LModel.Email);
			LModel.Password = HttpUtility.HtmlEncode(LModel.Password);
            //Verify response token with google
            var captchaResult = await VerifyToken(LModel.Token);
            if(!captchaResult)
            {
                //Does not pass
                return Page();
            }

			if (ModelState.IsValid)
			{
                var userEmailTrue = await userManager.FindByEmailAsync(LModel.Email);
				if (userEmailTrue != null)
				{
                    //var identityResult = await _context.PasswordSignInAsyncWithLog(LModel.Email, LModel.Password, LModel.RememberMe, true);
                    var identityResult = await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password, LModel.RememberMe, true);
                    
                    if (identityResult.IsLockedOut)
                    {
                        var lockoutEnd = userEmailTrue.LockoutEnd;

                        if (lockoutEnd != null)
                        {
                            var currentTime = DateTimeOffset.Now;
                            var lockoutEndTime = (DateTimeOffset)lockoutEnd;

                            TimeSpan lockoutTimeLeft = lockoutEndTime - currentTime;
                            
							lockoutTimeLeft = lockoutTimeLeft.Duration();
							string formattedDuration = "";

							if (lockoutTimeLeft.Hours > 0)
							{
								formattedDuration += $"{lockoutTimeLeft.Hours}h ";
							}

							if (lockoutTimeLeft.Minutes > 0)
							{
								formattedDuration += $"{lockoutTimeLeft.Minutes}min ";
							}
							if (lockoutTimeLeft.Seconds > 0 && lockoutTimeLeft.Minutes < 60) // Only include seconds if there are no minutes
							{
								formattedDuration += $"{lockoutTimeLeft.Seconds}s";
							}
							
							ModelState.Clear();
							ModelState.AddModelError("", $"Account locked. Try again after {formattedDuration}");
							return Page();
						}




					}
                    
                    else if (identityResult.RequiresTwoFactor == true)
                    {
                        var myUser = await userManager.FindByEmailAsync(LModel.Email);
                        await signInManager.SignOutAsync();

                        await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password, LModel.RememberMe, true);
                        //ViewData OTP tell user sent
                        var token = await userManager.GenerateTwoFactorTokenAsync(myUser, "Email");
                        var myUserID = myUser.Id;
                        //formatting email start
                        string myUserEmail = myUser.Email;

                        //string myLink = $"https://localhost:7217/ResetPassword?uid={myUserID}&token={token}"; //Change
                        var myBodyHTML = $"Your OTP Verification Code <br> Code: {token}";
                        var mySenderAddress = "no-reply@bookwormsmembership";
                        var mySenderDisplayName = "Bookworms team";

                        MailMessage myMail = new MailMessage
                        {
                            Subject = "2 Factor Authentication",

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
                        //sending email stop
                        return RedirectToPage("OTP", new { uid = myUserID });
                    }


					//else if password is expired
					else if (identityResult.Succeeded && userManager.FindByEmailAsync(LModel.Email).Result.PasswordExpired == true)
					{
                        var myUser = userManager.FindByEmailAsync(LModel.Email).Result;
						var myToken = await userManager.GeneratePasswordResetTokenAsync(myUser);

                        //send token in users email id
                        //"forgot-password?uid={uid}&token={token}http://localhost:7217/reset-password?uid={0}&token={1}
                        var link = $"<a href=https://localhost:7217/ResetPassword?uid={myUser.Id}&token={myToken}>Here</a>";
						ModelState.AddModelError("", $"Password Expire \n Please reset your password {link}");
						return RedirectToPage("ResetPassword", new { uid = myUser.Id, token = myToken, errorMsg="Your password has expired"});
						


					}


					else if (identityResult.Succeeded)
                    {

                        var myUser = await userManager.FindByEmailAsync(LModel.Email);

                        //start check password 
                        //Minimum age of password is 1min
                        //Maximum password change is 5mins
                        //1) Check last changed password of user
                        var myUserPasswordHistory = _context.PasswordHistory.FirstOrDefault(x => x.UserId == myUser.Id);
                        if (myUserPasswordHistory == null)
                        {
                            Console.WriteLine("no password hisstory");
                        }
                        else
                        {

                            DateTime myRetrievedLastChanged = myUserPasswordHistory.LastChanged;
							// Subtract 5 minutes from the current time
							DateTime fiveMinutesAgo = DateTime.Now.AddMinutes(-5);
                            bool hasPassedMaximumTime = myRetrievedLastChanged < fiveMinutesAgo;
                            if (hasPassedMaximumTime)
                            {
								//Reset Password set password expire true
								
								myUser.PasswordExpired = true;
                                _context.SaveChanges();
								var myToken = await userManager.GeneratePasswordResetTokenAsync(myUser);
								
								//send token in users email id
								//"forgot-password?uid={uid}&token={token}http://localhost:7217/reset-password?uid={0}&token={1}
								var link = $"<a href=https://localhost:7217/ResetPassword?uid={myUser.Id}&token={myToken}>Here</a>";
								ModelState.AddModelError("", $"Password Expired \n Please reset your password {link}");
								return RedirectToPage("ResetPassword", new { uid = myUser.Id, token = myToken, errorMsg = "Your password has expired" });

							}
                            else
                            {
                                //Authenticate
                                //else start
                                await _context.logInAsyncLog(LModel.Email, identityResult.Succeeded);


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

                                await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal);

                                _logger.LogInformation($"Session {HttpContext.Session.GetString("AuthToken")}");
                                return RedirectToPage("Index");
                                //else end
                            }
                        }

                        

                       
                    }
                    ModelState.AddModelError("", "Username or Password incorrect");
                }
                else
                {
                    ModelState.AddModelError("", "Username or Password incorrect");
                }
               

            }
			return Page();
		}

	}
	
}
