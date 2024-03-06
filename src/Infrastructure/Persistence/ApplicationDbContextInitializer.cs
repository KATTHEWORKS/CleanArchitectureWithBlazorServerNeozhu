﻿//#define HOSPITAL_SYSTEM
#define VOTING_SYSTEM
using System;
using System.Reflection;
using CleanArchitecture.Blazor.Application.Constants;
using CleanArchitecture.Blazor.Infrastructure.Constants.ClaimTypes;
using CleanArchitecture.Blazor.Infrastructure.PermissionSet;
using CleanArchitecture.Blazor.Infrastructure.Constants.Role;
using CleanArchitecture.Blazor.Infrastructure.Constants.User;

using CleanArchitecture.Blazor.Application.Constants.User;
using CleanArchitecture.Blazor.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using CleanArchitecture.Blazor.Infrastructure.Extensions;
using CleanArchitecture.Blazor.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using CleanArchitecture.Blazor.Application.Features.Tenants.DTOs;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence;
public class ApplicationDbContextInitializer
{
    private readonly ILogger<ApplicationDbContextInitializer> _logger;
    private readonly ApplicationDbContext _context;
    private CustomUserManager _userManager;
    private readonly CustomRoleManager _roleManager;
    private readonly IServiceProvider _serviceProvider;
    public ApplicationDbContextInitializer(ILogger<ApplicationDbContextInitializer> logger, ApplicationDbContext context, CustomUserManager userManager, CustomRoleManager roleManager, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _serviceProvider = serviceProvider;
    }
    public async Task InitialiseAsync()
    {
        try
        {
            if (_context.Database.IsSqlServer() || _context.Database.IsNpgsql() || _context.Database.IsSqlite())
            {
                await _context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database");
            throw;
        }
    }
    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
            _context.ChangeTracker.Clear();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }
    private static IEnumerable<string> GetAllPermissions()
    {
        var allPermissions = new List<string>();
        var modules = typeof(Permissions).GetNestedTypes();

        foreach (var module in modules)
        {
            var moduleName = string.Empty;
            var moduleDescription = string.Empty;

            var fields = module.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            foreach (var fi in fields)
            {
                var propertyValue = fi.GetValue(null);

                if (propertyValue is not null)
                    allPermissions.Add((string)propertyValue);
            }
        }

        return allPermissions;
    }

    private async Task TrySeedAsync()
    {
        // Default tenants
        if (!_context.Tenants.Any())
        {
            TenantBase.GetDefaultTenantStructure().ForEach(t =>
            {
                var tenant = _context.Tenants.Add(TenantStructure.Tenant(t));
                t.Id = tenant.Entity.Id;
            });

            //any more further basic
            await _context.SaveChangesAsync();
        }

        // Default roles //todo change these roles permissions
        var permissions = GetAllPermissions();
        foreach (var roles in TenantBase.GetDefaultTenantStructure().Select(t => t.Roles))
        {
            foreach (var role in roles)
            {
                //var ps = permissions.Where(r => r.StartsWith($"Permissions.{r}"));
                var ps = Perms.PermissionsAll.Where(x => x.roleOrType.Equals(role.Name, StringComparison.InvariantCultureIgnoreCase))
                    .FirstOrDefault().permissions;
                await AddRoleAndPermissions(role, ps);
            }

        }
#if HOSPITAL_SYSTEM
        TenantTypeEnum groupTenant = TenantTypeEnum.HospitalAndStaff;
        RoleNamesEnum groupAdminRole = RoleNamesEnum.HOSPITALADMIN;
        RoleNamesEnum groupViewerRole = RoleNamesEnum.HOSPITALADMIN;
#elif VOTING_SYSTEM
        TenantTypeEnum groupTenant = TenantTypeEnum.Default;
        RoleNamesEnum groupAdminRole = RoleNamesEnum.ELEVATEADMINGROUP;
        RoleNamesEnum groupViewerRole = RoleNamesEnum.ELEVATEADMINVIEWER;

#else
        TenantTypeEnum groupTenant = TenantTypeEnum.Default;
        RoleNamesEnum groupAdminRole = RoleNamesEnum.ELEVATEADMINGROUP;
        RoleNamesEnum groupViewerRole = RoleNamesEnum.ELEVATEADMINVIEWER;
#endif
        // Default users
        var defaultGoogleUsers = new List<(string email, RoleNamesEnum role, TenantTypeEnum tenantType)>()
       {("madhusudhan.veerabhadrappa@gmail.com",RoleNamesEnum.ROOTADMIN ,TenantTypeEnum.Internal)
       //,("vmadhu203@gmail.com", groupAdminRole, groupTenant)
       //,("vmadhu2023@gmail.com", groupViewerRole, groupTenant)
       };
        //TODO change this logic hospital admin/viewer should be only under specific hospital tenant rather general

        foreach (var (email, role, tenantType) in defaultGoogleUsers)
        {//need to verify
            using (var scope = _serviceProvider.CreateScope())
            {
                //var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                using (_userManager = scope.ServiceProvider.GetRequiredService<CustomUserManager>())
                {
                    if (!_userManager.Users.Any(u => u.Email == email))
                    {
                        var tenant1 = _context.Tenants.First(x => x.Type == (byte)tenantType);
                        var newUser = new ApplicationUser
                        {
                            UserName = email,
                            Provider = "Google",
                            IsActive = true,
                            //DefaultTenantId = _context.UserRoleTenants.First().Id,//todo need to make change
                            //todo need to change based on selection like whether PATIENT/Internal/any hospital
                            DefaultTenantId = tenant1.Id,//todo need to make change
                            DefaultTenantName = tenant1.Name,
                            DisplayName = email,
                            Email = email,
                            EmailConfirmed = true
                            //    , ProfilePictureDataUrl = "https://s.gravatar.com/avatar/78be68221020124c23c665ac54e07074?s=80" 
                        };
                        await _userManager.CreateAsync(newUser, roles: new List<string>() { role.ToString() });//todo pass roles and tenantids  //, UserName.DefaultPassword);

                    }
                }
            }
        }
        #region mustRemove
        //still in seeding StaticData are not loaded so cant use that
        var allTenants = new List<TenantDto> { };
        using (var scope = _serviceProvider.CreateScope())
        using (_userManager = scope.ServiceProvider.GetRequiredService<CustomUserManager>())
            allTenants = await _userManager.GetAllTenants();

        var internalTenant = allTenants.FirstOrDefault(x => x.Type == (byte)TenantTypeEnum.Internal);
        await AddNewUserToDb(UserName.Administrator, internalTenant, new List<RoleNamesEnum> { RoleNamesEnum.ROOTADMIN });

        var defaultTenant = allTenants.FirstOrDefault(x => x.Type == (byte)TenantTypeEnum.Default);
        await AddNewUserToDb(RoleNamesEnum.Default.ToString(), defaultTenant, new List<RoleNamesEnum> { RoleNamesEnum.Default });
        await AddNewUserToDb(RoleNamesEnum.Default.ToString() + "1", defaultTenant, new List<RoleNamesEnum> { RoleNamesEnum.Default });



#if HOSPITAL_SYSTEM
        var hospitalTenant = allTenants.FirstOrDefault(x => x.Type == (byte)groupTenant);
        await AddNewUserToDb(RoleNamesEnum.HOSPITAL.ToString(), hospitalTenant, new List<RoleNamesEnum> { RoleNamesEnum.HOSPITAL, RoleNamesEnum.HOSPITALADMIN, RoleNamesEnum.VIEWERHOSPITAL });
        await AddNewUserToDb(RoleNamesEnum.HOSPITALADMIN.ToString(), hospitalTenant, new List<RoleNamesEnum> { RoleNamesEnum.HOSPITALADMIN });
        await AddNewUserToDb(RoleNamesEnum.HOSPITALADMIN.ToString() + "1", hospitalTenant, [RoleNamesEnum.HOSPITALADMIN]);
        await AddNewUserToDb(RoleNamesEnum.VIEWERHOSPITAL.ToString(), hospitalTenant, new List<RoleNamesEnum> { RoleNamesEnum.VIEWERHOSPITAL });
        await AddNewUserToDb(RoleNamesEnum.VIEWERHOSPITAL.ToString() + "1", hospitalTenant, new List<RoleNamesEnum> { RoleNamesEnum.VIEWERHOSPITAL });
#elif VOTING_SYSTEM
/* 
       VoteConstituency AddConstituency(VoteConstituency c)
        {
            if (!_context.VoteConstituencies.Any(x => x.Name == c.Name && x.State == c.State))
            {
                _context.VoteConstituencies.Add(c);
                return c;
            }
            return null;
        }

        if (!_context.VoteConstituencies.Any() || AddConstituency(new VoteConstituency("Karnataka", "Shimoga", "BYR", "asdasdsad")) != null)
        {
            AddConstituency(new VoteConstituency("Karnataka", "Chikmagalur", "Bojegowda", "asdasdsad"));
            AddConstituency(new VoteConstituency("Karnataka", "Mandya", "Suma", "asdasdsad"));
            AddConstituency(new VoteConstituency("Tnad", "Chennai", "MGR", "asdasdsad"));
            AddConstituency(new VoteConstituency("Tnad", "Tiruchi", "BYR", "asdasdsad"));
            await _context.SaveChangesAsync();
        }
*/
#endif

        #endregion mustRemove
        // Default data
        // Seed, if necessary
        if (!_context.KeyValues.Any())
        {
            _context.KeyValues.Add(new KeyValue { Name = Picklist.Status, Value = "initialization", Text = "initialization", Description = "Status of workflow" });
            _context.KeyValues.Add(new KeyValue { Name = Picklist.Status, Value = "processing", Text = "processing", Description = "Status of workflow" });
            _context.KeyValues.Add(new KeyValue { Name = Picklist.Status, Value = "pending", Text = "pending", Description = "Status of workflow" });
            _context.KeyValues.Add(new KeyValue { Name = Picklist.Status, Value = "finished", Text = "finished", Description = "Status of workflow" });
            _context.KeyValues.Add(new KeyValue { Name = Picklist.Brand, Value = "Apple", Text = "Apple", Description = "Brand of production" });
            _context.KeyValues.Add(new KeyValue { Name = Picklist.Brand, Value = "MI", Text = "MI", Description = "Brand of production" });
            _context.KeyValues.Add(new KeyValue { Name = Picklist.Brand, Value = "Logitech", Text = "Logitech", Description = "Brand of production" });
            _context.KeyValues.Add(new KeyValue { Name = Picklist.Brand, Value = "Linksys", Text = "Linksys", Description = "Brand of production" });

            _context.KeyValues.Add(new KeyValue { Name = Picklist.Unit, Value = "EA", Text = "EA", Description = "Unit of product" });
            _context.KeyValues.Add(new KeyValue { Name = Picklist.Unit, Value = "KM", Text = "KM", Description = "Unit of product" });
            _context.KeyValues.Add(new KeyValue { Name = Picklist.Unit, Value = "PC", Text = "PC", Description = "Unit of product" });
            _context.KeyValues.Add(new KeyValue { Name = Picklist.Unit, Value = "KG", Text = "KG", Description = "Unit of product" });
            _context.KeyValues.Add(new KeyValue { Name = Picklist.Unit, Value = "ST", Text = "ST", Description = "Unit of product" });
            await _context.SaveChangesAsync();

        }
        if (!_context.Products.Any())
        {
            _context.Products.Add(new Product { Brand = "Apple", Name = "IPhone 13 Pro", Description = "Apple iPhone 13 Pro smartphone. Announced Sep 2021. Features 6.1″ display, Apple A15 Bionic chipset, 3095 mAh battery, 1024 GB storage.", Unit = "EA", Price = 999.98m });
            _context.Products.Add(new Product { Brand = "MI", Name = "MI 12 Pro", Description = "Xiaomi 12 Pro Android smartphone. Announced Dec 2021. Features 6.73″ display, Snapdragon 8 Gen 1 chipset, 4600 mAh battery, 256 GB storage.", Unit = "EA", Price = 199.00m });
            _context.Products.Add(new Product { Brand = "Logitech", Name = "MX KEYS Mini", Description = "Logitech MX Keys Mini Introducing MX Keys Mini – a smaller, smarter, and mightier keyboard made for creators. Type with confidence on a keyboard crafted for efficiency, stability, and...", Unit = "PA", Price = 99.90m });
            await _context.SaveChangesAsync();
        }
    }
    private static ApplicationUser GetNewUser(string userName, string tenantId, string tenantName)
    {
        return new ApplicationUser { UserName = userName, IsActive = true, Provider = "Local", DefaultTenantId = tenantId, DefaultTenantName = tenantName, DisplayName = userName + " profile", Email = userName + "@mail.com", EmailConfirmed = true, ProfilePictureDataUrl = "https://s.gravatar.com/avatar/ea753b0b0f357a41491408307ade445e?s=80" };
    }
    private async Task AddNewUserToDb(string userName, TenantDto? tenant, List<RoleNamesEnum> roleNamesEnums)
    {
        var user = GetNewUser(userName, tenant.Id, tenant.Name);

        using (var scope = _serviceProvider.CreateScope())
        {
            using (_userManager = scope.ServiceProvider.GetRequiredService<CustomUserManager>())
            {
                if (!_userManager.Users.Any(u => u.UserName == user.UserName))
                {
                    await _userManager.CreateAsync(user, roles: roleNamesEnums.Select(e => e.ToString()).ToList(), password: UserName.DefaultPassword);
                }
            }
        }
    }
    private async Task AddRoleAndPermissions(ApplicationRole role, IEnumerable<string> permissions)
    {
        if (!_roleManager.Roles.Any(r => r.Name == role.Name))
        {
            await _roleManager.CreateAsync(role);
            //todo AspNetRoles extend createasync to add Level parameter also
            if (permissions != null && permissions.Any())
                foreach (var permission in permissions)
                {
                    await _roleManager.AddClaimAsync(role, new Claim(ApplicationClaimTypes.Permission, permission));
                }
        }
    }
}

/*
         _context.UserRoleTenants.Add(new Tenant { Name = "Nanjappa HOSPITAL", Description = "Nanjappa HOSPITAL" });
         _context.UserRoleTenants.Add(new Tenant { Name = "Sarji HOSPITAL", Description = "Sarji HOSPITAL" });
         _context.UserRoleTenants.Add(new Tenant { Name = "Narayana HOSPITAL", Description = "Narayana HOSPITAL" });
         */
/*
 await AddRoleAndPermissions(new ApplicationRole(RoleName.ROOTADMIN), permissions);
 await AddRoleAndPermissions(new ApplicationRole(RoleName.ELEVATEADMINGROUP), permissions);
 await AddRoleAndPermissions(new ApplicationRole(RoleName.ELEVATEADMINVIEWER), permissions);

 await AddRoleAndPermissions(new ApplicationRole(RoleName.HOSPITAL), permissions);
 await AddRoleAndPermissions(new ApplicationRole(RoleName.HOSPITALADMIN), permissions);
 await AddRoleAndPermissions(new ApplicationRole(RoleName.DOCTORHOD), permissions);
 await AddRoleAndPermissions(new ApplicationRole(RoleName.DOCTOR), permissions);
 await AddRoleAndPermissions(new ApplicationRole(RoleName.DOCTORASSISTANT), permissions);
 await AddRoleAndPermissions(new ApplicationRole(RoleName.NURSE), permissions);
 await AddRoleAndPermissions(new ApplicationRole(RoleName.VIEWERHOSPITAL), permissions);

 await AddRoleAndPermissions(new ApplicationRole(RoleName.DIAGNOSTICCENTER), permissions);
 await AddRoleAndPermissions(new ApplicationRole(RoleName.DIAGNOSTICIAN), permissions);

 await AddRoleAndPermissions(new ApplicationRole(RoleName.PHARMACY), permissions);
 await AddRoleAndPermissions(new ApplicationRole(RoleName.PHARMACIST), permissions);

 await AddRoleAndPermissions(new ApplicationRole(RoleName.PATIENT), permissions);
 */
//  await AddRoleAndPermissions(new ApplicationRole(RoleName.GUEST), permissions);

/*      var userRole = new ApplicationRole(RoleName.Basic) { Description = "Basic Group" };
      if (_roleManager.Roles.All(r => r.Name != userRole.Name))
      {
          await _roleManager.CreateAsync(userRole);
          foreach (var permission in permissions)
          {
              if (permission.StartsWith("Permissions.Products"))
                  await _roleManager.AddClaimAsync(userRole, new Claim(ApplicationClaimTypes.Permission, permission));
          }
      } */

/*var user = new ApplicationUser { UserName = UserName.Administrator, Provider = "Local", IsActive = true, DefaultTenantId = _context.UserRoleTenants.First().Id, DefaultTenantName = _context.UserRoleTenants.First().Name, DisplayName = UserName.Administrator, Email = "new163@163.com", EmailConfirmed = true, ProfilePictureDataUrl = "https://s.gravatar.com/avatar/78be68221020124c23c665ac54e07074?s=80" };
var demo = new ApplicationUser { UserName = UserName.Demo, IsActive = true, Provider = "Local", DefaultTenantId = _context.UserRoleTenants.First().Id, DefaultTenantName = _context.UserRoleTenants.First().Name, DisplayName = UserName.Demo, Email = "neozhu@126.com", EmailConfirmed = true, ProfilePictureDataUrl = "https://s.gravatar.com/avatar/ea753b0b0f357a41491408307ade445e?s=80" };
if (_userManager.Users.All(u => u.UserName != user.UserName))
{
    await _userManager.CreateAsync(user, UserName.DefaultPassword);
    await _userManager.AddToRolesAsync(user, new[] { administratorRole.Name! });
}
if (_userManager.Users.All(u => u.UserName != demo.UserName))
{
    await _userManager.CreateAsync(demo, UserName.DefaultPassword);
    await _userManager.AddToRolesAsync(demo, new[] { userRole.Name! });
}
*/
//await _context.UserTenant.AddAsync(new UserTenant() { UserId = newUser.Id, DefaultTenantId = _context.UserRoleTenants.First().Id });
//await _userManager.AddToRolesAsync(newUser, new[] { role! });