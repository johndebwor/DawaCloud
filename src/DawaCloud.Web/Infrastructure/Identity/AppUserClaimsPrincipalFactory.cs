using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace DawaCloud.Web.Infrastructure.Identity;

public class AppUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
{
    public AppUserClaimsPrincipalFactory(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, roleManager, optionsAccessor)
    {
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        identity.AddClaim(new Claim("FirstName", user.FirstName ?? string.Empty));
        identity.AddClaim(new Claim("LastName", user.LastName ?? string.Empty));
        if (user.TenantId.HasValue)
            identity.AddClaim(new Claim("TenantId", user.TenantId.Value.ToString()));
        return identity;
    }
}
