
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;

namespace BookwormsMembership.Model
{
    public class PasswordEntry
    {
        public string UserId { get; set; }
        public List<string> PasswordHistory { get; set; } = new List<string>();

        public Passwords ToPassword()
        {
            var passwords = new Passwords();
            passwords.UserId = UserId;
            passwords.PasswordHistory = PasswordHistory.Count == 0 ? null : string.Join(",",PasswordHistory);
            passwords.LastChanged = DateTime.Now;

            return passwords;
        }
        

    }
}
