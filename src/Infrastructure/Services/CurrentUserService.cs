// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;

namespace CleanArchitecture.Blazor.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public ICollection<UserRoleTenantDto>? UserRoleTenants { get; set; }
    public string? DefaultTenantId { get; set; }
    public string? DefaultTenantName { get; set; }
}