global using Twilio;
global using Twilio.Exceptions;
global using Twilio.Rest.Verify.V2.Service;
global using Microsoft.Extensions.Caching.Memory;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddAzureKeyVault(
    new Uri("https://cm-identity-kv.vault.azure.net"),
       new ChainedTokenCredential(
            new AzureCliCredential(),
            new ManagedIdentityCredential()
));      

// Add services to the container.
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAdB2C"))
    .EnableTokenAcquisitionToCallDownstreamApi(new string[] {builder.Configuration["scope"]})
    .AddInMemoryTokenCaches();

builder.Services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, x =>
{
    x.ResponseType = "code";
});

builder.Services.AddScoped<IVerification, Verification>();
builder.Services.AddScoped<VerifyFilter>();
builder.Services.AddMemoryCache();

builder.Services.AddRazorPages().AddMicrosoftIdentityUI();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapGet("api/verify", async (IVerification verificationService, HttpContext context) =>
{
    var phoneNumber = context.User.Claims.First(x => x.Type.Contains("PhoneNumber")).Value;
    return await verificationService.StartVerificationAsync(phoneNumber, "sms");
}).RequireAuthorization();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
