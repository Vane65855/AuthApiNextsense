namespace AuthApiNextsense.Helpers {
	public static class PasswordHashHelper {
		/// <summary>
		/// Creates a hash from a password using BCrypt.
		/// </summary>
		/// <param name="password">The plain-text password.</param>
		/// <returns>The generated password hash.</returns>
		public static string CreatePasswordHash(string password) {
			return BCrypt.Net.BCrypt.HashPassword(password);
		}

		/// <summary>
		/// Verifies that a password matches a stored hash.
		/// </summary>
		/// <param name="password">The plain-text password to verify.</param>
		/// <param name="storedHash">The stored password hash from the database.</param>
		/// <returns>True if the password is valid, otherwise false.</returns>
		public static bool VerifyPasswordHash(string password, string storedHash) {
			return BCrypt.Net.BCrypt.Verify(password, storedHash);
		}
	}
}
