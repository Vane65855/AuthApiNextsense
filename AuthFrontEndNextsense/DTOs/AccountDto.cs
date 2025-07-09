namespace AuthFrontEndNextsense.DTOs {
	public class AccountDto {
		public int Id { get; set; }
		public string Username { get; set; }
		public bool IsActive { get; set; }
		public AccountRole Role { get; set; }
	}
}
