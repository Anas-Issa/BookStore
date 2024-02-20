using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookStore.Authentication;
internal class CustomClaimsTransformer : IClaimsTransformation
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CustomClaimsTransformer(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {

        var accesToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
        // get user id from token 
        //...
        //add claims here 
        ((ClaimsIdentity)principal.Identity).AddClaim(new Claim("type-x", "value-x"));

        return Task.FromResult(principal);
    }
}
