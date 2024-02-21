// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.DTOs;
using CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.Caching;
using CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.Queries.Pagination;

public class VotesWithPaginationQuery : VoteAdvancedFilter, ICacheableRequest<PaginatedData<VoteDto>>
{
    public override string ToString()
    {
        return $"Listview:{ListView}, Search:{Keyword}, {OrderBy}, {SortDirection}, {PageNumber}, {PageSize}";
    }
    public string CacheKey => VoteCacheKey.GetPaginationCacheKey($"{this}");
    public MemoryCacheEntryOptions? Options => VoteCacheKey.MemoryCacheEntryOptions;
    public VoteAdvancedSpecification Specification => new VoteAdvancedSpecification(this);
}
    
public class VotesWithPaginationQueryHandler :
         IRequestHandler<VotesWithPaginationQuery, PaginatedData<VoteDto>>
{
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<VotesWithPaginationQueryHandler> _localizer;

        public VotesWithPaginationQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<VotesWithPaginationQueryHandler> localizer
            )
        {
            _context = context;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<PaginatedData<VoteDto>> Handle(VotesWithPaginationQuery request, CancellationToken cancellationToken)
        {
           var data = await _context.Votes.OrderBy($"{request.OrderBy} {request.SortDirection}")
                                    .ProjectToPaginatedDataAsync<Vote, VoteDto>(request.Specification, request.PageNumber, request.PageSize, _mapper.ConfigurationProvider, cancellationToken);
            return data;
        }
}