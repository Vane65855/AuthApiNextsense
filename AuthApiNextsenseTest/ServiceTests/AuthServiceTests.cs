using System.Security.Authentication;
using AuthApiNextsense.DTOs;
using AuthApiNextsense.Models;
using AuthApiNextsense.Repositories.Interfaces;
using AuthApiNextsense.Services;
using AuthApiNextsense.Services.Interfaces;
using Moq;

namespace AuthApiNextsenseTest;

public class AuthServiceTests
{
    private Mock<IAccountRepository> _accountRepositoryMock;
    private Mock<ITokenService> _tokenServiceMock;
    private AuthService _authService;

    [SetUp]
    public void Setup()
    {
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _tokenServiceMock = new Mock<ITokenService>();
        _authService = new AuthService(_accountRepositoryMock.Object, _tokenServiceMock.Object);
    }

    [Test]
    public async Task LoginAsync_ValidCredentials_ReturnsToken()
    {
        // Arrange
        var loginRequest = new LoginRequestDto { Username = "user", Password = "password" };
        var account = new Account
        {
            Id = 1,
            Username = "user",
            PasswordHash = AuthApiNextsense.Helpers.PasswordHashHelper.CreatePasswordHash("password"),
            IsActive = true
        };
        _accountRepositoryMock.Setup(r => r.GetByUsernameAsync("user")).ReturnsAsync(account);
        _tokenServiceMock.Setup(t => t.GenerateJwtToken(account)).Returns("jwt_token");

        // Act
        var token = await _authService.LoginAsync(loginRequest);

		// Assert
		Assert.That(token, Is.EqualTo("jwt_token"));
	}

	[Test]
    public void LoginAsync_InvalidPassword_ThrowsInvalidCredentialException()
    {
        // Arrange
        var loginRequest = new LoginRequestDto { Username = "user", Password = "wrongpassword" };
        var account = new Account
        {
            Id = 1,
            Username = "user",
            PasswordHash = AuthApiNextsense.Helpers.PasswordHashHelper.CreatePasswordHash("password"),
            IsActive = true
        };
        _accountRepositoryMock.Setup(r => r.GetByUsernameAsync("user")).ReturnsAsync(account);

        // Act & Assert
        Assert.ThrowsAsync<InvalidCredentialException>(() => _authService.LoginAsync(loginRequest));
    }

    [Test]
    public void LoginAsync_NonExistentUser_ThrowsInvalidCredentialException()
    {
        // Arrange
        var loginRequest = new LoginRequestDto { Username = "nouser", Password = "password" };
        _accountRepositoryMock.Setup(r => r.GetByUsernameAsync("nouser")).ReturnsAsync((Account)null);

        // Act & Assert
        Assert.ThrowsAsync<InvalidCredentialException>(() => _authService.LoginAsync(loginRequest));
    }

    [Test]
    public void LoginAsync_InactiveAccount_ThrowsInvalidCredentialException()
    {
        // Arrange
        var loginRequest = new LoginRequestDto { Username = "user", Password = "password" };
        var account = new Account
        {
            Id = 1,
            Username = "user",
            PasswordHash = AuthApiNextsense.Helpers.PasswordHashHelper.CreatePasswordHash("password"),
            IsActive = false
        };
        _accountRepositoryMock.Setup(r => r.GetByUsernameAsync("user")).ReturnsAsync(account);

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidCredentialException>(() => _authService.LoginAsync(loginRequest));
        Assert.That(ex.Message, Is.EqualTo("Your account has been deactivated."));
    }
}
