﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Serialization;
using CleanArchitecture.Blazor.Domain.Identity;
using CleanArchitecture.Blazor.Infrastructure.Configurations;
using CleanArchitecture.Blazor.Infrastructure.Constants.ClaimTypes;
using CleanArchitecture.Blazor.Infrastructure.Constants.Database;
using CleanArchitecture.Blazor.Infrastructure.PermissionSet;
using CleanArchitecture.Blazor.Infrastructure.Constants.User;
using CleanArchitecture.Blazor.Infrastructure.Persistence.Interceptors;
using CleanArchitecture.Blazor.Infrastructure.Services.MultiTenant;
using CleanArchitecture.Blazor.Infrastructure.Services.PaddleOCR;
using CleanArchitecture.Blazor.Infrastructure.Services.Serialization;
using FluentEmail.MailKitSmtp;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using ZiggyCreatures.Caching.Fusion;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;

namespace CleanArchitecture.Blazor.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSettings(configuration)
            .AddDatabase(configuration)
            .AddServices()
            .AddMessageServices(configuration);

        services
            .AddAuthenticationService(configuration)
            .AddFusionCacheService();


        services.AddSingleton<IUsersStateContainer, UsersStateContainer>();

        return services;
    }

    private static IServiceCollection AddSettings(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<IdentitySettings>(configuration.GetSection(IdentitySettings.Key))
            .AddSingleton(s => s.GetRequiredService<IOptions<IdentitySettings>>().Value)
            .AddSingleton<IIdentitySettings>(s => s.GetRequiredService<IOptions<IdentitySettings>>().Value);

        services.Configure<AppConfigurationSettings>(configuration.GetSection(AppConfigurationSettings.Key))
            .AddSingleton(s => s.GetRequiredService<IOptions<AppConfigurationSettings>>().Value)
            .AddSingleton<IApplicationSettings>(s => s.GetRequiredService<IOptions<AppConfigurationSettings>>().Value);

        services.Configure<DatabaseSettings>(configuration.GetSection(DatabaseSettings.Key))
            .AddSingleton(s => s.GetRequiredService<IOptions<DatabaseSettings>>().Value);

        services.Configure<PrivacySettings>(configuration.GetSection(PrivacySettings.Key))
            .AddSingleton(s => s.GetRequiredService<IOptions<PrivacySettings>>().Value);

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>()
            .AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("BlazorDashboardDb");
                options.EnableSensitiveDataLogging();
            });
        else
            services.AddDbContext<ApplicationDbContext>((p, m) =>
            {
                var databaseSettings = p.GetRequiredService<IOptions<DatabaseSettings>>().Value;
                m.AddInterceptors(p.GetServices<ISaveChangesInterceptor>());
                m.UseDatabase(databaseSettings.DBProvider, databaseSettings.ConnectionString);
            });

        services.AddScoped<IDbContextFactory<ApplicationDbContext>, BlazorContextFactory<ApplicationDbContext>>();
        services.AddTransient<IApplicationDbContext>(provider =>
            provider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());
        services.AddScoped<ApplicationDbContextInitializer>();

        return services;
    }

    private static DbContextOptionsBuilder UseDatabase(this DbContextOptionsBuilder builder, string dbProvider,
        string connectionString)
    {
        switch (dbProvider.ToLowerInvariant())
        {
            case DbProviderKeys.Npgsql:
                AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
                return builder.UseNpgsql(connectionString,
                        e => e.MigrationsAssembly("CleanArchitecture.Blazor.Migrators.PostgreSQL"))
                    .UseSnakeCaseNamingConvention();

            case DbProviderKeys.SqlServer:
                return builder.UseSqlServer(connectionString,
                    e => e.MigrationsAssembly("CleanArchitecture.Blazor.Migrators.MSSQL"));

            case DbProviderKeys.SqLite:
                return builder.UseSqlite(connectionString,
                    e => e.MigrationsAssembly("CleanArchitecture.Blazor.Migrators.SqLite"));

            default:
                throw new InvalidOperationException($"DB Provider {dbProvider} is not supported.");
        }
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<PicklistService>()
            .AddSingleton<IPicklistService>(sp =>
            {
                var service = sp.GetRequiredService<PicklistService>();
                service.Initialize();
                return service;
            });

        services.AddSingleton<TenantService>()
            .AddSingleton<ITenantService>(sp =>
            {
                var service = sp.GetRequiredService<TenantService>();
                service.Initialize();
                return service;
            });

        return services.AddSingleton<ISerializer, SystemTextJsonSerializer>()
            .AddScoped<ICurrentUserService, CurrentUserService>()
            .AddScoped<ITenantProvider, TenantProvider>()
            .AddScoped<IValidationService, ValidationService>()
            .AddScoped<IDateTime, DateTimeService>()
            .AddScoped<IExcelService, ExcelService>()
            .AddScoped<IUploadService, UploadService>()
            .AddScoped<IPDFService, PDFService>()
            .AddTransient<IDocumentOcrJob, DocumentOcrJob>();
    }

    private static IServiceCollection AddMessageServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var smtpClientOptions = new SmtpClientOptions();
        configuration.GetSection(nameof(SmtpClientOptions)).Bind(smtpClientOptions);
        services.Configure<SmtpClientOptions>(configuration.GetSection(nameof(SmtpClientOptions)));

        services.AddSingleton(smtpClientOptions);
        services.AddScoped<IMailService, MailService>();

        // configure your sender and template choices with dependency injection.
        services.AddFluentEmail("support@blazorserver.com")
            .AddRazorRenderer(Path.Combine(Directory.GetCurrentDirectory(), "Resources", "EmailTemplates"))
            .AddMailKitSender(smtpClientOptions);

        return services;
    }

    private static IServiceCollection AddAuthenticationService(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddIdentityCore<ApplicationUser>()
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>()
            .AddDefaultTokenProviders();
          

        services.Configure<IdentityOptions>(options =>
        {
            var identitySettings = configuration.GetRequiredSection(IdentitySettings.Key).Get<IdentitySettings>();


            // Password settings
            options.Password.RequireDigit = identitySettings!.RequireDigit;
            options.Password.RequiredLength = identitySettings.RequiredLength;
            options.Password.RequireNonAlphanumeric = identitySettings.RequireNonAlphanumeric;
            options.Password.RequireUppercase = identitySettings.RequireUpperCase;
            options.Password.RequireLowercase = identitySettings.RequireLowerCase;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(identitySettings.DefaultLockoutTimeSpan);
            options.Lockout.MaxFailedAccessAttempts = 10;
            options.Lockout.AllowedForNewUsers = true;

            // Default SignIn settings.
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;

            // User settings
            options.User.RequireUniqueEmail = true;
        });

        services.AddScoped<IIdentityService, IdentityService>()
            .AddAuthorizationCore(options =>
            {
                options.AddPolicy("CanPurge", policy => policy.RequireUserName(UserName.Administrator));
                // Here I stored necessary permissions/roles in a constant
                foreach (var prop in typeof(Permissions).GetNestedTypes().SelectMany(c =>
                             c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
                {
                    var propertyValue = prop.GetValue(null);
                    if (propertyValue is not null)
                        options.AddPolicy((string)propertyValue,
                            policy => policy.RequireClaim(ApplicationClaimTypes.Permission, (string)propertyValue));
                }
            })
            .AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddMicrosoftAccount(microsoftOptions =>
            {

                microsoftOptions.ClientId = "";// configuration.GetValue<string>("Authentication:Microsoft:ClientId");
                microsoftOptions.ClientSecret = "";// configuration.GetValue<string>("Authentication:Microsoft:ClientSecret");
                microsoftOptions.CallbackPath = "/pages/authentication/ExternalLogin";
                microsoftOptions.AccessDeniedPath = "/";
                microsoftOptions.SaveTokens = true;
            })
            .AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = "";
                    googleOptions.ClientSecret = "";
                }
                )
            .AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = "";
                facebookOptions.AppSecret = "";
                facebookOptions.CallbackPath = "/pages/authentication/ExternalLogin";
            })
            .AddIdentityCookies(options => 
            {
               
            });



        services.ConfigureApplicationCookie(options => { options.LoginPath = "/pages/authentication/login"; });
        services.AddSingleton<UserService>()
            .AddSingleton<IUserService>(sp =>
            {
                var service = sp.GetRequiredService<UserService>();
                service.Initialize();
                return service;
            });

        return services;
    }

    private static IServiceCollection AddFusionCacheService(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddFusionCache().WithDefaultEntryOptions(new FusionCacheEntryOptions
        {
            // CACHE DURATION
            Duration = TimeSpan.FromMinutes(120),
            // FAIL-SAFE OPTIONS
            IsFailSafeEnabled = true,
            FailSafeMaxDuration = TimeSpan.FromHours(8),
            FailSafeThrottleDuration = TimeSpan.FromSeconds(30),
            // FACTORY TIMEOUTS
            FactorySoftTimeout = TimeSpan.FromMilliseconds(100),
            FactoryHardTimeout = TimeSpan.FromMilliseconds(1500)
        });
        return services;
    }


}