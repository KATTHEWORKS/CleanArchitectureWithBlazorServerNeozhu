using System.Diagnostics.Tracing;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using CleanArchitecture.Blazor.Application.Common.Extensions;
using CleanArchitecture.Blazor.Infrastructure.PermissionSet;
using CleanArchitecture.Blazor.Application.Constants.User;
using CleanArchitecture.Blazor.Domain.Entities;
using CleanArchitecture.Blazor.Domain.Enums;
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
using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;

namespace CleanArchitecture.Blazor.Infrastructure.Extensions;

public interface ICustomUserManager
{
    Task<List<(string TenantId, string TenantName)>> GetMyTenants(string userId);
    Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string roleName);

    Task<IdentityResult> CreateAsync(ApplicationUser user, List<string>? roles = null, string? tenantId = null, string? password = null);
    Task<IdentityResult> CreateWithDefaultRolesAsync(ApplicationUser user, string? tenantId = null, string? password = null);
    Task<ApplicationUser?> FindByIdAsync(string userId, bool isTrackingNeeded = false);
    Task<ApplicationUser?> FindByNameAsync(string userName, bool isTrackingNeeded = false);
    Task<ApplicationUser?> FindByNameForLocalAccountAsync(string userName, bool isTrackingNeeded = false);
    Task<ApplicationUser?> FindByNameOrId(string userName = "", Guid? userId = null, bool isTrackingNeeded = false);
    Task<ApplicationUser?> FindByNameOrIdFullUserObject(string userName = "", Guid? userId = null, bool isTrackingNeeded = false);
    List<TenantDto> GetAllowedTenants(ApplicationUser user);
    List<TenantDto> GetAllowedTenants(ApplicationUserDto userDto);
    Task<List<TenantDto>> GetAllTenants(bool forceLoad = false);
    Task<IdentityResult> RemoveFromRoleAsync(ApplicationUser user, string roleName);
    Task<int?> RolesUpdateInsert(ApplicationUser user, IEnumerable<string> roleNames);
    Task<IdentityResult> UpdateAsync(ApplicationUser user);
    Task<int> UpdateColumn(string userId, string columnName, object newValue);
    Task<int> UpdateIsLive(string userId, bool isLive = false);
    Task<IdentityResult> UpdateUserDefaultTenant(string userId, string tenantId);

    IdentityResult ChangeUserDefaultTenant(string userId, string tenantId);
    Task<IList<UserRoleTenant>> CheckUserHasTenantRolePermission(string userId, string? roleId = null, string? tenantId = null);
}

public class CustomUserManager : UserManager<ApplicationUser>, ICustomUserManager
{
    readonly List<string> _defaultRoles = new() { RoleNamesEnum.Default.ToString() };
    // private Repository<ApplicationUser> _repository;
    public const string DefaultTenantId = "";//todo make it loaded as per db
    private readonly CustomRoleManager _roleManager;
    private readonly IServiceProvider _serviceProvider;
    private ApplicationDbContext _dbContext;
    //private readonly IServiceScopeFactory _scopeFactory;//Currently its not using due to scope issue ,need to fix later //TODO
    private readonly TenantService _tenantService;
    private readonly IMapper _mapper;
    public CustomUserManager(
         ApplicationDbContext context,//Currently its not using due to scope issue ,need to fix later //TODO
         IUserStore<ApplicationUser> store,
         IOptions<IdentityOptions> optionsAccessor,
         IPasswordHasher<ApplicationUser> passwordHasher,
         IEnumerable<IUserValidator<ApplicationUser>> userValidators,
         IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
         ILookupNormalizer keyNormalizer,
         IdentityErrorDescriber errors,
         IServiceProvider services,
         ILogger<UserManager<ApplicationUser>> logger, CustomRoleManager roleManager,
         //IServiceScopeFactory scopeFactory,
         TenantService tenantService, IMapper mapper)
         : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {
        //_scopeFactory = scopeFactory;//Currently its not using due to scope issue ,need to fix later //TODO
        _roleManager = roleManager;
        _serviceProvider = services;
        _dbContext = context; //services.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
        // _repository = new Repository<ApplicationUser>(_dbContext);
        _tenantService = tenantService;
        _mapper = mapper;
    }
    public async Task<IdentityResult> UpdateUserDefaultTenant(string userId, string tenantId)
    {
        if (!(string.IsNullOrEmpty(userId) && string.IsNullOrEmpty(tenantId)))

            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {

                    // Check if the tenantId exists in the EmployeeTenantMappingTable
                    var mappingExists = await _dbContext.UserRoleTenants.AnyAsync(m => m.UserId == userId && m.TenantId == tenantId);

                    if (!mappingExists)
                    {
                        // Handle the case where the mapping doesn't exist (throw exception, log, etc.)
                        // You might want to roll back the transaction in this case
                        transaction.Rollback();//may be this is not required in this case
                        throw new InvalidOperationException("TenantId does not exist with user mapping .");
                    }

                    // Update the DefaultTenantId in the Employee table
                    var userToUpdate = await _dbContext.Users.FindAsync(userId);
                    var tenantName = await _dbContext.Tenants.Where(x => x.Id == tenantId).Select(x => x.Name).FirstOrDefaultAsync();
                    if (userToUpdate != null && !string.IsNullOrEmpty(tenantName))
                    {
                        userToUpdate.DefaultTenantId = tenantId;
                        userToUpdate.DefaultTenantName = tenantName;
                        // Mark the property as modified
                        _dbContext.Entry(userToUpdate).Property(e => e.DefaultTenantId).IsModified = true;

                        // Save changes
                        var result = await _dbContext.SaveChangesAsync();

                        // Commit the transaction
                        transaction.Commit();
                        if (result > 0)
                            return IdentityResult.Success;
                    }
                    else
                    {
                        // Handle the case where the employeeId is not found (throw exception, log, etc.)
                        // You might want to roll back the transaction in this case
                        transaction.Rollback();
                        throw new InvalidOperationException("EmployeeId not found.");
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions, log, etc.
                    // You might want to roll back the transaction in case of an exception
                    transaction.Rollback();
                    throw;
                }
            }

        return IdentityResult.Failed();
    }
    public async Task<IdentityResult> CreateWithDefaultRolesAsync(ApplicationUser user, string? tenantId = null, string? password = null)
    {
        return await CreateAsync(user, _defaultRoles, tenantId, password);
    }

    public async Task<int> UpdateIsLive(string userId, bool isLive = false)
    {
        return await UpdateColumn(userId, nameof(ApplicationUser.IsLive), isLive);
    }

    //TODO change it to async
    public async Task<int> UpdateColumn(string userId, string columnName, object newValue)
    {
        //todo instead of this use ExecuteUpdate so no separate read & update is required
        var entity = await _dbContext.Users.FindAsync(userId);

        if (entity != null)
        {
            // Use reflection to set the property value based on the column name
            var propertyInfo = entity.GetType().GetProperty(columnName);

            if (propertyInfo != null)
            {
                var convertedValue = Convert.ChangeType(newValue, propertyInfo.PropertyType);
                propertyInfo.SetValue(entity, convertedValue);
                _dbContext.Entry(entity).State = EntityState.Modified;
                return await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException("Invalid column name.");
            }
        }
        else
        {
            throw new ArgumentException("Entity not found.");
        }
        //}
    }

    //make all return in dto format
    public async Task<ApplicationUser?> FindByIdAsync(string userId, bool isTrackingNeeded = false)
    {
        return await FindByNameOrId(userId: Guid.Parse(userId.TrimSelf()), isTrackingNeeded: isTrackingNeeded);
    }
    public async Task<ApplicationUser?> FindByNameForLocalAccountAsync(string userName, bool isTrackingNeeded = false)
    {//Since local account validation expects hash and all so get it from default.In real time its performance overhead.
     //TODO need to validate which is best in performance

        return await FindByNameAsync(userName, isTrackingNeeded: isTrackingNeeded);
        // return await base.FindByNameAsync(userName);
    }
    public async Task<ApplicationUser?> FindByNameAsync(string userName, bool isTrackingNeeded = false)
    {
        return await FindByNameOrId(userName: userName, isTrackingNeeded: isTrackingNeeded);
    }
    public async Task<List<TenantDto>> GetAllTenants(bool forceLoad = false)
    {
        if (forceLoad || !_tenantService.DataSource.Any())
            return await _dbContext.Tenants.Where(x => x.Active).ProjectTo<TenantDto>(_mapper.ConfigurationProvider).ToListAsync(); ;
        return _tenantService.DataSource;
    }
    public async Task<ApplicationUser?> FindByNameOrId(string userName = "", Guid? userId = null, bool isTrackingNeeded = false)
    {
        if (userName.IsNullOrEmptyAndTrimSelf() && !userId.HasValue) return null;

        string searchCriteria = userName; // Replace with your search criteria
        bool searchById = false; // ToAdd to true if searching by Id, false if searching by UserName


        if (userName.IsNullOrEmptyAndTrimSelf() && userId.HasValue) { searchById = true; searchCriteria = userId?.ToString(); }

        var query = _dbContext.Users
               .Where(user => (searchById && user.Id == searchCriteria) || (!searchById && user.UserName == searchCriteria))
               .Select(user => new ApplicationUser
               {
                   UserName = user.UserName,
                   UserClaims = _dbContext.UserClaims.Where(uc => uc.UserId == user.Id).ToList(),
                   //TODO change below as join operator
                   UserRoleTenants = _dbContext.UserRoles.Where(urt => urt.UserId == user.Id).Select(u => new UserRoleTenant
                   {
                       TenantType = u.Tenant.Type,
                       TenantId = u.TenantId,
                       TenantName = u.Tenant.Name,
                       RoleId = u.RoleId,
                       RoleName = u.Role.Name,
                       RoleLevel = u.Role.Level
                   }).OrderByDescending(x => x.TenantType).ThenByDescending(x => x.RoleLevel).ToList(),
                   Id = user.Id,
                   Email = user.Email,
                   PhoneNumber = user.PhoneNumber,
                   SecurityStamp = user.SecurityStamp,
                   DisplayName = user.DisplayName,
                   Provider = user.Provider,
                   DefaultTenantId = user.DefaultTenantId,
                   DefaultTenantName = user.DefaultTenantName,
                   ProfilePictureDataUrl = user.ProfilePictureDataUrl,
                   IsActive = user.IsActive,
                   IsLive = user.IsLive,
                   RefreshToken = user.RefreshToken,
                   RefreshTokenExpiryTime = user.RefreshTokenExpiryTime,
                   Logins = user.Logins,
                   Tokens = user.Tokens,
                   SuperiorId = user.SuperiorId,
                   Superior = user.Superior//this also can be replaced by superior name kind of
               });
        return await (isTrackingNeeded ? query : query.AsNoTracking()).FirstOrDefaultAsync();

    }

    public async Task<ApplicationUser?> FindByNameOrIdFullUserObject(string userName = "", Guid? userId = null, bool isTrackingNeeded = false)
    {
        if (userName.IsNullOrEmptyAndTrimSelf() && !userId.HasValue) return null;

        string searchCriteria = userName; // Replace with your search criteria
        bool searchById = false; // ToAdd to true if searching by Id, false if searching by UserName

        if (userName.IsNullOrEmptyAndTrimSelf() && userId.HasValue) { searchById = true; searchCriteria = userId?.ToString(); }

        var query = _dbContext.Users
                .Include(x => x.UserClaims)
                .Include(x => x.UserRoleTenants).ThenInclude(x => x.Role)
                .Include(x => x.UserRoleTenants).ThenInclude(x => x.Tenant)
                .Where(user => (searchById && user.Id == searchCriteria) || (!searchById && user.UserName == searchCriteria))
                ;//.FirstOrDefaultAsync();
        return await (isTrackingNeeded ? query : query.AsNoTracking()).FirstOrDefaultAsync();

    }
    public async Task<IdentityResult> UpdateAsync(ApplicationUserDto userDto)
    {
        return await UpdateAsync(_mapper.Map<ApplicationUser>(userDto));
    }
    public override async Task<IdentityResult> UpdateAsync(ApplicationUser user)
    {//here role update not happening,for that separate code after this completion

        // Add your custom logic here before calling the base method
        // For example, you can validate user data or perform additional tasks.
        var existingUserRoleTenants = user?.UserRoleTenants;
        try
        {

            //todo need to verify the perfection
            user.UserRoleTenants = null;//if this not done then tracking issue will block,so this is improtant to keep

            //type working good
            var existingUser = await _dbContext.Users.FindAsync(user.Id);
            _dbContext.Entry(existingUser).CurrentValues.SetValues(user);

            //type1 wont work
            //_dbContext.Attach(user);
            //_dbContext.Entry(user).State=Microsoft.EntityFrameworkCore.EntityState.Modified;

            //type2 wont work
            //var result = _dbContext.Users.Update(user);

            var rrr = await _dbContext.SaveChangesAsync();
            user.UserRoleTenants = existingUserRoleTenants;
            return IdentityResult.Success;


        }
        catch (Exception e)
        {
            if (user != null && existingUserRoleTenants != null)
                user.UserRoleTenants = existingUserRoleTenants;
            Console.WriteLine(e.ToString());
            return IdentityResult.Failed(new IdentityError() { Description = e.ToString() });
        }
    }
    public async Task<IdentityResult> CreateAsync(ApplicationUser user, List<string>? roles = null, string? tenantId = null, string? password = null)
    {
        try
        {
            if (tenantId.IsNullOrEmptyAndTrimSelf()) tenantId = DefaultTenantId;
            if (user.DefaultTenantId.IsNullOrEmptyAndTrimSelf()) user.DefaultTenantId = tenantId;//this overrides already assigned tenant,had to make sure
            if (roles == null || !roles.Any()) return await CreateWithDefaultRolesAsync(user, user.DefaultTenantId, password);
            user.UserRoleTenants = new List<UserRoleTenant>();//here it ignores already existing UserRoleTenants //TODO need to think of this
            roles.ForEach(c =>
            {
                var roleId = (_roleManager.FindByNameAsync(c).Result)?.Id;
                if (roleId != null)
                {
                    user.UserRoleTenants.Add(new UserRoleTenant() { RoleId = roleId, TenantId = user.DefaultTenantId! });
                    /* This is required if default scopes for user level need to assign
                    var scopes = Perms.PermissionsAll.Find(x => x.roleOrType.Equals(c, StringComparison.InvariantCultureIgnoreCase)).permissions;
                    if (scopes != null && scopes.Any())
                        foreach (var scope in scopes)
                        {
                            base.AddClaimAsync(user, new Claim("Permissions", scope));
                        }
                    */
                }
            });
            var result = password.IsNullOrEmptyAndTrimSelf() ? await base.CreateAsync(user) : await base.CreateAsync(user, password!);
            return result;

            //TODO verify this works or not
            //    using var scope = _scopeFactory.CreateScope();
            //_dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            //var result = password.IsNullOrEmptyAndTrimSelf() ? await base.CreateAsync(user) : await base.CreateAsync(user, password!);
            //return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString()); throw;
        }
    }

    //all update for particular current default tenant only
    public async Task<int?> RolesUpdateInsert(ApplicationUser user, IEnumerable<string> roleNames)
    {
        if (user == null || user.DefaultTenantId.IsNullOrEmptyAndTrimSelf() || !Guid.TryParse(user.DefaultTenantId, out Guid tenantId)
            || user.Id.IsNullOrEmptyAndTrimSelf() || !Guid.TryParse(user.Id, out Guid id)) return null;

        if (roleNames == null || roleNames.Count() == 0)//means all roles removed
        {
            var existingAllInCurrentTenant = _dbContext.UserRoleTenants.Include(x => x.Role).Where(role => role.UserId == user.Id && role.TenantId == user.DefaultTenantId).ToList();
            if (existingAllInCurrentTenant != null && existingAllInCurrentTenant.Count > 0)
            {
                return await _dbContext.UserRoleTenants.Where(x => x.TenantId == user.DefaultTenantId)
                    .ExecuteDeleteAsync();
                //return await _dbContext.SaveChangesAsync();//this savechangesasync is not required 
            }
            //else //means already everything removed now nothing to do with role updation
        }
        else
        {
            roleNames = roleNames.Select(x => x.Trim().TrimStart().TrimEnd().ToUpper())
     .Where(str => !str.IsNullOrEmpty()).Distinct()
     .GroupBy(i => i).Select(x => x.Key).ToList();

            var existingAllInCurrentTenant = _dbContext.UserRoleTenants.Include(x => x.Role).Where(role => role.UserId == user.Id && role.TenantId == user.DefaultTenantId);
            //save happens ata time for one tenant,so other tenant will not fetch details
            var existing = existingAllInCurrentTenant.Where(x => roleNames.Contains(x.Role.NormalizedName!)).ToList();
            var changesTriggered = false;
            if (existing.All(x => x.IsActive != user.IsUserTenantRolesActive))//existing update
            {
                existing.ForEach(x =>
                {
                    x.IsActive = user.IsUserTenantRolesActive;
                    _dbContext.UserRoles.Attach(x);
                    _dbContext.Entry(x).State = EntityState.Modified;
                });
                changesTriggered = true;
            }

            var toInsert = roleNames.Except(existingAllInCurrentTenant.Select(x => x.Role.NormalizedName)).ToList();
            var toRemove = existingAllInCurrentTenant.Select(x => x.Role.NormalizedName).ToList().Except(roleNames).ToList();

            if (toInsert.Count != 0)
            {
                var toAdd = new List<UserRoleTenant>();
                toInsert.ForEach(x =>
                {
                    var roleId = (_dbContext.Roles.FirstOrDefault(r => r.NormalizedName == x!.ToUpper()))?.Id;
                    if (string.IsNullOrEmpty(roleId)) return;
                    toAdd.Add(new UserRoleTenant() { UserId = user.Id, TenantId = user.DefaultTenantId, RoleId = roleId });
                });
                await _dbContext.UserRoles.AddRangeAsync(toAdd);
                changesTriggered = true;
            }
            if (toRemove.Count != 0)
            {
                _dbContext.UserRoles.RemoveRange(existingAllInCurrentTenant.Where(x => toRemove.Contains(x.Role.NormalizedName)));
                changesTriggered = true;
            }
            return changesTriggered ? await _dbContext.SaveChangesAsync() : 0;
        }

        return 0;
    }
    public override async Task<IdentityResult> RemoveFromRoleAsync(ApplicationUser user, string roleName)
    {
        if (string.IsNullOrEmpty(roleName) || user == null || string.IsNullOrEmpty(user.DefaultTenantId) || !Guid.TryParse(user.DefaultTenantId, out Guid id1)
            || string.IsNullOrEmpty(user.Id) || !Guid.TryParse(user.Id, out Guid id)) return IdentityResult.Failed();
        roleName = roleName.ToUpperInvariant();
        var existing = _dbContext.UserRoles.Where(role => role.TenantId == user.DefaultTenantId && role.Role.Name == roleName);
        _dbContext.UserRoles.RemoveRange(existing);
        return await _dbContext.SaveChangesAsync() > 0 ? IdentityResult.Success : IdentityResult.Failed();

    }

    public override async Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string roleName)
    {
        if (string.IsNullOrEmpty(roleName) || user == null || string.IsNullOrEmpty(user.DefaultTenantId) || !Guid.TryParse(user.DefaultTenantId, out Guid id1)
             || string.IsNullOrEmpty(user.Id) || !Guid.TryParse(user.Id, out Guid id)) return IdentityResult.Failed();
        roleName = roleName.ToUpperInvariant();
        var roleId = (await _dbContext.Roles.FirstOrDefaultAsync(x => x.NormalizedName == roleName.ToUpper()))?.Id;
        if (string.IsNullOrEmpty(roleId)) return IdentityResult.Failed();
        var newInserted = _dbContext.UserRoles
            .AddAsync(new UserRoleTenant() { UserId = user.Id, TenantId = user.DefaultTenantId, RoleId = roleId });
        return await _dbContext.SaveChangesAsync() > 0 ? IdentityResult.Success : IdentityResult.Failed();
    }

    //TODO madhu continue here
    public IdentityResult ChangeUserDefaultTenant(string userId, string tenantId)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(tenantId) || !Guid.TryParse(tenantId, out Guid id1))
            return IdentityResult.Failed();
        return IdentityResult.Success;
        //roleName = roleName.ToUpperInvariant();
        //var roleId = (await _dbContext.Roles.FirstOrDefaultAsync(x => x.NormalizedName == roleName.ToUpper()))?.Id;
        //if (string.IsNullOrEmpty(roleId)) return IdentityResult.Failed();
        //var newInserted = _dbContext.UserRoles
        //    .AddAsync(new ApplicationUserRoleTenant() { UserId = user.Id, DefaultTenantId = user.DefaultTenantId, RoleId = roleId });
        //return await _dbContext.SaveChangesAsync() > 0 ? IdentityResult.Success : IdentityResult.Failed();
    }
    public async Task<IList<UserRoleTenant>> CheckUserHasTenantRolePermission(string userId, string? roleId = null, string? tenantId = null)
    {
        var query = _dbContext.UserRoles.AsQueryable();
        if (!string.IsNullOrEmpty(tenantId))
            query = query.Where(role => role.TenantId == tenantId);
        if (!string.IsNullOrEmpty(roleId))
            query = query.Where(role => role.RoleId == roleId);
        if (!string.IsNullOrEmpty(userId))
            query = query.Where(role => role.UserId == userId);
        return await query.ToListAsync();
    }

    //public List<TenantDto> GetAllowedTenants(string userTenantId)
    //{
    //    var userTenant = DataSource.Find(x => x.Id == userTenantId);
    //    if (userTenant != null)
    //    {
    //        var lessOrEquivalentTenants = DataSource.Where(x => x.Type <= userTenant.Type).ToList();
    //    }
    //    return null;
    //}
    //public List<TenantDto> GetAllowedTenants(string userId)
    //{
    //    //get user profile by iuserid & return
    //    //return GetAllowedTenants(_mapper.Map<ApplicationUserDto>(user));//todo this is not working mapping
    //    var dto = new ApplicationUserDto()
    //    {
    //        DefaultTenantId = user.DefaultTenantId //,DefaultRole=user.
    //        , Id = user.Id
    //        , UserRoleTenants = _mapper.Map<List<ApplicationUserRoleTenantDto>>(user.UserRoleTenants)
    //    };
    //    return GetAllowedTenants(dto);
    //}
    public IQueryable<ApplicationUser> GetUsersWithDynamicConditions(Dictionary<string, object> conditions)
    {
        //TODO

        IQueryable<ApplicationUser> query = _dbContext.Users;

        // Build dynamic conditions based on the dictionary
        foreach (var condition in conditions)
        {
            switch (condition.Key)
            {
                case "UserName":
                    query = query.Where(u => u.UserName == (string)condition.Value);
                    break;
                    // Add more conditions as needed...
            }
        }

        return query;

    }
    public async Task<List<(string TenantId, string TenantName)>> GetMyTenants(string userId)
    {

        var result = (await _dbContext.UserRoleTenants//.Include(x => x.Tenant)
            .Where(x => x.UserId == userId && x.IsActive)
           .Select(ur => new { ur.TenantId, ur.Tenant.Name })
           .Distinct()
            .ToListAsync()).Select(r => (r.TenantId, r.Name)).ToList();
        return result;

    }
    public List<TenantDto> GetAllowedTenants(ApplicationUser user)
    {
        //return GetAllowedTenants(_mapper.Map<ApplicationUserDto>(user));//todo this is not working mapping
        var dto = new ApplicationUserDto()
        {
            DefaultTenantId = user.DefaultTenantId //,DefaultRole=user.
            , Id = user.Id
            , UserRoleTenants = _mapper.Map<List<UserRoleTenantDto>>(user.UserRoleTenants)
        };
        return GetAllowedTenants(dto);
    }
    public List<TenantDto> GetAllowedTenants(ApplicationUserDto userDto)
    {
        //if no mapping exists
        if (userDto.UserRoleTenants == null || userDto.UserRoleTenants.Count == 0)
        {
            if (!userDto.DefaultTenantId.IsNullOrEmpty())
                return [_tenantService.DataSource.FirstOrDefault(x => x.Id == userDto.DefaultTenantId)];
            return null;
        }

        foreach (var t in userDto.UserRoleTenants)
        {
            t.Tenant = _tenantService.DataSource.Find(d => d.Id == t.TenantId);
        }

        //internal user,give all tenants
        if (userDto.DefaultRole == RoleNamesEnum.ROOTADMIN.ToString() || //ned to think more
            userDto.UserRoleTenants.Any(x => x.RoleName == RoleNamesEnum.ROOTADMIN.ToString()
            || userDto.UserRoleTenants.Any(x => x.Tenant != null && x.Tenant.Type == (byte)TenantTypeEnum.Internal)))
        {
            return _tenantService.DataSource;
        }

        //all other users their tenat + created/approved/modified
        var userTenantIds = userDto.UserRoleTenants.Select(t => t.TenantId);

        var myTenants = _tenantService.DataSource.Where(x => userTenantIds.Contains(x.Id))
            //.OrderByDescending(x => x.Type)//already datasource is in sorted order no need to sort again
            .ToList();
        var myApprovedTenants = _tenantService.DataSource.Where(x => x.CreatedByUser == userDto.Id || x.ApprovedByUser == userDto.Id || x.ModifiedLastByUser == userDto.Id)
            //.OrderByDescending(x => x.Type)//already datasource is in sorted order no need to sort again
            .ToList();
        var result = new List<TenantDto>();
        if (myTenants.Count != 0)
            result.AddRange(myTenants);

        if (myApprovedTenants.Count != 0)
            result.AddRange(myApprovedTenants);
        return result.DistinctBy(x => x.Id).ToList();
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