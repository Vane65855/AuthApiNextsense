using AuthApiNextsense.Data;
using AuthApiNextsense.Repositories;
using AuthApiNextsense.Repositories.Interfaces;
using AuthApiNextsense.Services;
using AuthApiNextsense.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;


builder.Services.AddDbContext<DataContext>(options => options.UseSqlite(config.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddAuthentication(options => {
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => {
	options.TokenValidationParameters = new TokenValidationParameters {
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = config["Jwt:Issuer"],
		ValidAudience = config["Jwt:Audience"],
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Secret"]))
	};
	options.Events = new JwtBearerEvents {
		OnTokenValidated = async context => {
			var accountRepo = context.HttpContext.RequestServices.GetRequiredService<IAccountRepository>();
			var userId = int.Parse(context.Principal.FindFirstValue(ClaimTypes.NameIdentifier));
			var account = await accountRepo.GetByIdAsync(userId);
			// Get the security timestamp from the token
			var tokenTimestamp = context.Principal.FindFirstValue("security_timestamp");
			// Compare the token's timestamp with the one in the database
			if (account == null || account.SecurityTimestamp.ToString("O") != tokenTimestamp) {
				// If they don't match, the token is old. Invalidate it.
				context.Fail("This token has been invalidated.");
			}
		}
	};
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
	// Define the security scheme (JWT)
	options.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme {
		Type = SecuritySchemeType.Http,
		Scheme = "bearer",
		BearerFormat = "JWT",
		Description = "Input your JWT token to access this API"
	});

	// Make Swagger UI apply the security scheme to all endpoints
	options.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "bearerAuth" }
			},
			new string[] {}
		}
	});
});
builder.Services.AddCors(options => {
	options.AddPolicy("AllowAll", builder => {
		builder.AllowAnyOrigin()
			   .AllowAnyMethod()
			   .AllowAnyHeader();
	});
});

var app = builder.Build();
app.UseCors("AllowAll");
app.UseSwagger();
app.UseSwaggerUI(c => {
	c.SwaggerEndpoint("/swagger/v1/swagger.json", "Nextsense");
	c.RoutePrefix = string.Empty;
});
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
