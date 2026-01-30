using Microsoft.AspNetCore.Authorization;

namespace FidoDino.Common.Authorization
{

    //IAuthorizationRequirement: định nghĩa cần kiểm tra cái gì
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }

        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }
}
