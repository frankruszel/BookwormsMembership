namespace BookwormsMembership.Model
{
	public class Audit
	{
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Type { get; set; }
        public bool isSuccessful { get; set; }
        public DateTime Time { get; set; }
    }
}
