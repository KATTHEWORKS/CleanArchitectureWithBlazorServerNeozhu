// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.CardTypes.DTOs;
using CleanArchitecture.Blazor.Application.Features.CardTypes.Caching;
using CleanArchitecture.Blazor.Application.Features.CardTypes.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.CardTypes.Queries.Pagination;

public class CardTypesWithPaginationQuery : CardTypeAdvancedFilter, ICacheableRequest<PaginatedData<CardTypeDto>>
{
    public override string ToString()
    {
        return $"Listview:{ListView}, Search:{Keyword}, {OrderBy}, {SortDirection}, {PageNumber}, {PageSize}";
    }
    public string CacheKey => CardTypeCacheKey.GetPaginationCacheKey($"{this}");
    public MemoryCacheEntryOptions? Options => CardTypeCacheKey.MemoryCacheEntryOptions;
    public CardTypeAdvancedSpecification Specification => new CardTypeAdvancedSpecification(this);
}
    
public class CardTypesWithPaginationQueryHandler :
         IRequestHandler<CardTypesWithPaginationQuery, PaginatedData<CardTypeDto>>
{
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<CardTypesWithPaginationQueryHandler> _localizer;

        public CardTypesWithPaginationQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<CardTypesWithPaginationQueryHandler> localizer
            )
        {
            _context = context;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<PaginatedData<CardTypeDto>> Handle(CardTypesWithPaginationQuery request, CancellationToken cancellationToken)
        {
           var data = await _context.CardTypes.OrderBy($"{request.OrderBy} {request.SortDirection}")
                                    .ProjectToPaginatedDataAsync<CardType, CardTypeDto>(request.Specification, request.PageNumber, request.PageSize, _mapper.ConfigurationProvider, cancellationToken);
            return data;
        }
}