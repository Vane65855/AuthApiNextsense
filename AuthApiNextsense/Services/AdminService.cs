using AuthApiNextsense.DTOs;
using AuthApiNextsense.Extensions;
using AuthApiNextsense.Helpers;
using AuthApiNextsense.Models;
using AuthApiNextsense.Repositories.Interfaces;
using AuthApiNextsense.Services.Interfaces;

namespace AuthApiNextsense.Services {
	public class AdminService : IAdminService {
		private readonly IAccountRepository _accountRepository;

		public AdminService(IAccountRepository accountRepository) {
			_accountRepository = accountRepository;
		}

		public async Task RegisterAsync(RegisterRequestDto registerRequest) {
			if (await _accountRepository.UsernameExistsAsync(registerRequest.Username)) {
				throw new ArgumentException("Username is already taken.");
			}
			var account = registerRequest.ToNewAccount();
			await _accountRepository.AddAsync(account);
		}

		public async Task<IEnumerable<AccountDto>> GetAllAccountsAsync() {
			var accounts = await _accountRepository.GetAllAsync();
			// Map entities to DTOs to avoid exposing sensitive data
			return accounts.Select(account => account.ToDto());
		}

		public async Task<AccountDto> GetAccountAsync(int id) {
			var accounts = await _accountRepository.GetAllAsync();
			// Map entities to DTOs to avoid exposing sensitive data
			return accounts.FirstOrDefault(account => account.Id == id)?.ToDto() ?? throw new KeyNotFoundException("No account found.");
		}

		public async Task EditAccountAsync(AccountDto editRequest) {
			var account = await _accountRepository.GetByIdAsync(editRequest.Id);
			if (account == null) {
				throw new KeyNotFoundException("Account not found.");
			}
			// Update properties from the DTO
			account.Username = editRequest.Username;
			account.Role = editRequest.Role;
			account.IsActive = editRequest.IsActive;
			await _accountRepository.UpdateAsync(account);
		}

		public async Task DeactivateAccountAsync(int accountId) {
			var account = await _accountRepository.GetByIdAsync(accountId);
			if (account != null) {
				account.IsActive = false;
				account.SecurityTimestamp = DateTime.UtcNow;
				await _accountRepository.UpdateAsync(account);
			}
		}

		public async Task ForceLogoutAsync(int accountId) {
			var account = await _accountRepository.GetByIdAsync(accountId);
			if (account != null) {
				// Updating the timestamp invalidates all previously issued JWTs
				account.SecurityTimestamp = DateTime.UtcNow;
				await _accountRepository.UpdateAsync(account);
			}
		}

		public async Task DeleteAccountAsync(int accountId) {
			await _accountRepository.DeleteAsync(accountId);
		}
	}
}
