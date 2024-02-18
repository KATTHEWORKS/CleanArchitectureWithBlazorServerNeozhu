// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using AutoMapper;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;
using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;
using CleanArchitecture.Blazor.Domain.Identity;
using CleanArchitecture.Blazor.Infrastructure.Configurations;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity.DTOs;
using CleanArchitecture.Blazor.Application.Features.Identity.Notification;
using CleanArchitecture.Blazor.Application.Features.Tenants.DTOs;
using CleanArchitecture.Blazor.Infrastructure.Extensions;
using LazyCache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using System.Linq.Dynamic.Core.Tokenizer;
using Newtonsoft.Json.Linq;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Identity;

public class IdentityService : IIdentityService
{
    private readonly CustomUserManager _userManager;
    private readonly CustomRoleManager _roleManager;
    private readonly AppConfigurationSettings _appConfig;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;
    private readonly IAppCache _cache;
    private readonly IStringLocalizer<IdentityService> _localizer;
    private readonly IMapper _mapper;

    public IdentityService(
        IServiceScopeFactory scopeFactory,
        IApplicationSettings appConfig,
        IAppCache cache,
        IMapper mapper,
        IStringLocalizer<IdentityService> localizer)
    {
        var scope = scopeFactory.CreateScope();
        _userManager = scope.ServiceProvider.GetRequiredService<CustomUserManager>();
        _roleManager = scope.ServiceProvider.GetRequiredService<CustomRoleManager>();
        _userClaimsPrincipalFactory = scope.ServiceProvider.GetRequiredService<IUserClaimsPrincipalFactory<ApplicationUser>>();
        _authorizationService = scope.ServiceProvider.GetRequiredService<IAuthorizationService>();
        _cache = cache;
        _mapper = mapper;
        _localizer = localizer;
        _appConfig = appConfig;
    }

    private TimeSpan RefreshInterval => TimeSpan.FromSeconds(60);

    private LazyCacheEntryOptions Options =>
        new LazyCacheEntryOptions().SetAbsoluteExpiration(RefreshInterval, ExpirationMode.LazyExpiration);

    public async Task<List<ApplicationRoleDto>> GetAllRoles()
    {
        var key = $"{nameof(GetAllRoles)}";
        var roles = await _cache.GetOrAddAsync(key, async () => await _roleManager.Roles.OrderByDescending(r => r.Level).ProjectTo<ApplicationRoleDto>(_mapper.ConfigurationProvider).ToListAsync());
        return roles;
    }
    public async Task<IDictionary<string, string?>> FetchUsers(string roleName,
    CancellationToken cancellation = default)
    {
        if (string.IsNullOrEmpty(roleName)) return null;
        roleName = roleName.ToUpperInvariant();
        var result = await _userManager.Users
             .Where(x => x.UserRoleTenants.Any(y => y.Role.NormalizedName == roleName))
             .Include(x => x.UserRoleTenants)
             .ToDictionaryAsync(x => x.UserName!, y => y.DisplayName, cancellation);
        return result;
    }
    public async Task<string?> GetUserNameAsync(string userId, CancellationToken cancellation = default)
    {
        var key = $"{nameof(GetUserNameAsync)}:{userId}";
        var user = await _cache.GetOrAddAsync(key,
            async () => await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId), Options);
        return user?.UserName;
    }

    public string GetUserName(string userId)
    {
        var key = $"{nameof(GetUserName)}-byId:{userId}";
        var user = _cache.GetOrAdd(key, () => _userManager.Users.SingleOrDefault(u => u.Id == userId), Options);
        return user?.UserName ?? string.Empty;
    }

    public async Task<bool> IsInRoleAsync(string userId, string role, CancellationToken cancellation = default)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellation) ??
                   throw new NotFoundException(_localizer["User Not Found."]);
        return await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName, CancellationToken cancellation = default)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellation) ??
                   throw new NotFoundException(_localizer["User Not Found."]);
        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);
        var result = await _authorizationService.AuthorizeAsync(principal, policyName);
        return result.Succeeded;
    }

    public async Task<Result> DeleteUserAsync(string userId, CancellationToken cancellation = default)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellation) ??
                   throw new NotFoundException(_localizer["User Not Found."]);
        var result = await _userManager.DeleteAsync(user);
        return result.ToApplicationResult();
    }

    public async Task UpdateLiveStatus(string userId, bool isLive, CancellationToken cancellation = default)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId && x.IsLive != isLive);
        if (user is not null)
        {
            user.IsLive = isLive;
            var result = await _userManager.UpdateAsync(user);
        }
    }
    public async Task<IdentityResult> CreateUserAsync(ApplicationUserDto userDto, CancellationToken cancellation = default)
    {
        return await _userManager.CreateAsync(_mapper.Map<ApplicationUser>(userDto));
    }
    //public async Task<IdentityResult> AddLoginAsync(ApplicationUserDto userDto, UserLoginInfo userLoginInfo)
    //{//not sure this works or not with DTO,bcz this is not derviced from IdentityResult claims,principles all
    //    return await _userManager.AddLoginAsync(_mapper.Map<ApplicationUser>(userDto), userLoginInfo);
    //}

    //ideally this should not be but no option so keeping
    public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, CancellationToken cancellation = default)
    {
        return await _userManager.CreateAsync(user);
    }
    public async Task<IdentityResult> AddLoginAsync(ApplicationUser user, UserLoginInfo userLoginInfo)
    {
        return await _userManager.AddLoginAsync(user, userLoginInfo);
    }
    public async Task<IdentityResult> UpdateAsync(ApplicationUserDto userDto)
    {
        return await _userManager.UpdateAsync(_mapper.Map<ApplicationUser>(userDto));
    }
    public async Task<string> GeneratePasswordResetTokenAsync(ApplicationUser item)
    {
        return await _userManager.GeneratePasswordResetTokenAsync(item);
    }
    public async Task<IdentityResult> ResetPasswordAsync(ApplicationUser item, string? token, string? password)
    {
        return await _userManager.ResetPasswordAsync(item, token, password!);
    }

    //this needs update handle in case user updated the user then this has to be addressed
    public async Task<ApplicationUserDto> GetUser(string userId, CancellationToken cancellation = default, bool isTrackingNeeded = false)
    {
        //var ty = (await _userManager.FindByIdAsync(userId));
        //var rr=_mapper.Map<ApplicationUserDto>(ty);

        var key = $"{nameof(GetUser)}:{userId}";
        //var result = await _cache.GetOrAddAsync(key,
        //    async () =>await _userManager.Users.Where(x => x.Id == userId).Include(x => x.UserRoleTenants)
        //        .ThenInclude(x => x.Role)
        //        .ProjectTo<ApplicationUserDto>(_mapper.ConfigurationProvider)
        //        .FirstOrDefaultAsync(cancellation) ?? new ApplicationUserDto(), Options);
        var result = await _cache.GetOrAddAsync(key,
            async () => _mapper.Map<ApplicationUserDto>(await _userManager.FindByIdAsync(userId, isTrackingNeeded: isTrackingNeeded)), Options);
        return result;
    }
    public async Task<ApplicationUserDto> GetUserDtoByUserName(string userName, CancellationToken cancellation = default)
    {
        var key = $"{nameof(GetUserDtoByUserName)}:{userName}";
        var result = await _cache.GetOrAddAsync(key,
            async () => _mapper.Map<ApplicationUserDto>(await _userManager.FindByNameAsync(userName)), Options);
        return result;
    }

    //in production this will be removed
    public async Task<ApplicationUser> GetUserByUserName(string userName, CancellationToken cancellation = default)
    {
        var key = $"{nameof(GetUserByUserName)}:{userName}";
        var result = await _cache.GetOrAddAsync(key,
            async () => await _userManager.FindByNameAsync(userName), Options);
        return result;
    }

    public async Task<List<ApplicationUserDto>?> GetUsersByTenantId(string? tenantId, CancellationToken cancellation = default)
    {
        //TODO add pagination, change logic as map with tenant id & name kind of as per customusermanagerrole
        var key = $"{nameof(GetUsersByTenantId)}:{tenantId}";
        Func<string?, CancellationToken, Task<List<ApplicationUserDto>?>> getUsersByTenantId = async (tenantId, token) =>
        {
            if (string.IsNullOrEmpty(tenantId))
            {//TODO this should not be in real time ,it explodes the system
                return await _userManager.Users.Take(20).Include(x => x.UserRoleTenants).ThenInclude(x => x.Role)
                       .ProjectTo<ApplicationUserDto>(_mapper.ConfigurationProvider).ToListAsync();
            }
            else
            {
                return await _userManager.Users.Where(x => x.DefaultTenantId == tenantId).Include(x => x.UserRoleTenants).ThenInclude(x => x.Role)
                      .ProjectTo<ApplicationUserDto>(_mapper.ConfigurationProvider).ToListAsync();
            }
        };
        var result = await _cache.GetOrAddAsync(key, () => getUsersByTenantId(tenantId, cancellation), Options);
        return result;
    }


    public async Task<TenantDto> GetUserTenants(string userId, CancellationToken cancellation = default)
    {
        var key = $"{nameof(GetUserTenants)}:{userId}";
        var result = await _cache.GetOrAddAsync(key, async () => await _userManager.Users.Where(x => x.Id == userId).Select(x => x.UserRoleTenants.Select(ur => ur.Tenant)).ProjectTo<TenantDto>(_mapper.ConfigurationProvider).FirstAsync(cancellation), Options);
        return result;
    }
}