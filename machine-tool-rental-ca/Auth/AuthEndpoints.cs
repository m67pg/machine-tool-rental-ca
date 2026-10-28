using System.Security.Claims;
using machine_tool_rental_ca.Data;
using machine_tool_rental_ca.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace machine_tool_rental_ca.Auth;

public class LoginForm
{
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? ReturnUrl { get; set; }
}

public static class AuthEndpoints
{
    private const string DefaultReturnUrl = "/admin/machines";

    public static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/auth/login", async (
            [FromForm] LoginForm form,
            IDbContextFactory<AppDbContext> factory,
            IPasswordHasher<AdminUser> hasher,
            HttpContext http) =>
        {
            await using var db = await factory.CreateDbContextAsync();
            var userName = form.UserName?.Trim() ?? "";
            var user = await db.AdminUsers.AsNoTracking().SingleOrDefaultAsync(u => u.UserName == userName);

            var ok = user is not null &&
                     hasher.VerifyHashedPassword(user, user.PasswordHash, form.Password ?? "")
                         != PasswordVerificationResult.Failed;
            if (!ok)
            {
                return Results.Redirect("/login?error=1");
            }

            var identity = new ClaimsIdentity(
                [new Claim(ClaimTypes.Name, user!.UserName), new Claim(ClaimTypes.Role, "Admin")],
                CookieAuthenticationDefaults.AuthenticationScheme);

            await http.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            return Results.Redirect(SafeReturnUrl(form.ReturnUrl));
        });

        // デモ用のため GET でログアウトする（ログアウトCSRFの影響は小さい）
        app.MapGet("/auth/logout", async (HttpContext http) =>
        {
            await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.Redirect("/");
        });
    }

    // オープンリダイレクト対策: サイト内の絶対パスだけ許可
    private static string SafeReturnUrl(string? url) =>
        !string.IsNullOrEmpty(url) && url.StartsWith('/') && !url.StartsWith("//") && !url.StartsWith("/\\")
            ? url
            : DefaultReturnUrl;
}
