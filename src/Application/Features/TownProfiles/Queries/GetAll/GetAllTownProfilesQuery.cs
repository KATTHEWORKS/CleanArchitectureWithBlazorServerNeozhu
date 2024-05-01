// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.TownProfiles.DTOs;
using CleanArchitecture.Blazor.Application.Features.TownProfiles.Caching;

namespace CleanArchitecture.Blazor.Application.Features.TownProfiles.Queries.GetAll;

public class GetAllTownProfilesQuery : ICacheableRequest<IEnumerable<TownProfileDto>>
{
   public string CacheKey => TownProfileCacheKey.GetAllCacheKey;
   public MemoryCacheEntryOptions? Options => TownProfileCacheKey.MemoryCacheEntryOptions;
}

public class GetAllTownProfilesQueryHandler :
     IRequestHandler<GetAllTownProfilesQuery, IEnumerable<TownProfileDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<GetAllTownProfilesQueryHandler> _localizer;

    public GetAllTownProfilesQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IStringLocalizer<GetAllTownProfilesQueryHandler> localizer
        )
    {
        _context = context;
        _mapper = mapper;
        _localizer = localizer;
    }

    public async Task<IEnumerable<TownProfileDto>> Handle(GetAllTownProfilesQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.TownProfiles
                     .ProjectTo<TownProfileDto>(_mapper.ConfigurationProvider)
                     .AsNoTracking()
                     .ToListAsync(cancellationToken);
        return data;
    }
}


