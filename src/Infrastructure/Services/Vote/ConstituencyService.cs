//#define HOSPITAL_SYSTEM
#define VOTING_SYSTEM

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Blazor.Application.Features.Tenants.Caching;
using CleanArchitecture.Blazor.Application.Features.Tenants.DTOs;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using LazyCache;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Vote;

public interface IConstituencyService
{
    List<V_Constituency>? GetAllConstituency { get; }
    List<StateWithConstituencies>? GetAllConstituencyStatewise { get; }

    List<string>? GetAllStateNames();
    List<V_Constituency>? GetConstituenciesOfState(string stateName);
    V_Constituency? GetConstituency(int constituencyId);
    void Initialize();
}



#if VOTING_SYSTEM
public class ConstituencyService : IConstituencyService
{
    private readonly IAppCache _cache;
    private readonly IApplicationDbContext _context;

    // private readonly IMapper _mapper;
    private const string ConstituenciesCacheKey = "all-Constituencies";

    public ConstituencyService(
        IAppCache cache,
        IServiceScopeFactory scopeFactory)
    //,IMapper mapper)
    {
        _cache = cache;
        var scope = scopeFactory.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        //   _mapper = mapper;
    }

    private List<V_Constituency> DataSourceDistrictwise { get; set; } = [];

    private List<StateWithConstituencies> DataSourceStatewise { get; set; } = [];

    public void Initialize()
    {
        if (_cache is not null && _context is not null)
        {
            DataSourceDistrictwise = _cache.GetOrAdd(ConstituenciesCacheKey,
                () => _context.V_Constituencies!.Any() ? _context.V_Constituencies!.ToList() : null);
            if (DataSourceDistrictwise != null && DataSourceDistrictwise.Count > 0)
            {
                DataSourceStatewise = DataSourceDistrictwise.GroupBy(x => x.StateName).Select(g => new StateWithConstituencies { StateName = g.Key, Constituencies = [.. g] }).ToList();
            }
        }
    }
    //we can load all MP details & keep in memory or cache to serve even on clientside also can be done
    //by keeping on changes had t reflect back in could be slight headache task
    public V_Constituency? GetConstituency(int constituencyId)
    {
        if (DataSourceDistrictwise == null) return null;
        return DataSourceDistrictwise.Find(x => x.Id == constituencyId);
    }
    public List<V_Constituency>? GetAllConstituency => DataSourceDistrictwise;
    public List<StateWithConstituencies>? GetAllConstituencyStatewise => DataSourceStatewise;
    public List<V_Constituency>? GetConstituenciesOfState(string stateName)
    {
        if (DataSourceStatewise == null) return null;
        return DataSourceStatewise.Find(c => c.StateName == stateName)?.Constituencies;
    }
    public List<string>? GetAllStateNames()
    {
        if (DataSourceStatewise == null) return null;
        return DataSourceStatewise.Select(x => x.StateName).Distinct().ToList();
    }
}

#endif