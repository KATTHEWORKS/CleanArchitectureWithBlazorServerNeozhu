// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.VotingSystem.Constituencies.DTOs;
using CleanArchitecture.Blazor.Application.Features.VotingSystem.Constituencies.Caching;
using CleanArchitecture.Blazor.Application.Features.VotingSystem.Constituencies.Specifications;
using CleanArchitecture.Blazor.Domain.Entities.VotingSystem;
using CleanArchitecture.Blazor.Application.Common.Extensions;

namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.Constituencies.Queries.Pagination;

public class ConstituenciesWithPaginationQuery : ConstituencyAdvancedFilter, ICacheableRequest<PaginatedData<ConstituencyDto>>
{
    public override string ToString()
    {
        return $"Listview:{ListView}, Search:{Keyword}, {OrderBy}, {SortDirection}, {PageNumber}, {PageSize}";
    }
    public string CacheKey => ConstituencyCacheKey.GetPaginationCacheKey($"{this}");
    public MemoryCacheEntryOptions? Options => ConstituencyCacheKey.MemoryCacheEntryOptions;
    public ConstituencyAdvancedSpecification Specification => new ConstituencyAdvancedSpecification(this);
}
    
public class ConstituenciesWithPaginationQueryHandler :
         IRequestHandler<ConstituenciesWithPaginationQuery, PaginatedData<ConstituencyDto>>
{
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<ConstituenciesWithPaginationQueryHandler> _localizer;

        public ConstituenciesWithPaginationQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<ConstituenciesWithPaginationQueryHandler> localizer
            )
        {
            _context = context;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<PaginatedData<ConstituencyDto>> Handle(ConstituenciesWithPaginationQuery request, CancellationToken cancellationToken)
        {
           var data = await _context.VoteConstituencies.OrderBy($"{request.OrderBy} {request.SortDirection}")
                                    .ProjectToPaginatedDataAsync<VoteConstituency, ConstituencyDto>(request.Specification, request.PageNumber, request.PageSize, _mapper.ConfigurationProvider, cancellationToken);
            return data;
        }
}