//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using CleanArchitecture.Blazor.Domain.Entities;
//using Microsoft.CodeAnalysis.CSharp.Syntax;
//using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

//namespace CleanArchitecture.Blazor.Domain.Identity;

////mostly not using anywhere,so can be removed or .. //TODO
//public static class UserManagerExtensions
//{
//    public static async Task<int> RemoveFromRolesAsyncWithTenantName(
//       string userId,
//       string tenantName, IApplicationDbContext context,
//       params string[] roles)
//    {
//        var tenant = await context.Tenants.FirstAsync(x => x.Name == tenantName);
//        return tenant != null ? await RemoveFromRolesAsyncWithTenantId(userId, tenant.Id, context, roles) : 0;
//    }
//    public static async Task<int> RemoveFromRolesAsyncWithTenantId(
//        string userId,
//        string tenantId, IApplicationDbContext context,
//        params string[] roles)
//    {
//        var itemsToRemove = context.UserRoleTenants.Where(item => item.UserId == userId && item.DefaultTenantId == tenantId &&
//        context.UserRoleTenants.Where(r => roles.Contains(r.Role.Name)).Contains(item.Role));
//        context.UserRoles.RemoveRange(itemsToRemove);
//        return await context.SaveChangesAsync();
//    }


//    public static async Task<int> AddToRolesAsyncWithTenantId(
//        string userId,
//        string tenantId, IApplicationDbContext context,
//        params string[] roles)
//    {
//        var rolesList = await context.UserRoleTenants.Where(x => roles.Contains(x.Role.Name)).ToListAsync();
//        if (rolesList != null && rolesList.Any())
//        {
//            var rolesToInsert = new List<UserRoleTenant>();
//            rolesList.ForEach(r => rolesToInsert.Add(new UserRoleTenant() { UserId = userId, DefaultTenantId = tenantId, Role = r }));
//            await context.UserRoleTenants.AddRangeAsync(rolesToInsert);
//            var resultCount = await context.SaveChangesAsync();
//            return resultCount;
//        }
//        return 0;
//    }
//    public static async Task<int> AddToRolesAsyncWithTenantName(
//        string userId,
//        string tenantName, IApplicationDbContext context,
//        params string[] roles)
//    {

//        var tenant = await context.Tenants.FirstAsync(x => x.Name == tenantName);
//        return tenant != null ? await AddToRolesAsyncWithTenantId(userId, tenant.Id, context, roles) : 0;
//    }

//}
