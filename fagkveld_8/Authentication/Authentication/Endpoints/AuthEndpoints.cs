using Microsoft.Identity.Web.Resource;

namespace Authentication.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/minimal");

        // 1. No authentication
        group.MapGet("/public", () => "This is a public endpoint.")
            .AllowAnonymous();

        // 2. Only needs authentication, no roles or scope
        group.MapGet("/authenticated", () => "You are authenticated.")
            .RequireAuthorization();
        
        // 3. Needs an Entra ID app role
        group.MapGet("/role-required", () => "You have the required App Role.")
            .RequireAuthorization(policy => policy.RequireRole("Role.Test"));

        // 4. Needs a scope
        group.MapGet("/scope-required", [RequiredScope("Scope.Test")] () => "You have the required Scope.")
            .RequireAuthorization();
    }
}
