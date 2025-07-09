using Blazored.LocalStorage;
using System.Net.Http.Headers;

namespace AuthFrontEndNextsense.Services {
	public class HttpInterceptorService : DelegatingHandler {
		private readonly ILocalStorageService _localStorage;

		public HttpInterceptorService(ILocalStorageService localStorage) {
			_localStorage = localStorage;
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
			// Get the token from local storage
			var token = await _localStorage.GetItemAsStringAsync("authToken", cancellationToken);

			// If the token exists, add it to the Authorization header
			if (!string.IsNullOrEmpty(token)) {
				request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
			}

			// Continue the request pipeline
			return await base.SendAsync(request, cancellationToken);
		}
	}
}
