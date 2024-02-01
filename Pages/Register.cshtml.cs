using BookwormsMembership.Model;
using BookwormsMembership.ViewModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NanoidDotNet;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Web;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookwormsMembership.Pages
{
    [ValidateAntiForgeryToken]
    //Initialize the build-in ASP.NET Identity
    public class RegisterModel : PageModel
    {
        private readonly IWebHostEnvironment _environment;
        private readonly AuthDbContext _context;
        private UserManager<ApplicationUser> userManager { get; }
        private SignInManager<ApplicationUser> signInManager { get; }
        

        [BindProperty]
        public Register RModel { get; set; }
        public RegisterModel(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager, IWebHostEnvironment environment, AuthDbContext context)
        {
            
            this.userManager = userManager;
            this.signInManager = signInManager;
            this._environment = environment;
            this._context = context;
        }
        public void OnGet()
        { 
        }
        
        //Save data into the database
        public async Task<IActionResult> OnPostAsync()
        {

            if (ModelState.IsValid)
            {


                var file = RModel.Photo;
                var id = Nanoid.Generate(size: 10);
                var filename = id + Path.GetExtension(file.FileName);
                var imagePath = Path.Combine(_environment.ContentRootPath,
                @"wwwroot/uploads", filename);
                using var fileStream = new FileStream(imagePath, FileMode.Create);
                file.CopyTo(fileStream);


                var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
                var protector = dataProtectionProvider.CreateProtector("MySecretKey");
                var user = new ApplicationUser()
                {
                    Photo = filename,
                    UserName = HttpUtility.HtmlEncode(RModel.Email),
                    FirstName = HttpUtility.HtmlEncode(RModel.FirstName),
                    LastName = HttpUtility.HtmlEncode(RModel.LastName),
                    MobileNo = HttpUtility.HtmlEncode(RModel.MobileNo),
                    CreditCard = HttpUtility.HtmlEncode(protector.Protect(RModel.CreditCard)),
                    BillingAddress = HttpUtility.HtmlEncode(RModel.BillingAddress),
                    ShippingAddress = HttpUtility.HtmlEncode(RModel.ShippingAddress),
                    Email = HttpUtility.HtmlEncode(RModel.Email),
                    PasswordExpired = false
                    
                };
                
                var result = await userManager.CreateAsync(user, HttpUtility.HtmlEncode(RModel.Password));

                
                // var result = await _context.CreateAsyncWithLog(user, HttpUtility.HtmlEncode(RModel.Password));
                await _context.signUpAsyncLog(user.UserName, result.Succeeded);

                await _context.passwordChange(user.Id, userManager.FindByNameAsync(user.UserName).Result.PasswordHash);
                if (result.Succeeded)
                {
                    //await signInManager.SignInAsync(user, false); || ConfirmEmail?uid={}&token{}
                    

					var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

                    if (!string.IsNullOrEmpty(token))
                    {
						

						    //send token in users email id
						    //"forgot-password?uid={uid}&token={token}http://localhost:7217/reset-password?uid={0}&token={1}

						    //formatting email start
						    string myUserEmail = user.Email;
						    var myUserID = user.Id;
						    string myLink = $"https://localhost:7217/ConfirmEmail?uid={myUserID}&token={token}";
						    var myLinkHTML = $"<a href={myLink}>Verify here</a>";
						    var myBodyHTML = $"Verify your account with the link below <br> {myLinkHTML}";
						    var mySenderAddress = "no-reply@bookwormsmembership";
						    var mySenderDisplayName = "Bookworms team";
						    MailMessage myMail = new MailMessage
						{
							Subject = "Email Confirmation",

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
                        ViewData["isSent"] = true;
					}

					return Page();
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
