using AuthFrontEndNextsense.DTOs;

namespace AuthFrontEndNextsense.Services.Interfaces {
	public interface IAdminDataService {
		Task<IEnumerable<AccountDto>> GetAllAccountsAsync();
		Task<AccountDto> GetAccountByIdAsync(int accountId);
		Task RegisterAccountAsync(RegisterRequestDto registerRequest);
		Task EditAccountAsync(int accountId, AccountDto account);
		Task DeactivateAccountAsync(int accountId);
		Task ForceLogoutAsync(int accountId);
		Task DeleteAccountAsync(int accountId);
	}
}
