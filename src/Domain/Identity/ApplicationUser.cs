#define VOTING_SYSTEM
//#define HOSPITAL_SYSTEM
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace CleanArchitecture.Blazor.Domain.Identity;

public class ApplicationUser : IdentityUser
{
    public ApplicationUser() : base()
    {
        UserClaims = new HashSet<ApplicationUserClaim>();
        UserRoleTenants = new HashSet<UserRoleTenant>();
        Logins = new HashSet<ApplicationUserLogin>();
        Tokens = new HashSet<ApplicationUserToken>();
    }


    public string? DisplayName { get; set; }
    public string? Provider { get; set; } = "Local";
    public string DefaultTenantId { get; set; }
    public string DefaultTenantName { get; set; }
    public virtual Tenant DefaultTenant { get; set; }

    [Column(TypeName = "text")] public string? ProfilePictureDataUrl { get; set; }

    public bool IsActive { get; set; }
    public bool IsLive { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public virtual ICollection<ApplicationUserClaim> UserClaims { get; set; }
    public virtual ICollection<UserRoleTenant> UserRoleTenants { get; set; } = new List<UserRoleTenant>();

    [Description("Is User-Tenant Roles Active")]
    [NotMapped]
    public bool IsUserTenantRolesActive { get; set; } = true;
    public virtual ICollection<ApplicationUserLogin> Logins { get; set; }
    public virtual ICollection<ApplicationUserToken> Tokens { get; set; }

    public string? SuperiorId { get; set; } = null;

    public ApplicationUser? Superior { get; set; } = null;

#if VOTING_SYSTEM
    //[NotMapped]
    //public V_Vote? MyVote { get; set; }
#endif
}
