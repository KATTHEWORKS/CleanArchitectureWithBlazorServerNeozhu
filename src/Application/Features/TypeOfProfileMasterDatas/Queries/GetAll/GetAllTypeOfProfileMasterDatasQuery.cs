// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.TypeOfProfileMasterDatas.DTOs;
using CleanArchitecture.Blazor.Application.Features.TypeOfProfileMasterDatas.Caching;

namespace CleanArchitecture.Blazor.Application.Features.TypeOfProfileMasterDatas.Queries.GetAll;

public class GetAllTypeOfProfileMasterDatasQuery : ICacheableRequest<IEnumerable<TypeOfProfileMasterDataDto>>
{
   public string CacheKey => TypeOfProfileMasterDataCacheKey.GetAllCacheKey;
   public MemoryCacheEntryOptions? Options => TypeOfProfileMasterDataCacheKey.MemoryCacheEntryOptions;
}

public class GetAllTypeOfProfileMasterDatasQueryHandler :
     IRequestHandler<GetAllTypeOfProfileMasterDatasQuery, IEnumerable<TypeOfProfileMasterDataDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<GetAllTypeOfProfileMasterDatasQueryHandler> _localizer;

    public GetAllTypeOfProfileMasterDatasQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IStringLocalizer<GetAllTypeOfProfileMasterDatasQueryHandler> localizer
        )
    {
        _context = context;
        _mapper = mapper;
        _localizer = localizer;
    }

    public async Task<IEnumerable<TypeOfProfileMasterDataDto>> Handle(GetAllTypeOfProfileMasterDatasQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.TypeOfProfileMasterDatas
                     .ProjectTo<TypeOfProfileMasterDataDto>(_mapper.ConfigurationProvider)
                     .AsNoTracking()
                     .ToListAsync(cancellationToken);
        return data;
    }
}


