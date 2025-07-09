using AuthApiNextsense.Enums;

namespace AuthApiNextsense.DTOs {
	public class AccountDto {
		public required int Id { get; set; }
		public required string Username { get; set; }
		public required bool IsActive { get; set; }
		public required AccountRole Role { get; set; }
	}
}
