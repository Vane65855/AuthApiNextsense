using AuthApiNextsense.Models;
using AuthApiNextsense.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthApiNextsense.Services {
	public class TokenService : ITokenService {
		private readonly IConfiguration _config;

		public TokenService(IConfiguration config) {
			_config = config;
		}

		public string GenerateJwtToken(Account account) {
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]));
			var claims = new List<Claim> {
				new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
				new Claim(ClaimTypes.Name, account.Username),
				new Claim(ClaimTypes.Role, account.Role.ToString()),
				new Claim("security_timestamp", account.SecurityTimestamp.ToString("O"))
			};
			var tokenDescriptor = new SecurityTokenDescriptor {
				Subject = new ClaimsIdentity(claims),
				Issuer = _config["Jwt:Issuer"],
				Audience = _config["Jwt:Audience"],
				Expires = DateTime.UtcNow.AddDays(7),
				SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}
	}
}
