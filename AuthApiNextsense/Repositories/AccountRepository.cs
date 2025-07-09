using AuthApiNextsense.Data;
using AuthApiNextsense.Models;
using AuthApiNextsense.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthApiNextsense.Repositories {
	public class AccountRepository : IAccountRepository {
		private readonly DataContext _context;

		/// <summary>
		/// Initializes a new instance of the AccountRepository class.
		/// </summary>
		/// <param name="context">The database context to be used for data operations.</param>
		public AccountRepository(DataContext context) {
			_context = context;
		}

		/// <summary>
		/// Adds a new account to the data store and saves changes.
		/// </summary>
		/// <param name="account">The account to add.</param>
		public async Task AddAsync(Account account) {
			await _context.Accounts.AddAsync(account);
			await _context.SaveChangesAsync();
		}

		/// <summary>
		/// Deletes an account from the data store by its ID and saves changes.
		/// </summary>
		/// <param name="id">The ID of the account to delete.</param>
		public async Task DeleteAsync(int id) {
			var accountToDelete = await _context.Accounts.FindAsync(id);
			if (accountToDelete != null) {
				_context.Accounts.Remove(accountToDelete);
				await _context.SaveChangesAsync();
			}
		}

		/// <summary>
		/// Retrieves all accounts from the data store.
		/// </summary>
		/// <returns>A collection of all accounts.</returns>
		public async Task<IEnumerable<Account>> GetAllAsync() {
			return await _context.Accounts.ToListAsync();
		}

		/// <summary>
		/// Retrieves an account by its unique identifier.
		/// </summary>
		/// <param name="id">The ID of the account.</param>
		/// <returns>The account if found; otherwise, null.</returns>
		public async Task<Account> GetByIdAsync(int id) {
			return await _context.Accounts.FindAsync(id);
		}

		/// <summary>
		/// Retrieves an account by its username.
		/// </summary>
		/// <param name="username">The username of the account.</param>
		/// <returns>The account if found; otherwise, null.</returns>
		public async Task<Account> GetByUsernameAsync(string username) {
			return await _context.Accounts.SingleOrDefaultAsync(a => a.Username == username);
		}

		/// <summary>
		/// Updates an existing account in the data store and saves changes.
		/// </summary>
		/// <param name="account">The account with updated values.</param>
		public async Task UpdateAsync(Account account) {
			_context.Entry(account).State = EntityState.Modified;
			await _context.SaveChangesAsync();
		}

		/// <summary>
		/// Checks if an account with the specified username already exists.
		/// </summary>
		/// <param name="username">The username to check.</param>
		/// <returns>True if the username exists; otherwise, false.</returns>
		public async Task<bool> UsernameExistsAsync(string username) {
			return await _context.Accounts.AnyAsync(a => a.Username == username);
		}
	}
}
