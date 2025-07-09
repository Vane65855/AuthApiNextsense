using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace AuthFrontEndNextsense.State {
	public class CustomAuthStateProvider : AuthenticationStateProvider {
		private readonly ILocalStorageService _localStorage;
		private readonly HttpClient _httpClient;

		public CustomAuthStateProvider(ILocalStorageService localStorage, HttpClient httpClient) {
			_localStorage = localStorage;
			_httpClient = httpClient;
		}

		public override async Task<AuthenticationState> GetAuthenticationStateAsync() {
			var authToken = await _localStorage.GetItemAsStringAsync("authToken");
			var identity = new ClaimsIdentity();
			_httpClient.DefaultRequestHeaders.Authorization = null;

			if (!string.IsNullOrEmpty(authToken)) {
				try {
					var claims = ParseClaimsFromJwt(authToken);
					identity = new ClaimsIdentity(claims, "jwt", nameType: "unique_name", roleType: "role");
					_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
				}
				catch {
					await _localStorage.RemoveItemAsync("authToken");
					identity = new ClaimsIdentity();
				}
			}

			var user = new ClaimsPrincipal(identity);
			return new AuthenticationState(user);
		}

		public async Task NotifyUserAuthentication(string token) {
			var claims = ParseClaimsFromJwt(token);
			var identity = new ClaimsIdentity(claims, "jwt", nameType: "unique_name", roleType: "role");
			var user = new ClaimsPrincipal(identity);
			var authState = Task.FromResult(new AuthenticationState(user));

			await _localStorage.SetItemAsStringAsync("authToken", token);
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

			NotifyAuthenticationStateChanged(authState);
		}

		public async Task NotifyUserLogout() {
			var identity = new ClaimsIdentity();
			var user = new ClaimsPrincipal(identity);
			var authState = Task.FromResult(new AuthenticationState(user));

			await _localStorage.RemoveItemAsync("authToken");
			_httpClient.DefaultRequestHeaders.Authorization = null;

			NotifyAuthenticationStateChanged(authState);
		}

		public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt) {
			var payload = jwt.Split('.')[1];
			var jsonBytes = ParseBase64WithoutPadding(payload);
			var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
			var claims = keyValuePairs.Select(kvp => {
				if (kvp.Key == "role" && kvp.Value is JsonElement { ValueKind: JsonValueKind.Array } rolesElement) {
					return rolesElement.EnumerateArray().Select(role => new Claim("role", role.ToString()));
				}
				return new[] { new Claim(kvp.Key, kvp.Value.ToString()) };
			}).SelectMany(c => c);

			return claims;
		}

		private static byte[] ParseBase64WithoutPadding(string base64) {
			switch (base64.Length % 4) {
				case 2: base64 += "=="; break;
				case 3: base64 += "="; break;
			}
			return Convert.FromBase64String(base64);
		}
	}
}