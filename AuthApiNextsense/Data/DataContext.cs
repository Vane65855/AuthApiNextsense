using AuthApiNextsense.Enums;
using AuthApiNextsense.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthApiNextsense.Data {
	public class DataContext : DbContext {
		public DataContext(DbContextOptions<DataContext> options) : base(options) { }
		public DbSet<Account> Accounts { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			base.OnModelCreating(modelBuilder);
			// Seed the initial Admin account
			modelBuilder.Entity<Account>().HasData(
				new Account {
					Id = 1,
					Username = "admin",
					//Password is "admin" hashed with BCrypt. Remember for login.
					PasswordHash = "$2a$11$l6yWKq2ZCwfi7VQIeZoAVeQJfG3nKoBPw0rxy/JAz2X4o6FdiBeTG",
					Role = AccountRole.Admin,
					IsActive = true,
					SecurityTimestamp = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
				}
			);
		}
	}
}
