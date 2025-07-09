using AuthApiNextsense.DTOs;
using AuthApiNextsense.Helpers;
using AuthApiNextsense.Models;

namespace AuthApiNextsense.Extensions {
	public static class MappingExtensions {
		public static AccountDto ToDto(this Account account) {
			return new AccountDto {
				Id = account.Id,
				Username = account.Username,
				Role = account.Role,
				IsActive = account.IsActive
			};
		}
		public static Account ToNewAccount(this RegisterRequestDto request) {
			return new Account {
				Username = request.Username,
				PasswordHash = PasswordHashHelper.CreatePasswordHash(request.Password),
				Role = request.Role,
				IsActive = request.IsActive,
				SecurityTimestamp = DateTime.UtcNow
			};
		}
	}
}
