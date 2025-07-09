using AuthApiNextsense.Helpers;
using NUnit.Framework;

namespace AuthApiNextsenseTest;

public class PasswordHashHelperTests
{
	[TestCase("my-secret-password-123", true, TestName = "Correct password")]
	[TestCase("wrong-password", false, TestName = "Incorrect password")]
	[TestCase("", false, TestName = "Empty password")] 
	public void VerifyPasswordHash_WithVariousInputs_ReturnsExpectedResult(string passwordToTest, bool expectedResult) {
		// Arrange
		const string correctPassword = "my-secret-password-123";
		string passwordHash = PasswordHashHelper.CreatePasswordHash(correctPassword);

		// Act
		bool isVerified = PasswordHashHelper.VerifyPasswordHash(passwordToTest, passwordHash);

		// Assert
		Assert.That(isVerified, Is.EqualTo(expectedResult));
	}

	[Test]
	public void CreatePasswordHash_ValidPassword_ReturnsNonNullOrEmptyString() {
		// Arrange
		var password = "a-valid-password";

		// Act
		string passwordHash = PasswordHashHelper.CreatePasswordHash(password);

		// Assert
		Assert.That(passwordHash, Is.Not.Null.And.Not.Empty);
	}
}
