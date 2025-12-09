using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace Authentication.Controllers;

[ApiController]
[Route("api/controller")]
public class AuthController : ControllerBase
{
    // 1. No authentication
    [HttpGet("public")]
    [AllowAnonymous]
    public IActionResult GetPublic()
    {
        return Ok("This is a public endpoint.");
    }

    // 2. Only needs authentication, no roles or scope
    [HttpGet("authenticated")]
    [Authorize]
    public IActionResult GetAuthenticated()
    {
        return Ok("You are authenticated.");
    }
    
    // 3. Needs an Entra ID app role
    [HttpGet("role-required")]
    [Authorize(Roles = "Role.Test")]
    public IActionResult GetRoleRequired()
    {
        return Ok("You have the required App Role.");
    }

    // 4. Needs a scope
    [HttpGet("scope-required")]
    [Authorize]
    [RequiredScope("Scope.Test")]
    public IActionResult GetScopeRequired()
    {
        return Ok("You have the required Scope.");
    }
}
