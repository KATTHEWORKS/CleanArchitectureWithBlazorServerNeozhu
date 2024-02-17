// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Security.Claims;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity.DTOs;
using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;
using CleanArchitecture.Blazor.Application.Features.Tenants.DTOs;
using CleanArchitecture.Blazor.Domain.Identity;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;

public interface IIdentityService : IService
{
    Task<List<ApplicationRoleDto>> GetAllRoles();

    //Task<Result<TokenResponse>> LoginAsync(TokenRequest request, CancellationToken cancellation = default);
    //Task<TokenResponse> GenerateJwtAsync(ApplicationUser user, bool rememberMe = false);
    //Task<Result<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellation = default);
    //Task<ClaimsPrincipal> GetClaimsPrincipal(string token);
    Task<string?> GetUserNameAsync(string userId, CancellationToken cancellation = default);
    Task<bool> IsInRoleAsync(string userId, string role, CancellationToken cancellation = default);
    Task<bool> AuthorizeAsync(string userId, string policyName, CancellationToken cancellation = default);
    Task<Result> DeleteUserAsync(string userId, CancellationToken cancellation = default);
    Task<IDictionary<string, string?>> FetchUsers(string roleName, CancellationToken cancellation = default);
    Task UpdateLiveStatus(string userId, bool isLive, CancellationToken cancellation = default);
    Task<ApplicationUserDto> GetUser(string userId, CancellationToken cancellation = default, bool isTrackingNeeded = false);
    Task<ApplicationUserDto> GetUserDtoByUserName(string userName, CancellationToken cancellation = default);

    //Production this will be removed,now temp
    Task<ApplicationUser> GetUserByUserName(string userName, CancellationToken cancellation = default);
    string GetUserName(string userId);
    Task<List<ApplicationUserDto>?> GetUsersByTenantId(string? tenantId, CancellationToken cancellation = default);

    Task<IdentityResult> UpdateAsync(ApplicationUserDto userDto);

    Task<IdentityResult> CreateUserAsync(ApplicationUserDto userDto, CancellationToken cancellation = default);
    Task<IdentityResult> AddLoginAsync(ApplicationUser user, UserLoginInfo userLoginInfo);
    Task<string> GeneratePasswordResetTokenAsync(ApplicationUser item);
    Task<IdentityResult> ResetPasswordAsync(ApplicationUser item, string? token, string? password);

    Task<TenantDto> GetUserTenants(string userId, CancellationToken cancellation = default);
}
