using JITC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);




var builderConfig = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();


IConfiguration config = builderConfig.Build();
String connextionString = config.GetConnectionString("default");


builder.Services.AddDefaultIdentity<User>(option => {
    option.SignIn.RequireConfirmedAccount = false;
    option.Password.RequiredLength =  5;
    option.Password.RequireNonAlphanumeric = false; 
    option.Password.RequireUppercase = false;
}).AddDefaultUI()
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<JITCSDbContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<JITCSDbContext>( options => options.UseSqlServer(connextionString));








builder.Services.AddRazorPages();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var _userManager = scope.ServiceProvider.GetService<UserManager<User>>();
    var _roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
    var _context = scope.ServiceProvider.GetRequiredService<JITCSDbContext>();
    Seed.Start(_roleManager, _userManager, _context);

}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();


app.MapControllerRoute(name: "default",pattern: "{controller=Home}/{action=Index}/{id?}");
app.UseEndpoints(endpoints => endpoints.MapRazorPages());
app.Run();
