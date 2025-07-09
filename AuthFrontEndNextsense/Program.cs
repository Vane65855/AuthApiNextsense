using AuthFrontEndNextsense;
using AuthFrontEndNextsense.Services;
using AuthFrontEndNextsense.Services.Interfaces;
using AuthFrontEndNextsense.State;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddTransient<HttpInterceptorService>();
builder.Services.AddHttpClient("Api", client => {
	client.BaseAddress = new Uri("https://localhost:5005");
})
.AddHttpMessageHandler<HttpInterceptorService>();
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api"));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAdminDataService, AdminDataService>();
await builder.Build().RunAsync();
