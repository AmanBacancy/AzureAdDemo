using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace AzureAdDemo.Authorization
{
	public class PermissionPolicyProvider : IAuthorizationPolicyProvider
	{
		public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }
		public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
		{
			FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
		}
		public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();
		public Task<AuthorizationPolicy?> GetPolicyAsync(string permission)
		{
			if (!string.IsNullOrEmpty(permission))
			{
				AuthorizationPolicyBuilder policy = new AuthorizationPolicyBuilder();
				policy.AddRequirements(new PermissionRequirement(permission));
				return Task.FromResult(policy?.Build());
			}
			return FallbackPolicyProvider.GetPolicyAsync(permission);
		}
		public Task<AuthorizationPolicy> GetFallbackPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();

	}
}
