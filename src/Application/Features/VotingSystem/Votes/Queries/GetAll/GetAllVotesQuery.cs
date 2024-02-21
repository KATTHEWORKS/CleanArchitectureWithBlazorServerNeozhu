// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.DTOs;
using CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.Caching;

namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.Queries.GetAll;

public class GetAllVotesQuery : ICacheableRequest<IEnumerable<VoteDto>>
{
   public string CacheKey => VoteCacheKey.GetAllCacheKey;
   public MemoryCacheEntryOptions? Options => VoteCacheKey.MemoryCacheEntryOptions;
}

public class GetAllVotesQueryHandler :
     IRequestHandler<GetAllVotesQuery, IEnumerable<VoteDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<GetAllVotesQueryHandler> _localizer;

    public GetAllVotesQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IStringLocalizer<GetAllVotesQueryHandler> localizer
        )
    {
        _context = context;
        _mapper = mapper;
        _localizer = localizer;
    }

    public async Task<IEnumerable<VoteDto>> Handle(GetAllVotesQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Votes
                     .ProjectTo<VoteDto>(_mapper.ConfigurationProvider)
                     .AsNoTracking()
                     .ToListAsync(cancellationToken);
        return data;
    }
}


