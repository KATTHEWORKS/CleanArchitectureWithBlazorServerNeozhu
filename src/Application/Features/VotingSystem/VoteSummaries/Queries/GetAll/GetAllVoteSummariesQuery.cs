// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.VotingSystem.VoteSummaries.DTOs;
using CleanArchitecture.Blazor.Application.Features.VotingSystem.VoteSummaries.Caching;

namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.VoteSummaries.Queries.GetAll;

public class GetAllVoteSummariesQuery : ICacheableRequest<IEnumerable<VoteSummaryDto>>
{
   public string CacheKey => VoteSummaryCacheKey.GetAllCacheKey;
   public MemoryCacheEntryOptions? Options => VoteSummaryCacheKey.MemoryCacheEntryOptions;
}

public class GetAllVoteSummariesQueryHandler :
     IRequestHandler<GetAllVoteSummariesQuery, IEnumerable<VoteSummaryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<GetAllVoteSummariesQueryHandler> _localizer;

    public GetAllVoteSummariesQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IStringLocalizer<GetAllVoteSummariesQueryHandler> localizer
        )
    {
        _context = context;
        _mapper = mapper;
        _localizer = localizer;
    }

    public async Task<IEnumerable<VoteSummaryDto>> Handle(GetAllVoteSummariesQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.VoteSummaries
                     .ProjectTo<VoteSummaryDto>(_mapper.ConfigurationProvider)
                     .AsNoTracking()
                     .ToListAsync(cancellationToken);
        return data;
    }
}


