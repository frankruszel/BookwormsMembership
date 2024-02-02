

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.AccessControl;
using System;
using Microsoft.AspNetCore.Identity;
using System.Web;
using Newtonsoft.Json;

namespace BookwormsMembership.Model
{
    public class AuthDbContext : IdentityDbContext<ApplicationUser>
    {
		
		private readonly IConfiguration _configuration;
        private readonly SignInManager<ApplicationUser> signInManager;
        private UserManager<ApplicationUser> userManager { get; }
        //public AuthDbContext(DbContextOptions<AuthDbContext> options):base(options){ }
        public AuthDbContext(IConfiguration configuration)
        {
            _configuration = configuration;

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = _configuration.GetConnectionString("AuthConnectionString"); optionsBuilder.UseSqlServer(connectionString);
        }

        public DbSet<Audit> Audits { get; set; }

        public virtual async Task<bool> logInAsyncLog(string username, bool isSucccess)
        {
            var myAudit = new Audit()
            {
                UserName = username,
                Type = "logIn",
                isSuccessful = isSucccess,
                Time = DateTime.Now
            };
              Audits.Add(myAudit);
            base.SaveChanges();
            return true;
        }

       
        public virtual async Task<bool> signUpAsyncLog(string username, bool isSucccess)
        {
            var myAudit = new Audit()
            {
                UserName = username,
                Type = "signUp",
                isSuccessful = isSucccess,
                Time = DateTime.Now
            };

            Audits.Add(myAudit);
            base.SaveChanges();

           return true;
        }

        public virtual async Task<bool> logOutAsyncLog(string username, bool isSucccess)
        {
            var myAudit = new Audit()
            {
                UserName = username,
                Type = "logOut",
                isSuccessful = isSucccess,
                Time = DateTime.Now
            };

            Audits.Add(myAudit);
            base.SaveChanges();

            return true;
        }
        public virtual async Task<bool> passwordChangeAsyncLog(string username, bool isSucccess)
        {
            var myAudit = new Audit()
            {
                UserName = username,
                Type = "resetPassword",
                isSuccessful = isSucccess,
                Time = DateTime.Now
            };

            Audits.Add(myAudit);
            base.SaveChanges();


            return true;
        }

        public DbSet<Passwords> PasswordHistory { get; set; }

        public virtual async Task<bool> passwordChange(string uid, string newPasswordHashed)
        {
            var myPasswords = PasswordHistory.FirstOrDefault(x => x.UserId == uid);
            if (myPasswords != null) 
            {
                var myPasswordEntry = new PasswordEntry();
				int commaIndex = myPasswords.PasswordHistory.IndexOf(',');

				myPasswords.PasswordHistory = myPasswords.PasswordHistory.Substring(commaIndex + 1) + "," + newPasswordHashed;
                
                //Password List end
                myPasswords.LastChanged = DateTime.Now;
                
			
			}
            else
            {
                var newUserPasswords = new Passwords()
                {
                    UserId = uid,
                    LastChanged = DateTime.Now,

                };
                newUserPasswords.PasswordHistory = newPasswordHashed;

                PasswordHistory.Add(newUserPasswords);
                
			}
            

			base.SaveChanges();
			return true;

		}







    }
}
