using machine_tool_rental_ca.Auth;
using machine_tool_rental_ca.Components;
using machine_tool_rental_ca.Data;
using machine_tool_rental_ca.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// DB（Neon / PostgreSQL）。Blazor Serverでは回路の間DbContextが長生きしないよう、Factoryで都合よく生成する
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException(
        "ConnectionStrings:Default が未設定です。ユーザーシークレット（またはContainer Appsの環境変数 ConnectionStrings__Default）に設定してください。");
builder.Services.AddDbContextFactory<AppDbContext>(o => o.UseNpgsql(connectionString));

// 認証（Cookie）。管理画面のID/パスワードはDbSeederで固定作成する
builder.Services.AddScoped<IPasswordHasher<AdminUser>, PasswordHasher<AdminUser>>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/login";
        o.ExpireTimeSpan = TimeSpan.FromHours(8);
        o.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

// マイグレーション適用 + 固定データ投入（Laravelの migrate + db:seed 相当）
using (var scope = app.Services.CreateScope())
{
    await DbSeeder.MigrateAndSeedAsync(scope.ServiceProvider);
}

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
else
{
    app.UseHsts();
}

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapAuthEndpoints();

app.Run();
