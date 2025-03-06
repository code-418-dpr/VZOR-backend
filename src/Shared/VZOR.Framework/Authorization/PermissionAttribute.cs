using Microsoft.AspNetCore.Authorization;

namespace VZOR.Framework.Authorization;

public class PermissionAttribute: AuthorizeAttribute, IAuthorizationRequirement
{
    public string Code { get; }

    public PermissionAttribute(string code) : base(policy: code) => Code = code;

}