﻿using CleanArchitecture.Blazor.Application.Common.Security;
using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;
using CleanArchitecture.Blazor.Domain.Enums;
using PublicCommon;

namespace CleanArchitecture.Blazor.Server.UI.Fluxor;

[FeatureState]
public class UserProfileState
{
    public UserProfileState()
    {
        IsLoading = true;
        UserProfile = new UserProfile { Email = "", UserId = "", UserName = "" };
    }

    public UserProfileState(bool loading, UserProfile? userProfile)
    {
        IsLoading = loading;
        UserProfile = userProfile ?? new UserProfile { Email = "", UserId = "", UserName = "" };
    }

    public UserProfileState(ApplicationUserDto dto)
    {
        UserProfile = new UserProfile
        {
            UserId = dto.Id,
            ProfilePictureDataUrl = dto.ProfilePictureDataUrl,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            DisplayName = dto.DisplayName,
            Provider = dto.Provider,
            UserName = dto.UserName,
            IsActive = dto.IsActive,
            DefaultTenantId = dto.DefaultTenantId,
            DefaultTenantName = dto.DefaultTenantName,
            SuperiorId = dto.SuperiorId,
            SuperiorName = dto.SuperiorName,
            AssignedRoles = dto.AssignedRoles,
            DefaultRole = dto.DefaultRole//.AssignedRoles.MaxEnumString<RoleNamesEnum>() //dto.DefaultRole
            ,UserRoleTenants=dto.UserRoleTenants
        };
        //if (dto.UserRoleTenants != null && dto.UserRoleTenants.Any())
        //{
        //    UserProfile.AssignedRoles = dto.UserRoleTenants.Where(x => x.DefaultTenantId == dto.DefaultTenantId).Select(x => x.RoleName).ToList().ToArray();
        //    UserProfile.DefaultRole = UserProfile.AssignedRoles.First();
        //}
    }

    public UserProfile UserProfile { get; }
    public bool IsLoading { get; }
}