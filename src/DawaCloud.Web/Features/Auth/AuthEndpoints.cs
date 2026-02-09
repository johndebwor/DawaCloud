using DawaCloud.Web.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace DawaCloud.Web.Features.Auth;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Authentication");

        group.MapPost("/login", Login);
        group.MapPost("/login-form", LoginForm).DisableAntiforgery();
        group.MapPost("/logout", Logout);
        group.MapGet("/logout", LogoutGet);
        group.MapPost("/register", Register);
        group.MapPost("/register-form", RegisterForm).DisableAntiforgery();
        group.MapPost("/forgot-password", ForgotPassword);
        group.MapPost("/forgot-password-form", ForgotPasswordForm).DisableAntiforgery();
        group.MapPost("/reset-password", ResetPassword);
    }

    private static async Task<IResult> LoginForm(
        HttpContext context,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager)
    {
        var form = await context.Request.ReadFormAsync();
        var email = form["email"].ToString();
        var password = form["password"].ToString();
        var rememberMe = form["rememberMe"].ToString().ToLower() == "true";
        var returnUrl = form["returnUrl"].ToString() ?? "/";

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            return Results.Redirect($"/auth/login?error={HttpUtility.UrlEncode("Email and password are required")}");
        }

        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return Results.Redirect($"/auth/login?error={HttpUtility.UrlEncode("Invalid email or password")}");
        }

        if (!user.IsActive)
        {
            return Results.Redirect($"/auth/login?error={HttpUtility.UrlEncode("Your account has been deactivated")}");
        }

        var result = await signInManager.PasswordSignInAsync(
            user,
            password,
            rememberMe,
            lockoutOnFailure: true);

        if (result.Succeeded)
        {
            user.LastLoginAt = DateTime.UtcNow;
            await userManager.UpdateAsync(user);
            return Results.Redirect(returnUrl);
        }

        if (result.IsLockedOut)
        {
            return Results.Redirect($"/auth/login?error={HttpUtility.UrlEncode("Account locked. Try again in 15 minutes")}");
        }

        if (result.IsNotAllowed)
        {
            return Results.Redirect($"/auth/login?error={HttpUtility.UrlEncode("Please confirm your email first")}");
        }

        return Results.Redirect($"/auth/login?error={HttpUtility.UrlEncode("Invalid email or password")}");
    }

    private static async Task<IResult> LogoutGet(SignInManager<ApplicationUser> signInManager)
    {
        await signInManager.SignOutAsync();
        return Results.Redirect("/auth/login");
    }

    private static async Task<IResult> RegisterForm(
        HttpContext context,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        var form = await context.Request.ReadFormAsync();
        var email = form["email"].ToString();
        var password = form["password"].ToString();
        var confirmPassword = form["confirmPassword"].ToString();
        var firstName = form["firstName"].ToString();
        var lastName = form["lastName"].ToString();
        var phoneNumber = form["phoneNumber"].ToString();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            return Results.Redirect($"/auth/register?error={HttpUtility.UrlEncode("Email and password are required")}");
        }

        if (password != confirmPassword)
        {
            return Results.Redirect($"/auth/register?error={HttpUtility.UrlEncode("Passwords do not match")}");
        }

        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            return Results.Redirect($"/auth/register?error={HttpUtility.UrlEncode("An account with this email already exists")}");
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName ?? string.Empty,
            LastName = lastName ?? string.Empty,
            PhoneNumber = phoneNumber,
            EmailConfirmed = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "Cashier");
            await signInManager.SignInAsync(user, isPersistent: false);
            return Results.Redirect("/");
        }

        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        return Results.Redirect($"/auth/register?error={HttpUtility.UrlEncode(errors)}");
    }

    private static async Task<IResult> ForgotPasswordForm(
        HttpContext context,
        UserManager<ApplicationUser> userManager)
    {
        var form = await context.Request.ReadFormAsync();
        var email = form["email"].ToString();

        if (string.IsNullOrEmpty(email))
        {
            return Results.Redirect($"/auth/forgot-password?error={HttpUtility.UrlEncode("Email is required")}");
        }

        var user = await userManager.FindByEmailAsync(email);
        if (user != null && user.IsActive)
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            // TODO: Send email via notification service
            Console.WriteLine($"Password reset token for {email}: {token}");
        }

        // Always redirect to success to prevent email enumeration
        return Results.Redirect($"/auth/forgot-password?success=true&email={HttpUtility.UrlEncode(email)}");
    }

    private static async Task<IResult> Login(
        [FromBody] LoginRequest request,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        {
            return Results.BadRequest(new AuthResponse(false, "Email and password are required"));
        }

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Results.BadRequest(new AuthResponse(false, "Invalid email or password"));
        }

        if (!user.IsActive)
        {
            return Results.BadRequest(new AuthResponse(false, "Your account has been deactivated. Please contact support."));
        }

        var result = await signInManager.PasswordSignInAsync(
            user,
            request.Password,
            request.RememberMe,
            lockoutOnFailure: true);

        if (result.Succeeded)
        {
            user.LastLoginAt = DateTime.UtcNow;
            await userManager.UpdateAsync(user);

            return Results.Ok(new AuthResponse(true, "Login successful", "/"));
        }

        if (result.IsLockedOut)
        {
            return Results.BadRequest(new AuthResponse(false, "Account locked. Please try again in 15 minutes."));
        }

        if (result.IsNotAllowed)
        {
            return Results.BadRequest(new AuthResponse(false, "Please confirm your email before logging in."));
        }

        return Results.BadRequest(new AuthResponse(false, "Invalid email or password"));
    }

    private static async Task<IResult> Logout(SignInManager<ApplicationUser> signInManager)
    {
        await signInManager.SignOutAsync();
        return Results.Ok(new AuthResponse(true, "Logged out successfully", "/auth/login"));
    }

    private static async Task<IResult> Register(
        [FromBody] RegisterRequest request,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        {
            return Results.BadRequest(new AuthResponse(false, "Email and password are required"));
        }

        if (request.Password != request.ConfirmPassword)
        {
            return Results.BadRequest(new AuthResponse(false, "Passwords do not match"));
        }

        var existingUser = await userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return Results.BadRequest(new AuthResponse(false, "An account with this email already exists"));
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName ?? string.Empty,
            LastName = request.LastName ?? string.Empty,
            PhoneNumber = request.PhoneNumber,
            EmailConfirmed = true, // For demo purposes - in production, send email confirmation
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            // Assign default role
            await userManager.AddToRoleAsync(user, "Cashier");

            // Auto sign-in after registration (for demo)
            await signInManager.SignInAsync(user, isPersistent: false);

            return Results.Ok(new AuthResponse(true, "Registration successful", "/"));
        }

        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        return Results.BadRequest(new AuthResponse(false, errors));
    }

    private static async Task<IResult> ForgotPassword(
        [FromBody] ForgotPasswordRequest request,
        UserManager<ApplicationUser> userManager)
    {
        if (string.IsNullOrEmpty(request.Email))
        {
            return Results.BadRequest(new AuthResponse(false, "Email is required"));
        }

        var user = await userManager.FindByEmailAsync(request.Email);

        // Always return success to prevent email enumeration attacks
        if (user == null || !user.IsActive)
        {
            return Results.Ok(new AuthResponse(true, "If your email is registered, you will receive a password reset link."));
        }

        var token = await userManager.GeneratePasswordResetTokenAsync(user);

        // TODO: Send email via INotificationService
        // For now, we'll just log the token (in production, send email)
        Console.WriteLine($"Password reset token for {request.Email}: {token}");

        return Results.Ok(new AuthResponse(true, "If your email is registered, you will receive a password reset link."));
    }

    private static async Task<IResult> ResetPassword(
        [FromBody] ResetPasswordRequest request,
        UserManager<ApplicationUser> userManager)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Token) || string.IsNullOrEmpty(request.NewPassword))
        {
            return Results.BadRequest(new AuthResponse(false, "All fields are required"));
        }

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Results.BadRequest(new AuthResponse(false, "Invalid request"));
        }

        var result = await userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

        if (result.Succeeded)
        {
            return Results.Ok(new AuthResponse(true, "Password reset successful", "/auth/login"));
        }

        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        return Results.BadRequest(new AuthResponse(false, errors));
    }
}

// Request/Response DTOs
public record LoginRequest(string Email, string Password, bool RememberMe = false);
public record RegisterRequest(string Email, string Password, string ConfirmPassword, string? FirstName, string? LastName, string? PhoneNumber);
public record ForgotPasswordRequest(string Email);
public record ResetPasswordRequest(string Email, string Token, string NewPassword);
public record AuthResponse(bool Success, string Message, string? RedirectUrl = null);
