using AuthApiNextsense.Models;

namespace AuthApiNextsense.Repositories.Interfaces {
	public interface IAccountRepository {
		Task<Account> GetByIdAsync(int id);
		Task<Account> GetByUsernameAsync(string username);
		Task<IEnumerable<Account>> GetAllAsync();
		Task AddAsync(Account account);
		Task UpdateAsync(Account account);
		Task DeleteAsync(int id);
		Task<bool> UsernameExistsAsync(string username);
	}
}
