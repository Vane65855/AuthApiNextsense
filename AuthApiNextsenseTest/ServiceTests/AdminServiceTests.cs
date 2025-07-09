using AuthApiNextsense.DTOs;
using AuthApiNextsense.Enums;
using AuthApiNextsense.Models;
using AuthApiNextsense.Repositories.Interfaces;
using AuthApiNextsense.Services;
using Moq;

namespace AuthApiNextsenseTest;

public class AdminServiceTests
{
    private Mock<IAccountRepository> _accountRepositoryMock;
    private AdminService _adminService;

    [SetUp]
    public void Setup()
    {
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _adminService = new AdminService(_accountRepositoryMock.Object);
    }

    [Test]
    public async Task RegisterAsync_UsernameExists_ThrowsArgumentException()
    {
        // Arrange
        var registerRequest = new RegisterRequestDto
        {
            Username = "existing",
            Password = "pass",
            IsActive = true,
            Role = AccountRole.User
        };
        _accountRepositoryMock.Setup(r => r.UsernameExistsAsync("existing")).ReturnsAsync(true);

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(() => _adminService.RegisterAsync(registerRequest));
        Assert.That(ex.Message, Is.EqualTo("Username is already taken."));
    }

    [Test]
    public async Task RegisterAsync_ValidRequest_AddsAccount()
    {
        // Arrange
        var registerRequest = new RegisterRequestDto
        {
            Username = "newuser",
            Password = "pass",
            IsActive = true,
            Role = AccountRole.User
        };
        _accountRepositoryMock.Setup(r => r.UsernameExistsAsync("newuser")).ReturnsAsync(false);

        // Act
        await _adminService.RegisterAsync(registerRequest);

        // Assert
        _accountRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Account>()), Times.Once);
    }

    [Test]
    public async Task GetAllAccountsAsync_ReturnsMappedDtos()
    {
        // Arrange
        var accounts = new List<Account>
        {
            new Account { Id = 1, Username = "a", IsActive = true, Role = AccountRole.User },
            new Account { Id = 2, Username = "b", IsActive = false, Role = AccountRole.Admin }
        };
        _accountRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(accounts);

        // Act
        var result = await _adminService.GetAllAccountsAsync();

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.Any(dto => dto.Username == "a"));
        Assert.That(result.Any(dto => dto.Username == "b"));
    }

    [Test]
    public void EditAccountAsync_AccountNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var dto = new AccountDto { Id = 99, Username = "x", IsActive = true, Role = AccountRole.User };
        _accountRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Account)null);

        // Act & Assert
        Assert.ThrowsAsync<KeyNotFoundException>(() => _adminService.EditAccountAsync(dto));
    }

    [Test]
    public async Task EditAccountAsync_ValidRequest_UpdatesAccount()
    {
        // Arrange
        var dto = new AccountDto { Id = 1, Username = "edit", IsActive = false, Role = AccountRole.Admin };
        var account = new Account { Id = 1, Username = "old", IsActive = true, Role = AccountRole.User };
        _accountRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(account);

        // Act
        await _adminService.EditAccountAsync(dto);

        // Assert
        _accountRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Account>(a =>
            a.Id == 1 && a.Username == "edit" && a.IsActive == false && a.Role == AccountRole.Admin)), Times.Once);
    }

    [Test]
    public async Task DeactivateAccountAsync_AccountExists_UpdatesIsActive()
    {
        // Arrange
        var account = new Account { Id = 1, IsActive = true };
        _accountRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(account);

        // Act
        await _adminService.DeactivateAccountAsync(1);

        // Assert
        Assert.That(account.IsActive, Is.False);
        _accountRepositoryMock.Verify(r => r.UpdateAsync(account), Times.Once);
    }

    [Test]
    public async Task DeactivateAccountAsync_AccountNotFound_DoesNothing()
    {
        // Arrange
        _accountRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync((Account)null);

        // Act
        await _adminService.DeactivateAccountAsync(2);

        // Assert
        _accountRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Account>()), Times.Never);
    }

    [Test]
    public async Task ForceLogoutAsync_AccountExists_UpdatesSecurityTimestamp()
    {
        // Arrange
        var account = new Account { Id = 1, SecurityTimestamp = DateTime.MinValue };
        _accountRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(account);

        // Act
        await _adminService.ForceLogoutAsync(1);

        // Assert
        Assert.That(account.SecurityTimestamp, Is.Not.EqualTo(DateTime.MinValue));
        _accountRepositoryMock.Verify(r => r.UpdateAsync(account), Times.Once);
    }

    [Test]
    public async Task DeleteAccountAsync_CallsRepositoryDelete()
    {
        // Arrange

        // Act
        await _adminService.DeleteAccountAsync(5);

        // Assert
        _accountRepositoryMock.Verify(r => r.DeleteAsync(5), Times.Once);
    }
}
