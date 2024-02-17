// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; set; }
    string? UserName { get; set; }
    ICollection<UserRoleTenantDto>? UserRoleTenants { get; set; } 
    string? DefaultTenantId { get; set; }
    string? DefaultTenantName { get; set; }
}