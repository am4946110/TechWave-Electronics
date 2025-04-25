using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TechWave_Electronics.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
    .AddRazorOptions(options =>
    {
        options.ViewLocationFormats.Add("/Views/TblProducts/Partials/story.cshtml");
    });

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<TechWaveElectronics>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("First")));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Last")));
builder.Services.AddScoped<ICustomKeyManager, KeyManager>();


builder.Services.AddIdentity<MyUser, MyRole>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();


builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Accounts/Login";
    options.AccessDeniedPath = "/Accounts/AccessDenied";
    options.Events.OnRedirectToLogin = context =>
    {
        var separator = context.RedirectUri.Contains("?") ? "&" : "?";
        var messageParam = "message=" + Uri.EscapeDataString("Your account has been modified, please log in again.");
        context.Response.Redirect(context.RedirectUri + separator + messageParam);
        return Task.CompletedTask;
    };
});



builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.Zero;
    options.TimeProvider = TimeProvider.System;
});

builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".AdventureWorks.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Antiforgery
builder.Services.AddAntiforgery(options =>
{
    options.FormFieldName = "AntiforgeryFieldname";
    options.HeaderName = "X-CSRF-TOKEN-HEADERNAME";
    options.SuppressXFrameOptionsHeader = false;
});

// Add SignalR, Razor Pages, Logging
builder.Services.AddSignalR();
builder.Services.AddRazorPages();
builder.Services.AddLogging();

builder.Services.AddLogging();
builder.Services.AddDataProtection();
builder.Services.AddSingleton<OnlineUsersService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseStaticFiles();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();
app.UseMiddleware<OnlineUsersMiddleware>();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
