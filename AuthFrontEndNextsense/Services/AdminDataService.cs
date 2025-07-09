using AuthFrontEndNextsense.DTOs;
using AuthFrontEndNextsense.Services.Interfaces;
using System.Net.Http.Json;

namespace AuthFrontEndNextsense.Services {
	public class AdminDataService : IAdminDataService {
		private readonly HttpClient _httpClient;

		public AdminDataService(HttpClient httpClient) {
			_httpClient = httpClient;
		}

		public async Task<IEnumerable<AccountDto>> GetAllAccountsAsync() {
			return await _httpClient.GetFromJsonAsync<IEnumerable<AccountDto>>("api/admin/accounts");
		}

		public async Task<AccountDto> GetAccountByIdAsync(int accountId) {
			return await _httpClient.GetFromJsonAsync<AccountDto>($"api/admin/accounts/{accountId}");
		}

		public async Task RegisterAccountAsync(RegisterRequestDto registerRequest) {
			var response = await _httpClient.PostAsJsonAsync("api/admin/register", registerRequest);
			response.EnsureSuccessStatusCode();
		}

		public async Task EditAccountAsync(int accountId, AccountDto account) {
			var response = await _httpClient.PutAsJsonAsync($"api/admin/accounts/{accountId}", account);
			response.EnsureSuccessStatusCode();
		}

		public async Task DeactivateAccountAsync(int accountId) {
			var response = await _httpClient.PostAsync($"api/admin/accounts/{accountId}/deactivate", null);
			response.EnsureSuccessStatusCode();
		}

		public async Task ForceLogoutAsync(int accountId) {
			var response = await _httpClient.PostAsync($"api/admin/accounts/{accountId}/force-logout", null);
			response.EnsureSuccessStatusCode();
		}

		public async Task DeleteAccountAsync(int accountId) {
			var response = await _httpClient.DeleteAsync($"api/admin/accounts/{accountId}");
			response.EnsureSuccessStatusCode();
		}
	}
}
