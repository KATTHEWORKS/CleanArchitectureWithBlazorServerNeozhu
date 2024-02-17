using System.Linq.Dynamic.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;
using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;
using CleanArchitecture.Blazor.Application.Features.Tenants.Caching;
using CleanArchitecture.Blazor.Application.Features.Tenants.DTOs;
using CleanArchitecture.Blazor.Domain.Enums;
using CleanArchitecture.Blazor.Domain.Identity;
using CleanArchitecture.Blazor.Infrastructure.Common.Extensions;
using LazyCache;
using ZiggyCreatures.Caching.Fusion;

namespace CleanArchitecture.Blazor.Infrastructure.Services.MultiTenant;

public class TenantService : ITenantService
{

    private readonly IApplicationDbContext _context;
    private readonly IFusionCache _fusionCache;
    private readonly IMapper _mapper;

    public TenantService(
        IFusionCache fusionCache,
        IServiceScopeFactory scopeFactory,
        IMapper mapper)
    {

        var scope = scopeFactory.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        _fusionCache = fusionCache;
        _mapper = mapper;
    }

    public event Action? OnChange;
    public List<TenantDto> DataSource { get; private set; } = new();



    public void Initialize()
    {
        DataSource = _fusionCache.GetOrSet(TenantCacheKey.TenantsCacheKey,
            _ => _context.Tenants.OrderBy(x => x.Name)
                .ProjectTo<TenantDto>(_mapper.ConfigurationProvider)
                .ToList()) ?? new List<TenantDto>();
    }

    public void Refresh()
    {
        _fusionCache.Remove(TenantCacheKey.TenantsCacheKey);
        DataSource = _fusionCache.GetOrSet(TenantCacheKey.TenantsCacheKey,
             _ => _context.Tenants.OrderBy(x => x.Name)
                 .ProjectTo<TenantDto>(_mapper.ConfigurationProvider)
                 .ToList()) ?? new List<TenantDto>();
        OnChange?.Invoke();
    }

    //public async Task<byte> GetTenantType(string tenantId)
    //{
    //    return await _context.Tenants.Where(x => x.Id == tenantId).Select(x => x.Type).FirstOrDefaultAsync();
    //}
    public byte GetTenantType(string tenantId)
    {
        return DataSource.Where(x => x.Id == tenantId).Select(x => x.Type).FirstOrDefault();
    }
}