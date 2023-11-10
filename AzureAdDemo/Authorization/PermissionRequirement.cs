using Microsoft.AspNetCore.Authorization;

namespace AzureAdDemo.Authorization
{
	public class PermissionRequirement : IAuthorizationRequirement
	{
		public string Permission { get; }

		public PermissionRequirement(string permission)
		{
			Permission = permission;
		}
	}

}
