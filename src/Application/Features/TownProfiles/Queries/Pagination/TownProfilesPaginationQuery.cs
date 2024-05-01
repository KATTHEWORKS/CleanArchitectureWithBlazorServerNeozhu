// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.TownProfiles.DTOs;
using CleanArchitecture.Blazor.Application.Features.TownProfiles.Caching;
using CleanArchitecture.Blazor.Application.Features.TownProfiles.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.TownProfiles.Queries.Pagination;

public class TownProfilesWithPaginationQuery : TownProfileAdvancedFilter, ICacheableRequest<PaginatedData<TownProfileDto>>
{
    public override string ToString()
    {
        return $"Listview:{ListView}, Search:{Keyword}, {OrderBy}, {SortDirection}, {PageNumber}, {PageSize}";
    }
    public string CacheKey => TownProfileCacheKey.GetPaginationCacheKey($"{this}");
    public MemoryCacheEntryOptions? Options => TownProfileCacheKey.MemoryCacheEntryOptions;
    public TownProfileAdvancedSpecification Specification => new TownProfileAdvancedSpecification(this);
}
    
public class TownProfilesWithPaginationQueryHandler :
         IRequestHandler<TownProfilesWithPaginationQuery, PaginatedData<TownProfileDto>>
{
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<TownProfilesWithPaginationQueryHandler> _localizer;

        public TownProfilesWithPaginationQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<TownProfilesWithPaginationQueryHandler> localizer
            )
        {
            _context = context;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<PaginatedData<TownProfileDto>> Handle(TownProfilesWithPaginationQuery request, CancellationToken cancellationToken)
        {
           var data = await _context.TownProfiles.OrderBy($"{request.OrderBy} {request.SortDirection}")
                                    .ProjectToPaginatedDataAsync<TownProfile, TownProfileDto>(request.Specification, request.PageNumber, request.PageSize, _mapper.ConfigurationProvider, cancellationToken);
            return data;
        }
}