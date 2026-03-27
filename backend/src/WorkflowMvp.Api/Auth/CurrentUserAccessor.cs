using System.Security.Claims;

namespace WorkflowMvp.Api.Auth;

public interface ICurrentUserAccessor
{
    string GetActor(ClaimsPrincipal user);
    string GetRole(ClaimsPrincipal user);
}

public class CurrentUserAccessor : ICurrentUserAccessor
{
    public string GetActor(ClaimsPrincipal user)
        => user.Identity?.Name
            ?? user.FindFirstValue("preferred_username")
            ?? "demo.user";

    public string GetRole(ClaimsPrincipal user)
        => user.FindFirst("role")?.Value
            ?? user.FindFirst("roles")?.Value
            ?? "user";
}
