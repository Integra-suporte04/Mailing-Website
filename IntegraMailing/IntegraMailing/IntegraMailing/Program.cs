using Microsoft.AspNetCore.Identity;
//using IntegraMailing.Data;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

//builder.Services.AddIdentity<IdentityUser, IdentityRole>() // Configura o Identity
   // .AddEntityFrameworkStores<ApplicationDbContext>() // Usa o DbContext para o Identity
 //   .AddDefaultTokenProviders(); // Adiciona provedores de token padrão

builder.Services.AddRazorPages(); // Adiciona suporte para Razor Pages (ou AddControllersWithViews para MVC)
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    // Set properties and call methods on options
    serverOptions.ListenAnyIP(5000);
});

var app = builder.Build();

// Chama o método de inicialização para criar as roles
//await MySeedData.Initialize(app.Services, app.Services.GetRequiredService<UserManager<IdentityUser>>());

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}



app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors("AllowLocalImages");

app.UseAuthorization();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=SignUp}/{id?}");

app.Run();
