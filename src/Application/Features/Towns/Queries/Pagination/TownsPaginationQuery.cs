// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Towns.DTOs;
using CleanArchitecture.Blazor.Application.Features.Towns.Caching;
using CleanArchitecture.Blazor.Application.Features.Towns.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.Towns.Queries.Pagination;

public class TownsWithPaginationQuery : TownAdvancedFilter, ICacheableRequest<PaginatedData<TownDto>>
{
    public override string ToString()
    {
        return $"Listview:{ListView}, Search:{Keyword}, {OrderBy}, {SortDirection}, {PageNumber}, {PageSize}";
    }
    public string CacheKey => TownCacheKey.GetPaginationCacheKey($"{this}");
    public MemoryCacheEntryOptions? Options => TownCacheKey.MemoryCacheEntryOptions;
    public TownAdvancedSpecification Specification => new TownAdvancedSpecification(this);
}
    
public class TownsWithPaginationQueryHandler :
         IRequestHandler<TownsWithPaginationQuery, PaginatedData<TownDto>>
{
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<TownsWithPaginationQueryHandler> _localizer;

        public TownsWithPaginationQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<TownsWithPaginationQueryHandler> localizer
            )
        {
            _context = context;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<PaginatedData<TownDto>> Handle(TownsWithPaginationQuery request, CancellationToken cancellationToken)
        {
           var data = await _context.Towns.OrderBy($"{request.OrderBy} {request.SortDirection}")
                                    .ProjectToPaginatedDataAsync<Town, TownDto>(request.Specification, request.PageNumber, request.PageSize, _mapper.ConfigurationProvider, cancellationToken);
            return data;
        }
}