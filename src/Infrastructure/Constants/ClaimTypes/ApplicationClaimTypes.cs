// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Infrastructure.Constants.ClaimTypes;

public static class ApplicationClaimTypes
{//all are for logged in user account self details
    public const string Provider = "Provider";
    public const string DefaultTenantId = "DefaultTenantId";
    public const string DefaultTenantName = "DefaultTenantName";
    public const string UserRoleTenants = "UserRoleTenants";
    public const string SuperiorId = "SuperiorId";
    public const string SuperiorName = "SuperiorName";
    public const string Status = "Status";

    public const string Permission = "Permission";
    public const string AssignedRoles = "AssignedRoles";
    public const string ProfilePictureDataUrl = "ProfilePictureDataUrl";

    public const string MyVote = "MyVote";//for voting_system
}