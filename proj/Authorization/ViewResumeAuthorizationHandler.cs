using System.Security.Claims;
using System.Threading.Tasks;
using Elfie.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using proj.Authorization;
using proj.Models;

public class ViewResumeAuthorizationHandler : AuthorizationHandler<ViewResumeRequirement, Resume>
{
    UserManager<IdentityUser> _userManager;

    public ViewResumeAuthorizationHandler(UserManager<IdentityUser>
        userManager)
    {
        _userManager = userManager;
    }
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ViewResumeRequirement requirement, Resume r)
    {
        // Check if the user is signed in
        if (context.User == null || r == null)
        {
            return Task.CompletedTask;
        }

        // Check if the user is trying to access their own resume
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId != null && userId == r.user.Id)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }

        return Task.CompletedTask;
    }
}