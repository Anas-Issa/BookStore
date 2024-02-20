using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Security.Claims;

namespace Acme.BookStore.Authentication;
public class FullNameClaimsPrincipalContributer : IAbpClaimsPrincipalContributor, ITransientDependency
{
    public async Task ContributeAsync(AbpClaimsPrincipalContributorContext context)
    {
        var identity = context.ClaimsPrincipal.Identities.FirstOrDefault();
        var name = identity?.FindFirst(AbpClaimTypes.Name)?.Value;
        var surename = identity?.FindFirst(AbpClaimTypes.Name)?.Value;

        if (!name.IsNullOrEmpty() && !surename.IsNullOrEmpty())
        {
            string fullName = name + " " + surename;
            identity.AddClaim(new Claim("FullName", fullName));
        }
        await Task.CompletedTask;

    }
}
