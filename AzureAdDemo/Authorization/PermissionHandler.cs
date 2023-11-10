using AzureAdDemo.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace AzureAdDemo.Authorization
{
    /// <summary>
    ///  The PermissionHandler checks if the user is authorized to access a resource.
    /// </summary>
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly AzureAdDemoDbContext _context;

        public PermissionHandler(IHttpContextAccessor accessor, AzureAdDemoDbContext context)
        {
            _accessor = accessor;
            _context = context;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var httpContext = (context.Resource as DefaultHttpContext)?.HttpContext;
            if (httpContext != null)
            {
                if (_accessor.HttpContext.Request.Cookies["token"] == null)
                {
                    var azureUserId = context.User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;

                    if (azureUserId != null)
                    {
                        // Get the user by azure id
                        User user = await _context.User.FirstOrDefaultAsync(x => x.AzureUserId == Guid.Parse(azureUserId) && x.IsActive);
                        if (user != null)
                        {
                            var identity = context.User.Identity as ClaimsIdentity;
                            identity.AddClaim(new Claim("userid", user.Id.ToString()));

                            var roleNames = await _context.UserRoleMapping.Where(w => w.UserId == user.Id && w.IsActive).Include(x => x.RoleNavigation).Select(x => x.RoleNavigation.Name).ToListAsync();
                            if (roleNames != null && roleNames.Count > 0 && roleNames.Contains(requirement.Permission))
                            {
                                var token = Guid.NewGuid().ToString();
                                var roleIds = await _context.UserRoleMapping.Where(x => x.UserId == user.Id && x.IsActive).Select(s => s.RoleId).ToListAsync();
                                _accessor.HttpContext.Response.Cookies.Append("loginID", user.Id.ToString());
                                _accessor.HttpContext.Response.Cookies.Append("token", token);
                                _accessor.HttpContext.Response.Cookies.Append("roleIds", string.Join(',', roleIds));
                                context.Succeed(requirement);
                            }
                            else
                                context.Fail();
                        }
                        else
                        {
                            var newUser = new User()
                            {
                                Name = context.User.Identity.Name,
                                AzureUserId = Guid.Parse(azureUserId),
                                IsActive = true,
                                CreateDate = DateTime.Now
                            };
                            await _context.User.AddAsync(newUser);
                            await _context.SaveChangesAsync();

                            var roleId = (await _context.Role.FirstOrDefaultAsync(x => x.Name == "Admin" && x.IsActive)).Id;
                            var userRoleMapping = new UserRoleMapping()
                            {
                                UserId = newUser.Id,
                                RoleId = roleId,
                                IsActive = true,
                                CreateDate = DateTime.Now
                            };
                            await _context.UserRoleMapping.AddAsync(userRoleMapping);
                            await _context.SaveChangesAsync();

                            var identity = context.User.Identity as ClaimsIdentity;
                            identity.AddClaim(new Claim("userid", newUser.Id.ToString()));
                            context.Succeed(requirement);
                        }
                    }
                    else
                    {
                        httpContext.Response.Redirect("/Home/Login");
                        //context.Fail();
                    }

                }
                else
                {
                    context.Succeed(requirement);
                    //httpContext.Response.Redirect("/Home/Login");
                    //context.Fail();
                }
            }
            //context.Fail();
        }
    }
}