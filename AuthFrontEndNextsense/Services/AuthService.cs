using AuthFrontEndNextsense.DTOs;
using AuthFrontEndNextsense.Services.Interfaces;
using AuthFrontEndNextsense.State;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using System.Text.Json;

namespace AuthFrontEndNextsense.Services {
	public class AuthService : IAuthService{
		private readonly HttpClient _httpClient;
		private readonly ILocalStorageService _localStorage;
		private readonly AuthenticationStateProvider _authenticationStateProvider;
		private readonly NavigationManager _navigationManager;

		public AuthService(HttpClient httpClient, ILocalStorageService localStorage, AuthenticationStateProvider authenticationStateProvider, NavigationManager navigationManager) {
			_httpClient = httpClient;
			_localStorage = localStorage;
			_authenticationStateProvider = authenticationStateProvider;
			_navigationManager = navigationManager;
		}

		public async Task LoginAsync(LoginRequestDto loginRequest) {
			// Send the login request to the backend API
			var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginRequest);

			// If the response is not successful, an exception will be thrown by the HttpClient
			// which should be caught by the calling component.
			response.EnsureSuccessStatusCode();

			// Deserialize the response to get the token
			var responseString = await response.Content.ReadAsStringAsync();
			var loginResult = JsonSerializer.Deserialize<LoginResult>(responseString,
				new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

			// Save the token to local storage
			await _localStorage.SetItemAsStringAsync("authToken", loginResult.Token);

			var claims = CustomAuthStateProvider.ParseClaimsFromJwt(loginResult.Token);
			var username = claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;
			await _localStorage.SetItemAsync("username", username);

			// Notify the application that the authentication state has changed
			await ((CustomAuthStateProvider)_authenticationStateProvider).NotifyUserAuthentication(loginResult.Token);

			// Redirect the user to the hub page
			_navigationManager.NavigateTo("/hub");
		}

		public async Task LogoutAsync() {
			// Remove the token from local storage
			await _localStorage.RemoveItemAsync("authToken");
			await _localStorage.RemoveItemAsync("username");

			// Notify the application that the authentication state has changed
			await ((CustomAuthStateProvider)_authenticationStateProvider).NotifyUserLogout();

			// Redirect the user to the login page
			_navigationManager.NavigateTo("/login");
		}

		// Helper class to deserialize the login response
		private class LoginResult {
			public string Token { get; set; }
		}
	}
}
