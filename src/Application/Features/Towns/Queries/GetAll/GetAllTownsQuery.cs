// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Towns.DTOs;
using CleanArchitecture.Blazor.Application.Features.Towns.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Towns.Queries.GetAll;

public class GetAllTownsQuery : ICacheableRequest<IEnumerable<TownDto>>
{
   public string CacheKey => TownCacheKey.GetAllCacheKey;
   public MemoryCacheEntryOptions? Options => TownCacheKey.MemoryCacheEntryOptions;
}

public class GetAllTownsQueryHandler :
     IRequestHandler<GetAllTownsQuery, IEnumerable<TownDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<GetAllTownsQueryHandler> _localizer;

    public GetAllTownsQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IStringLocalizer<GetAllTownsQueryHandler> localizer
        )
    {
        _context = context;
        _mapper = mapper;
        _localizer = localizer;
    }

    public async Task<IEnumerable<TownDto>> Handle(GetAllTownsQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Towns
                     .ProjectTo<TownDto>(_mapper.ConfigurationProvider)
                     .AsNoTracking()
                     .ToListAsync(cancellationToken);
        return data;
    }
}


