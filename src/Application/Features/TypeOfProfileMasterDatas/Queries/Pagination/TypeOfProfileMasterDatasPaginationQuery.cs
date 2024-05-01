// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.TypeOfProfileMasterDatas.DTOs;
using CleanArchitecture.Blazor.Application.Features.TypeOfProfileMasterDatas.Caching;
using CleanArchitecture.Blazor.Application.Features.TypeOfProfileMasterDatas.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.TypeOfProfileMasterDatas.Queries.Pagination;

public class TypeOfProfileMasterDatasWithPaginationQuery : TypeOfProfileMasterDataAdvancedFilter, ICacheableRequest<PaginatedData<TypeOfProfileMasterDataDto>>
{
    public override string ToString()
    {
        return $"Listview:{ListView}, Search:{Keyword}, {OrderBy}, {SortDirection}, {PageNumber}, {PageSize}";
    }
    public string CacheKey => TypeOfProfileMasterDataCacheKey.GetPaginationCacheKey($"{this}");
    public MemoryCacheEntryOptions? Options => TypeOfProfileMasterDataCacheKey.MemoryCacheEntryOptions;
    public TypeOfProfileMasterDataAdvancedSpecification Specification => new TypeOfProfileMasterDataAdvancedSpecification(this);
}
    
public class TypeOfProfileMasterDatasWithPaginationQueryHandler :
         IRequestHandler<TypeOfProfileMasterDatasWithPaginationQuery, PaginatedData<TypeOfProfileMasterDataDto>>
{
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<TypeOfProfileMasterDatasWithPaginationQueryHandler> _localizer;

        public TypeOfProfileMasterDatasWithPaginationQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<TypeOfProfileMasterDatasWithPaginationQueryHandler> localizer
            )
        {
            _context = context;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<PaginatedData<TypeOfProfileMasterDataDto>> Handle(TypeOfProfileMasterDatasWithPaginationQuery request, CancellationToken cancellationToken)
        {
           var data = await _context.TypeOfProfileMasterDatas.OrderBy($"{request.OrderBy} {request.SortDirection}")
                                    .ProjectToPaginatedDataAsync<TypeOfProfileMasterData, TypeOfProfileMasterDataDto>(request.Specification, request.PageNumber, request.PageSize, _mapper.ConfigurationProvider, cancellationToken);
            return data;
        }
}