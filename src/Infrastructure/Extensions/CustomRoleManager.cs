using System.Diagnostics.Tracing;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using CleanArchitecture.Blazor.Application.Common.Extensions;
using CleanArchitecture.Blazor.Infrastructure.PermissionSet;
using CleanArchitecture.Blazor.Application.Constants.User;
using CleanArchitecture.Blazor.Domain.Entities;
using CleanArchitecture.Blazor.Domain.Enums;
using PublicCommon;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using FluentEmail.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.IdentityModel.Tokens;
using CleanArchitecture.Blazor.Infrastructure.Common.Extensions;
using static CleanArchitecture.Blazor.Infrastructure.PermissionSet.Permissions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using CleanArchitecture.Blazor.Domain.Identity;
using CleanArchitecture.Blazor.Infrastructure.Services.MultiTenant;
using CleanArchitecture.Blazor.Application.Features.Tenants.DTOs;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using System.Data.Entity;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Blazor.Infrastructure.Extensions;

public class CustomRoleManager(ApplicationDbContext dbContext,
IRoleStore<ApplicationRole> store,
    IEnumerable<IRoleValidator<ApplicationRole>> roleValidators,
    ILookupNormalizer keyNormalizer,
    IdentityErrorDescriber errors,
    ILogger<CustomRoleManager> logger,
    IServiceProvider serviceProvider)
    : RoleManager<ApplicationRole>(store, roleValidators, keyNormalizer, errors, logger)
{
    ApplicationDbContext _dbContext = dbContext;
    public async Task<ApplicationRole?> FindByNameAsync(string roleName, TenantTypeEnum type)
    {
        return await FindByNameAsync(roleName, (byte)type);
    }
    public async Task<ApplicationRole?> FindByNameAsync(string roleName, byte tenantType)
    {
        //using (var _dbContext = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>())
            return roleName.IsNullOrEmptyAndTrimSelf()
                ? null
                : await _dbContext.Roles?.FirstOrDefaultAsync(r => r != null && r.Name! == roleName! && r.TenantType == tenantType);
    }




    //public async Task<ApplicationRole> AddToRoleAsync(ApplicationUser user, string roleName)
    //{
    //    throw new NotImplementedException("Please use with tenantId");
    //}


    //public async Task<IdentityRole> FindByNameAsync(string roleName)
    //{
    //    return await Roles?.FirstOrDefaultAsync(r => r.Name == roleName);
    //}
}

public interface IUserRoleService
{
    Task<int> UpdateUserRolesAsync(string userId, string tenantId, List<string> roleIds);
    Task<bool> RemoveUserRolesWithTenantIdAsync(string userId, string tenantId, List<string> roleIds);
}

public class UserRoleService : IUserRoleService
{
    private readonly ICustomUserManager _customUserManager;
    IServiceProvider _serviceProvider;

    public UserRoleService(ICustomUserManager customUserManager, IServiceProvider serviceProvider)
    {
        _customUserManager = customUserManager;
        _serviceProvider = serviceProvider;
    }

    public async Task<int> UpdateUserRolesAsync(string userId, string tenantId, List<string> roleIds)
    {
        using (var dbContext = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>())
            //above should be removed and make use of DI but still pending //TODO if IApplicationDbContext with database ,entry and all
        using (var transaction = await dbContext.Database.BeginTransactionAsync())
        {
            try
            {
                // Ensure that the user exists
                var user = await _customUserManager.FindByIdAsync(userId);
                if (user == null)
                {
                    // Handle the case where the user is not found
                    return -1;
                }

                // Get the current roles of the user for the specific tenant
                var currentRoles = await dbContext.UserRoleTenants
                    .Where(ur => ur.UserId == userId && ur.TenantId == tenantId)
                    .Select(ur => ur.RoleId)
                    .ToListAsync();

                // Calculate the roles to be added and removed
                var rolesToAdd = roleIds.Except(currentRoles);
                var rolesToRemove = currentRoles.Except(roleIds);

                // Add new roles
                foreach (var roleId in rolesToAdd)
                {
                    var newUserRole = new UserRoleTenant
                    {
                        UserId = userId,
                        RoleId = roleId,
                        TenantId = tenantId
                    };

                    dbContext.UserRoleTenants.Add(newUserRole);
                }

                // Remove old roles
                foreach (var roleId in rolesToRemove)
                {
                    var userRoleToRemove = await dbContext.UserRoleTenants
                        .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId && ur.TenantId == tenantId);

                    if (userRoleToRemove != null)
                    {
                        dbContext.UserRoleTenants.Remove(userRoleToRemove);
                    }
                }

                // Save changes to the database
                var resultCount = await dbContext.SaveChangesAsync();

                // Commit the transaction if everything is successful
                await transaction.CommitAsync();
                return resultCount;
            }
            catch (Exception ex)
            {
                // Handle exceptions and log if necessary
                await transaction.RollbackAsync();
                throw;
            }
            //return 0;
        }
    }

    public async Task<bool> RemoveUserRolesWithTenantIdAsync(string userId, string tenantId, List<string> roleIds)
    {
        using (var dbContext = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>())
        using (var transaction = await dbContext.Database.BeginTransactionAsync())
        {
            try
            {
                var user = await _customUserManager.FindByIdAsync(userId);

                if (user == null)
                {
                    // User not found
                    return false;
                }

                // Ensure the user is in the specified tenant
                //if (user.DefaultTenantId != tenantId)//this is not required

                // Remove user from the specified roles in UserRoleTenant mapping table
                var userRolesToRemove = await dbContext.UserRoleTenants
                    .Where(ur => ur.UserId == userId && ur.TenantId == tenantId && roleIds.Contains(ur.RoleId))
                    .ToListAsync();

                dbContext.UserRoleTenants.RemoveRange(userRolesToRemove);
                var resultCount = await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Handle exceptions, log errors, etc.
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}














/*
 public class YourDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
}

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    // Add other columns as needed
}

public class Repository<T> where T : class
{
    private YourDbContext _dbContext;

    public Repository(YourDbContext context)
    {
        _dbContext = context;
    }

    public void UpdateColumnAsync<TProperty>(T entity, Expression<Func<T, TProperty>> propertyExpression, TProperty value)
    {
        var propertyName = ((MemberExpression)propertyExpression.Body).Member.Name;
        var entry = _dbContext.Entry(entity);
        entry.Property(propertyName).IsModified = true;
        entry.CurrentValues.SetValues(entity);

        _dbContext.SaveChanges();
    }
}

class Program
{
    static void Main()
    {
        using (var _dbContext = new YourDbContext())
        {
            var repository = new Repository<User>(_dbContext);

            var user = _dbContext.Users.FirstOrDefault(u => u.Id == 1);
            if (user != null)
            {
                repository.UpdateColumnAsync(user, u => u.FirstName, "NewFirstName");
            }
        }
    }
}
 */