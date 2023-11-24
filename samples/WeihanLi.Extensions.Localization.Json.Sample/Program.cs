using System.Globalization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using WeihanLi.Extensions.Localization.Json;

var builder = WebApplication.CreateSlimBuilder(args);
var services = builder.Services;

var supportedCultures = new[]
{
    new CultureInfo("zh"),
    new CultureInfo("en"),
};
services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("zh");
    // Formatting numbers, dates, etc.
    options.SupportedCultures = supportedCultures;
    // UI strings that we have localized.
    options.SupportedUICultures = supportedCultures;
});
var resourcesPath = builder.Configuration.GetAppSetting("ResourcesPath") ?? "Resources";
services.AddJsonLocalization(options =>
{
    options.ResourcesPath = resourcesPath;
    // options.ResourcesPathType = ResourcesPathType.TypeBased;
    options.ResourcesPathType = ResourcesPathType.CultureBased;                    
});
services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

services.AddControllersWithViews()
    .AddMvcLocalization(options =>
    {
        options.ResourcesPath = resourcesPath;
    }, LanguageViewLocationExpanderFormat.Suffix)
    ;

var app = builder.Build();

app.UseRequestLocalization();
app.UseStaticFiles();

app.MapControllers();
app.MapDefaultControllerRoute();

await app.RunAsync();
