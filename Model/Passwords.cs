using System.ComponentModel.DataAnnotations.Schema;

namespace BookwormsMembership.Model
{
    public class Passwords
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        
        public string PasswordHistory { get; set; }
        public DateTime LastChanged { get; set; }
    }
}
