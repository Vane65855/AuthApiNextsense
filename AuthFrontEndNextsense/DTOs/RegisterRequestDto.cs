using System.ComponentModel.DataAnnotations;

namespace AuthFrontEndNextsense.DTOs {
	public class RegisterRequestDto {
		[Required]
		[StringLength(50, MinimumLength = 3)]
		public string Username { get; set; }

		[Required]
		[StringLength(100, MinimumLength = 8)]
		public string Password { get; set; }

		[Required]
		[Compare("Password", ErrorMessage = "Passwords do not match.")]
		public string ConfirmPassword { get; set; }

		[Required]
		public AccountRole Role { get; set; } = AccountRole.User;

		[Required]
		public bool IsActive { get; set; }
	}
}
