// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Common.Security;
using CleanArchitecture.Blazor.Infrastructure.Constants.ClaimTypes;
using CleanArchitecture.Blazor.Infrastructure.Constants.Role;

using CleanArchitecture.Blazor.Domain.Enums;
namespace CleanArchitecture.Blazor.Infrastructure.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static UserProfile GetUserProfileFromClaim(this
        ClaimsPrincipal claimsPrincipal
           )
    {
        var profile = new UserProfile() { Email = "", UserId = "", UserName = "" };
        if (claimsPrincipal.Identity?.IsAuthenticated ?? false)
        {
            profile.UserId = claimsPrincipal.GetUserId() ?? "";
            profile.UserName = claimsPrincipal.GetUserName() ?? "";
            profile.DefaultTenantName = claimsPrincipal.GetTenantName();
            profile.DefaultTenantId = claimsPrincipal.GetDefaultTenantId();
            profile.PhoneNumber = claimsPrincipal.GetPhoneNumber();
            profile.SuperiorName = claimsPrincipal.GetSuperiorName();
            profile.SuperiorId = claimsPrincipal.GetSuperiorId();
            profile.Email = claimsPrincipal.GetEmail() ?? "";
            profile.DisplayName = claimsPrincipal.GetDisplayName();
            profile.AssignedRoles = claimsPrincipal.GetRoles();//of all tenant
            profile.UserRoleTenants = claimsPrincipal.GetUserRoleTenants();
            profile.DefaultRole = profile.UserRoleTenants.Where(x => x.TenantId == profile.DefaultTenantId).Select(x => x.RoleName).MaxEnumString<RoleNamesEnum>();
            profile.ProfilePictureDataUrl = claimsPrincipal.GetProfilePictureDataUrl();
            profile.IsActive = true;

        }
        return profile;
    }

    /*
     * // You can use the HttpContextAccessor to access the HttpContext
    private HttpContext HttpContext => HttpContextAccessor.HttpContext;

    // Your method to get user information
    private string GetUserId() => HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    private string GetUserName() => HttpContext.User.Identity?.Name;
    private string GetUserEmail() => HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
     */
    public static bool IsAuthenticated(this AuthenticationState? state)
    {
        return state != null && state.User != null && state.User.Identity != null && state.User.Identity.IsAuthenticated;
    }
    public static string? GetUserId(this AuthenticationState? state)
    {
        if (state!= null && state.IsAuthenticated())
        {
            return state.User.GetUserId();
        }
        return null;
    }

    public static string? GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
    }
    public static string? GetEmail(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ClaimTypes.Email);
    }

    public static string? GetPhoneNumber(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ClaimTypes.MobilePhone);
    }



    public static string? GetUserName(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ClaimTypes.Name);
    }

    public static string? GetProvider(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ApplicationClaimTypes.Provider);
    }

    public static string? GetDisplayName(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ClaimTypes.GivenName);
    }

    public static string? GetProfilePictureDataUrl(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ApplicationClaimTypes.ProfilePictureDataUrl);
    }

    public static string? GetSuperiorName(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ApplicationClaimTypes.SuperiorName);
    }

    public static string? GetSuperiorId(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ApplicationClaimTypes.SuperiorId);
    }

    public static string? GetTenantName(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ApplicationClaimTypes.DefaultTenantName);
    }

    public static string? GetDefaultTenantId(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.FindFirstValue(ApplicationClaimTypes.DefaultTenantId);
    //public static string? GetUserTenantRoles(this ClaimsPrincipal claimsPrincipal)
    //    => claimsPrincipal.FindFirstValue(ApplicationClaimTypes.UserRoleTenants);
    public static ICollection<UserRoleTenantDto>? GetUserRoleTenants(this ClaimsPrincipal claimsPrincipal)
    {
        //var ex = claimsPrincipal.FindFirstValue(ApplicationClaimTypes.UserRoleTenants);
        //return string.IsNullOrEmpty(ex) ? null : JsonConvert.DeserializeObject<ICollection<UserRoleTenantDto>>(ex);
        return JsonExtensions.TryDeserialize(claimsPrincipal.FindFirstValue(ApplicationClaimTypes.UserRoleTenants), out ICollection<UserRoleTenantDto> result)
            ? result : null;
    }
    public static bool GetStatus(this ClaimsPrincipal claimsPrincipal)
    {
        return Convert.ToBoolean(claimsPrincipal.FindFirstValue(ApplicationClaimTypes.Status));
    }

    public static string? GetAssignRoles(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirstValue(ApplicationClaimTypes.AssignedRoles);
    }

    public static string[] GetRoles(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToArray();
    }

}