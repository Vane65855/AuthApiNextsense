using AuthApiNextsense.DTOs;

namespace AuthApiNextsense.Services.Interfaces {
	public interface IAdminService {
		Task RegisterAsync(RegisterRequestDto registerRequest);
		Task<IEnumerable<AccountDto>> GetAllAccountsAsync();
		Task<AccountDto> GetAccountAsync(int id);
		Task EditAccountAsync(AccountDto editRequest);
		Task DeactivateAccountAsync(int accountId);
		Task ForceLogoutAsync(int accountId);
		Task DeleteAccountAsync(int accountId);
	}
}
