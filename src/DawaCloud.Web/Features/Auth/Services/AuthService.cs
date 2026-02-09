using DawaCloud.Web.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace DawaCloud.Web.Features.Auth.Services;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(string email, string password, bool rememberMe);
    Task<AuthResult> RegisterAsync(RegisterDto dto);
    Task<AuthResult> ForgotPasswordAsync(string email);
    Task<AuthResult> ResetPasswordAsync(string email, string token, string newPassword);
    Task LogoutAsync();
}

public class AuthService : IAuthService
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        ILogger<AuthService> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<AuthResult> LoginAsync(string email, string password, bool rememberMe)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return AuthResult.Failure("Invalid email or password");
            }

            if (!user.IsActive)
            {
                return AuthResult.Failure("Your account has been deactivated. Please contact support.");
            }

            var result = await _signInManager.PasswordSignInAsync(
                user,
                password,
                rememberMe,
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                user.LastLoginAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
                _logger.LogInformation("User {Email} logged in successfully", email);
                return AuthResult.SuccessResult("/");
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("User {Email} account locked out", email);
                return AuthResult.Failure("Account locked. Please try again in 15 minutes.");
            }

            if (result.IsNotAllowed)
            {
                return AuthResult.Failure("Please confirm your email before logging in.");
            }

            return AuthResult.Failure("Invalid email or password");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for {Email}", email);
            return AuthResult.Failure("An error occurred during login. Please try again.");
        }
    }

    public async Task<AuthResult> RegisterAsync(RegisterDto dto)
    {
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                return AuthResult.Failure("An account with this email already exists");
            }

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName ?? string.Empty,
                LastName = dto.LastName ?? string.Empty,
                PhoneNumber = dto.PhoneNumber,
                EmailConfirmed = true, // For demo - in production, send email confirmation
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (result.Succeeded)
            {
                // Assign default role
                await _userManager.AddToRoleAsync(user, "Cashier");

                // Auto sign-in after registration (for demo)
                await _signInManager.SignInAsync(user, isPersistent: false);

                _logger.LogInformation("User {Email} registered successfully", dto.Email);
                return AuthResult.SuccessResult("/");
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return AuthResult.Failure(errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for {Email}", dto.Email);
            return AuthResult.Failure("An error occurred during registration. Please try again.");
        }
    }

    public async Task<AuthResult> ForgotPasswordAsync(string email)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);

            // Always return success to prevent email enumeration attacks
            if (user == null || !user.IsActive)
            {
                return AuthResult.SuccessResult(message: "If your email is registered, you will receive a password reset link.");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // TODO: Send email via INotificationService
            _logger.LogInformation("Password reset token generated for {Email}: {Token}", email, token);

            return AuthResult.SuccessResult(message: "If your email is registered, you will receive a password reset link.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during forgot password for {Email}", email);
            return AuthResult.Failure("An error occurred. Please try again.");
        }
    }

    public async Task<AuthResult> ResetPasswordAsync(string email, string token, string newPassword)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return AuthResult.Failure("Invalid request");
            }

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (result.Succeeded)
            {
                _logger.LogInformation("Password reset successful for {Email}", email);
                return AuthResult.SuccessResult("/auth/login", "Password reset successful. Please login with your new password.");
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return AuthResult.Failure(errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password reset for {Email}", email);
            return AuthResult.Failure("An error occurred. Please try again.");
        }
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("User logged out");
    }
}

public class AuthResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? RedirectUrl { get; set; }

    public static AuthResult SuccessResult(string? redirectUrl = null, string message = "Success")
        => new() { Success = true, Message = message, RedirectUrl = redirectUrl };

    public static AuthResult Failure(string message)
        => new() { Success = false, Message = message };
}

public record RegisterDto(
    string Email,
    string Password,
    string? FirstName,
    string? LastName,
    string? PhoneNumber);
