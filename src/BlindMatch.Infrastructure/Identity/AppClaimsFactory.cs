using System.Security.Claims;
using BlindMatch.Core.Common;
using BlindMatch.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace BlindMatch.Infrastructure.Identity;

public class AppClaimsFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
{
    public AppClaimsFactory(
        UserManager<ApplicationUser>  userManager,
        RoleManager<IdentityRole>     roleManager,
        IOptions<IdentityOptions>     options)
        : base(userManager, roleManager, options) { }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);

        identity.AddClaim(new Claim(AppClaimTypes.FullName, user.FullName));

        if (!string.IsNullOrEmpty(user.Programme))
            identity.AddClaim(new Claim(AppClaimTypes.Programme, user.Programme));

        if (user.YearOfStudy.HasValue)
            identity.AddClaim(new Claim(AppClaimTypes.YearOfStudy,
                user.YearOfStudy.Value.ToString()));

        return identity;
    }
}