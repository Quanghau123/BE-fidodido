using Microsoft.AspNetCore.Authorization;

namespace FidoDino.Common.Authorization
{
    //AuthorizationHandler : kiểm tra như thế nào
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var permissions = context.User
                .FindAll("permission")
                .Select(x => x.Value);

            if (permissions.Contains(requirement.Permission))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
