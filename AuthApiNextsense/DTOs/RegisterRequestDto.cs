using AuthApiNextsense.Enums;

namespace AuthApiNextsense.DTOs {
	public class RegisterRequestDto {
		public string Username { get; set; }
		public string Password { get; set; }
		public bool IsActive { get; set; }
		public AccountRole Role { get; set; }
	}
}
