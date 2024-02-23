// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.VotingSystem.Constituencies.DTOs;
using CleanArchitecture.Blazor.Application.Features.VotingSystem.Constituencies.Caching;

namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.Constituencies.Queries.GetAll;

public class GetAllConstituenciesQuery : ICacheableRequest<IEnumerable<ConstituencyDto>>
{
   public string CacheKey => ConstituencyCacheKey.GetAllCacheKey;
   public MemoryCacheEntryOptions? Options => ConstituencyCacheKey.MemoryCacheEntryOptions;
}

public class GetAllConstituenciesQueryHandler :
     IRequestHandler<GetAllConstituenciesQuery, IEnumerable<ConstituencyDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<GetAllConstituenciesQueryHandler> _localizer;

    public GetAllConstituenciesQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IStringLocalizer<GetAllConstituenciesQueryHandler> localizer
        )
    {
        _context = context;
        _mapper = mapper;
        _localizer = localizer;
    }

    public async Task<IEnumerable<ConstituencyDto>> Handle(GetAllConstituenciesQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.VoteConstituencies
                     .ProjectTo<ConstituencyDto>(_mapper.ConfigurationProvider)
                     .AsNoTracking()
                     .ToListAsync(cancellationToken);
        return data;
    }
}


