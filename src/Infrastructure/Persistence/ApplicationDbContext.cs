//#define HOSPITAL_SYSTEM
#define VOTING_SYSTEM

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using CleanArchitecture.Blazor.Domain.Common.Entities;
using CleanArchitecture.Blazor.Domain.Identity;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence;

#nullable disable
//public class ApplicationDbContext : IdentityDbContext<
//    ApplicationUser, ApplicationRole, string,
//    ApplicationUserClaim, UserRoleTenant, ApplicationUserLogin,
//    ApplicationRoleClaim, ApplicationUserToken>, IApplicationDbContext
public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, ApplicationUserClaim, UserRoleTenant, ApplicationUserLogin, ApplicationRoleClaim, ApplicationUserToken>, IApplicationDbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options
        ) : base(options)
    {

    }
    public DbSet<UserRoleTenant> UserRoleTenants { get; set; }

    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantPending> TenantPending { get; set; }
    public DbSet<Logger> Loggers { get; set; }
    public DbSet<AuditTrail> AuditTrails { get; set; }


    public DbSet<KeyValue> KeyValues { get; set; }

    public DbSet<Document> Documents { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Product> Products { get; set; }

#if VOTING_SYSTEM
    public DbSet<V_Constituency> V_Constituencies { get; set; }
    public DbSet<V_Vote> V_Votes { get; set; }
    //public DbSet<V_CommentSupportOppose> V_CommentSupportOpposeCounts { get; set; }

    public DbSet<V_VoteSummary> V_VoteSummarys { get; set; }
#endif
    public async Task<int> SaveChangesAsync()
    {
        return await base.SaveChangesAsync(CancellationToken.None);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.ApplyGlobalFilters<ISoftDelete>(s => s.Deleted == null);

        builder.Entity<UserRoleTenant>()
            .HasKey(ur => new { ur.UserId, ur.RoleId, ur.TenantId });

        //    builder.Entity<UserRoleTenant>()
        //.HasIndex(ur => new { ur.UserId, ur.RoleId, ur.TenantId })
        //.IsUnique()
        //.HasDatabaseName("IX_ApplicationUserRoles_UserId_RoleId_TenantId")
        //.HasAnnotation("SqlServer:IndexPrefixLength", 900);

        builder.Entity<UserRoleTenant>()
        .HasOne(ur => ur.Tenant)
        .WithMany()
        .HasForeignKey(ur => ur.TenantId)
        .OnDelete(DeleteBehavior.Restrict); // or DeleteBehavior.NoAction, depending on your requirements

#if VOTING_SYSTEM

        builder.Entity<V_Constituency>().Property(b => b.Id).ValueGeneratedOnAdd();
        //https://learn.microsoft.com/en-us/ef/core/modeling/generated-properties?tabs=fluent-api
        builder.Entity<V_Vote>().Property(b => b.Id).ValueGeneratedOnAdd();
        builder.Entity<V_Vote>().Property(b => b.Created).HasDefaultValueSql("getdate()");
        //for updatetime had to use triggers

        builder.Entity<V_VoteSummary>().HasKey(x => x.ConstituencyId);

        builder.Entity<V_VoteSummary>().Property(b => b.Created).HasDefaultValueSql("getdate()");

        builder.Entity<V_VoteSummary>()
            .HasOne(x => x.Constituency)
            .WithOne(c => c.Summary)
            .HasForeignKey<V_VoteSummary>(x => x.ConstituencyId);
#else
        builder.Ignore<V_Vote>();
        builder.Ignore<V_Constituency>();
        builder.Ignore<V_VoteSummary>();
#endif
        /*
        builder.Entity<ApplicationRole>()
             .HasKey(r => new { r.Id, r.TenantType });

        builder.Entity<ApplicationRoleClaim>()
    .HasOne<ApplicationRole>()
    .WithMany()
    .HasForeignKey(rc => rc.RoleId);*/
    }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.LogTo(Console.WriteLine);
        if (!optionsBuilder.IsConfigured)
        {
        }
    }
}