using BookwormsMembership.Model;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


builder.Services.AddDistributedMemoryCache(); //save session in memory
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromSeconds(30);
	options.Cookie.HttpOnly = true;
	
});

//builder.Services.AddCookiePolicy(options => {
//	options.OnAppendCookie = ctx =>
//	{
//		ctx.CookieOptions.MaxAge = TimeSpan.FromSeconds(30);
//	};
//});

builder.Services.AddDataProtection();
builder.Services.AddDbContext<AuthDbContext>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AuthDbContext>().AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options => {
	// Password settings.
	options.Password.RequireDigit = true;
	options.Password.RequireLowercase = true;
	options.Password.RequireNonAlphanumeric = true;
	options.Password.RequireUppercase = true;
	options.Password.RequiredLength = 12;
	options.Password.RequiredUniqueChars = 1;
	// Lockout settings.
	options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); //Account recovery after x mins of lockout
	options.Lockout.MaxFailedAccessAttempts = 3; //Rate Limiting
	options.Lockout.AllowedForNewUsers = true;
	// User settings.
	options.User.AllowedUserNameCharacters =
	"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
	options.User.RequireUniqueEmail = true; //Check for duplicate email

	options.SignIn.RequireConfirmedEmail = true;

	

});
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
	options.TokenLifespan = TimeSpan.FromMinutes(5);
});


builder.Services.AddAuthentication("MyCookieAuth").AddCookie("MyCookieAuth", options =>
{
	options.Cookie.Name = "MyCookieAuth";
	options.AccessDeniedPath = "/Account/AccessDenied";

	
	options.ExpireTimeSpan = TimeSpan.FromSeconds(30);


});

builder.Services.ConfigureApplicationCookie(Config =>
{
	Config.LoginPath = "/Login";
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();

app.UseStatusCodePagesWithRedirects("/errors/{0}");

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
