using AuthApiNextsense.DTOs;
using AuthApiNextsense.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthApiNextsense.Controllers {
	[ApiController]
	[Route("api/[controller]")]
	[Authorize(Roles = "Admin")]
	public class AdminController : ControllerBase {
		private readonly IAdminService _adminService;

		public AdminController(IAdminService adminService) {
			_adminService = adminService;
		}

		/// <summary>
		/// Registers a new user account.
		/// </summary>
		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequest) {
			try {
				await _adminService.RegisterAsync(registerRequest);
				return Ok(new { message = "Account registered successfully." });
			}
			catch (ArgumentException ex) {
				return BadRequest(new { message = ex.Message });
			}
		}

		/// <summary>
		/// Retrieves a list of all user accounts.
		/// </summary>
		[HttpGet("accounts")]
		public async Task<ActionResult<IEnumerable<AccountDto>>> GetAllAccounts() {
			var accounts = await _adminService.GetAllAccountsAsync();
			return Ok(accounts);
		}

		/// <summary>
		/// Retrieves a user account.
		/// </summary>
		[HttpGet("accounts/{id}")]
		public async Task<ActionResult<IEnumerable<AccountDto>>> GetAllAccounts(int id) {
			try {
				var accounts = await _adminService.GetAccountAsync(id);
				return Ok(accounts);
			}
			catch (Exception ex) {

				if (ex is KeyNotFoundException) return NotFound(new { message = ex.Message });
				return StatusCode(500, new { message = "An error occurred while retrieving the account." });
			}
		}

		/// <summary>
		/// Edits an existing user account.
		/// </summary>
		[HttpPut("accounts/{id}")]
		public async Task<IActionResult> EditAccount(int id, [FromBody] AccountDto editRequest) {
			if (id != editRequest.Id) {
				return BadRequest("Account ID mismatch.");
			}

			try {
				await _adminService.EditAccountAsync(editRequest);
				return NoContent();
			}
			catch (KeyNotFoundException ex) {
				return NotFound(new { message = ex.Message });
			}
		}

		/// <summary>
		/// Deactivates a user account and invalidates their tokens.
		/// </summary>
		[HttpPost("accounts/{id}/deactivate")]
		public async Task<IActionResult> DeactivateAccount(int id) {
			await _adminService.DeactivateAccountAsync(id);
			return Ok(new { message = "Account deactivated successfully." });
		}

		/// <summary>
		/// Invalidates all existing tokens for a user, forcing them to log in again.
		/// </summary>
		[HttpPost("accounts/{id}/force-logout")]
		public async Task<IActionResult> ForceLogout(int id) {
			await _adminService.ForceLogoutAsync(id);
			return Ok(new { message = "User logout forced successfully." });
		}

		/// <summary>
        /// Deletes a user account permanently.
        /// </summary>
        [HttpDelete("accounts/{id}")]
		public async Task<IActionResult> DeleteAccount(int id) {
			await _adminService.DeleteAccountAsync(id);
			return NoContent();
		}
	}
}
