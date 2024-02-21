// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.VotingSystem.VoteSummaries.DTOs;
using CleanArchitecture.Blazor.Application.Features.VotingSystem.VoteSummaries.Caching;
using CleanArchitecture.Blazor.Application.Features.VotingSystem.VoteSummaries.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.VoteSummaries.Queries.Pagination;

public class VoteSummariesWithPaginationQuery : VoteSummaryAdvancedFilter, ICacheableRequest<PaginatedData<VoteSummaryDto>>
{
    public override string ToString()
    {
        return $"Listview:{ListView}, Search:{Keyword}, {OrderBy}, {SortDirection}, {PageNumber}, {PageSize}";
    }
    public string CacheKey => VoteSummaryCacheKey.GetPaginationCacheKey($"{this}");
    public MemoryCacheEntryOptions? Options => VoteSummaryCacheKey.MemoryCacheEntryOptions;
    public VoteSummaryAdvancedSpecification Specification => new VoteSummaryAdvancedSpecification(this);
}
    
public class VoteSummariesWithPaginationQueryHandler :
         IRequestHandler<VoteSummariesWithPaginationQuery, PaginatedData<VoteSummaryDto>>
{
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<VoteSummariesWithPaginationQueryHandler> _localizer;

        public VoteSummariesWithPaginationQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<VoteSummariesWithPaginationQueryHandler> localizer
            )
        {
            _context = context;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<PaginatedData<VoteSummaryDto>> Handle(VoteSummariesWithPaginationQuery request, CancellationToken cancellationToken)
        {
           var data = await _context.VoteSummaries.OrderBy($"{request.OrderBy} {request.SortDirection}")
                                    .ProjectToPaginatedDataAsync<VoteSummary, VoteSummaryDto>(request.Specification, request.PageNumber, request.PageSize, _mapper.ConfigurationProvider, cancellationToken);
            return data;
        }
}