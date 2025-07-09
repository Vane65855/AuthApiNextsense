using AuthApiNextsense.Enums;

namespace AuthApiNextsense.Models {
	public class Account {
		public int Id { get; set; }
		public string Username { get; set; } 
		public string PasswordHash { get; set; }
		public bool IsActive { get; set; } 
		public AccountRole Role { get; set; } 
		public DateTime SecurityTimestamp { get; set; } 
	}
}
