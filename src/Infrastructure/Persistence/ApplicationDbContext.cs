//#define HOSPITAL_SYSTEM
#define VOTING_SYSTEM

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using CleanArchitecture.Blazor.Domain.Common.Entities;
using CleanArchitecture.Blazor.Domain.Identity;
using System.Text.Json;
using CleanArchitecture.Blazor.Domain.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using CleanArchitecture.Blazor.Domain.Entities.VotingSystem;

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
    public DbContext Instance => this;
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
    public DbSet<VoteConstituency> VoteConstituencies { get; set; }
    public DbSet<Vote> Votes { get; set; }
    public DbSet<VoteSummary> VoteSummaries { get; set; }


    //had to remove below 3
    //public DbSet<V_Constituency> V_Constituencies { get; set; }
    //public DbSet<V_Vote> V_Votes { get; set; }
    ////public DbSet<V_CommentSupportOppose> V_CommentSupportOpposeCounts { get; set; }

    //public DbSet<V_VoteSummary> V_VoteSummarys { get; set; }
#endif

    public async Task AddEntityAsync<TEntity>(TEntity entity) where TEntity : class
    {
        Entry(entity).State = EntityState.Added;
        await AddAsync(entity);
    }
    public void UpdateEntity<TEntity>(TEntity entity) where TEntity : class
    {
        Attach(entity);
        Entry(entity).State = EntityState.Modified;
        Update(entity);
    }
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        //var entries = ChangeTracker
        //.Entries()
        //.Where(e => e.Entity is BaseEntity && (
        //        e.State == EntityState.Added
        //        || e.State == EntityState.Modified));

        //foreach (var entityEntry in entries)
        //{
        //    ((BaseEntity)entityEntry.Entity).UpdatedDate = DateTime.Now;

        //    if (entityEntry.State == EntityState.Added)
        //    {
        //        ((BaseEntity)entityEntry.Entity).CreatedDate = DateTime.Now;
        //    }
        //}

        //return base.SaveChanges();
        return await base.SaveChangesAsync(cancellationToken);
    }

    // Additional methods
    public override EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class
    {
        return base.Entry(entity);
    }

    public override DatabaseFacade Database => base.Database;
  
    public async Task BeginTransactionAsync()
    {
        if (Database.CurrentTransaction == null)
        {
            await Database.BeginTransactionAsync();
        }
        else
        {
            throw new InvalidOperationException("A transaction is already in progress.");
        }
    }
    public async Task CommitTransactionAsync()
    {
        if (Database.CurrentTransaction != null)
        {
            await Database.CurrentTransaction.CommitAsync();
        }
        else
        {
            throw new InvalidOperationException("No transaction is in progress.");
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (Database.CurrentTransaction != null)
        {
            await Database.CurrentTransaction.RollbackAsync();
        }
        else
        {
            throw new InvalidOperationException("No transaction is in progress.");
        }
    }

    public bool ExistsLocally<T>(T entity) where T : class
    {
        return this.Set<T>().Local.Any(e => e == entity);
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
        builder.Entity<VoteConstituency>().Property(b => b.Id).ValueGeneratedOnAdd();
        builder.Entity<Vote>().Property(b => b.Id).ValueGeneratedOnAdd();
        builder.Entity<Vote>().Property(b => b.Created).HasDefaultValueSql("getdate()");
        builder.Entity<Vote>()
         .Property(v => v.KPIRatingComments)
         .HasConversion(
             v => JsonSerializer.Serialize(v.Where(c => c.Rating != null).ToList(), JsonExtensions.IgnoreNullSerializationOptions),
             v => JsonSerializer.Deserialize<List<KPIRatingComment>>(v, JsonExtensions.IgnoreNullSerializationOptions)
         );
        builder.Entity<Vote>()
         .Property(v => v.KPIComments)
         .HasConversion(
             v => JsonSerializer.Serialize(v.Where(c => c.Comment != null).ToList(), JsonExtensions.IgnoreNullSerializationOptions),
             v => JsonSerializer.Deserialize<List<KPIComment>>(v, JsonExtensions.IgnoreNullSerializationOptions)
         );
        var delta0 = new List<KPIRatingComment>();
        builder.Entity<Vote>()
         .Property(v => v.KPIRatingCommentsDelta)
         .HasConversion(
             v => JsonSerializer.Serialize(v, JsonExtensions.IgnoreNullSerializationOptions),
             v => (JsonExtensions.TryDeserialize<List<KPIRatingComment>>(v, out delta0, JsonExtensions.IgnoreNullSerializationOptions)
             ? delta0 : new List<KPIRatingComment>())
         );

        //for updatetime had to use triggers

        builder.Entity<VoteSummary>().HasKey(x => x.Id);
        builder.Entity<VoteSummary>().Property(b => b.Id).ValueGeneratedOnAdd();

        builder.Entity<VoteSummary>().Property(b => b.Created).HasDefaultValueSql("getdate()");

        builder.Entity<VoteSummary>()
                .HasOne(x => x.Constituency)
                .WithOne(c => c.Summary)
                .HasForeignKey<VoteSummary>(x => x.ConstituencyId);

        builder.Entity<VoteSummary>()
        .Property(v => v.KPIVotes)
        .HasConversion(
            v => JsonSerializer.Serialize(v, JsonExtensions.IgnoreNullSerializationOptions),
            v => JsonSerializer.Deserialize<List<KPIVote>>(v, JsonExtensions.IgnoreNullSerializationOptions)
        );














        //builder.Entity<V_Constituency>().Property(b => b.Id).ValueGeneratedOnAdd();

        ////https://learn.microsoft.com/en-us/ef/core/modeling/generated-properties?tabs=fluent-api
        //builder.Entity<V_Vote>().Property(b => b.Id).ValueGeneratedOnAdd();
        //builder.Entity<V_Vote>().Property(b => b.Created).HasDefaultValueSql("getdate()");

        ////builder.Entity<V_Vote>().OwnsMany(b => b.VoteKPIRatingComments, ownedNavigationBuilder =>
        //// { ownedNavigationBuilder.ToJson(); }
        ////);
        ////above wont work so trying like below
        //builder.Entity<V_Vote>()
        // .Property(v => v.VoteKPIRatingComments)
        // .HasConversion(
        //     v => JsonSerializer.Serialize(v.Where(c => c.Rating != null).ToList(), JsonExtensions.IgnoreNullSerializationOptions),
        //     v => JsonSerializer.Deserialize<List<VoteKPIRatingComment>>(v, JsonExtensions.IgnoreNullSerializationOptions)
        // );
        //builder.Entity<V_Vote>()
        // .Property(v => v.VoteKPIComments)
        // .HasConversion(
        //     v => JsonSerializer.Serialize(v.Where(c => c.Comment != null).ToList(), JsonExtensions.IgnoreNullSerializationOptions),
        //     v => JsonSerializer.Deserialize<List<VoteKPIComment>>(v, JsonExtensions.IgnoreNullSerializationOptions)
        // );
        //var delta = new List<VoteKPIRatingComment>();
        //builder.Entity<V_Vote>()
        // .Property(v => v.VoteKPIRatingCommentsDelta)
        // .HasConversion(
        //     v => JsonSerializer.Serialize(v, JsonExtensions.IgnoreNullSerializationOptions),
        //     v => (JsonExtensions.TryDeserialize<List<VoteKPIRatingComment>>(v, out delta, JsonExtensions.IgnoreNullSerializationOptions)
        //     ? delta : new List<VoteKPIRatingComment>())
        // );

        ////for updatetime had to use triggers

        //builder.Entity<V_VoteSummary>().HasKey(x => x.Id);
        //builder.Entity<V_VoteSummary>().Property(b => b.Id).ValueGeneratedOnAdd();
        
        //builder.Entity<V_VoteSummary>().Property(b => b.Created).HasDefaultValueSql("getdate()");

        //builder.Entity<V_VoteSummary>()
        //        .HasOne(x => x.Constituency)
        //        .WithOne(c => c.Summary)
        //        .HasForeignKey<V_VoteSummary>(x => x.ConstituencyId);

        //builder.Entity<V_VoteSummary>()
        //.Property(v => v.KPIVotes)
        //.HasConversion(
        //    v => JsonSerializer.Serialize(v, JsonExtensions.IgnoreNullSerializationOptions),
        //    v => JsonSerializer.Deserialize<List<VoteSummary_KPIVote>>(v, JsonExtensions.IgnoreNullSerializationOptions)
        //);
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