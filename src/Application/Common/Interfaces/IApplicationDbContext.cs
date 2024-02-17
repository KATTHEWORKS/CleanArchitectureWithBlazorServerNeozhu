//#define HOSPITAL_SYSTEM
#define VOTING_SYSTEM

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using CleanArchitecture.Blazor.Domain.Identity;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    //DatabaseFacade Database { get; set; }
    //EntityEntry<TEntity> Entry<TEntity>(TEntity entity);
    DbSet<ApplicationUser> Users { get; set; }
    DbSet<UserRoleTenant> UserRoleTenants { get; set; }
    // DbSet<ApplicationRole> Roles { get; set; }
    DbSet<Logger> Loggers { get; set; }
    DbSet<AuditTrail> AuditTrails { get; set; }
    DbSet<Document> Documents { get; set; }
    DbSet<KeyValue> KeyValues { get; set; }
    DbSet<Product> Products { get; set; }
    DbSet<Tenant> Tenants { get; set; }
    DbSet<TenantPending> TenantPending { get; set; }
    DbSet<Customer> Customers { get; set; }

#if VOTING_SYSTEM
    public DbSet<V_Constituency> V_Constituencies { get; set; }
    public DbSet<V_Vote> V_Votes { get; set; }
    public DbSet<V_VoteSummary> V_VoteSummarys { get; set; }
#endif

    ChangeTracker ChangeTracker { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task<int> SaveChangesAsync();
}
