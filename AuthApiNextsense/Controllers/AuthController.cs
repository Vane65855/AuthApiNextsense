using AuthApiNextsense.DTOs;
using AuthApiNextsense.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;

namespace AuthApiNextsense.Controllers {
	[ApiController]
	[Route("api/[controller]")]
	[AllowAnonymous]
	public class AuthController : ControllerBase {
		private readonly IAuthService _authService;

		public AuthController(IAuthService authService) {
			_authService = authService;
		}

		/// <summary>
		/// Authenticates a user and returns a JWT.
		/// </summary>
		/// <param name="loginRequest">The user's login credentials.</param>
		/// <returns>A JWT string if authentication is successful.</returns>
		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest) {
			try {
				var token = await _authService.LoginAsync(loginRequest);
				return Ok(new { Token = token });
			}
			catch (InvalidCredentialException ex) {
				// Return a 401 Unauthorized response for bad credentials
				return Unauthorized(new { message = ex.Message });
			}
			catch (Exception ex) {
				// Return a 500 Internal Server Error for any other issues
				return StatusCode(500, new { message = "An unexpected error occurred." });
			}
		}
	}
}
