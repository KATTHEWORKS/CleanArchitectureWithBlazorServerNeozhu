// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.CardTypes.DTOs;
using CleanArchitecture.Blazor.Application.Features.CardTypes.Caching;

namespace CleanArchitecture.Blazor.Application.Features.CardTypes.Queries.GetAll;

public class GetAllCardTypesQuery : ICacheableRequest<IEnumerable<CardTypeDto>>
{
   public string CacheKey => CardTypeCacheKey.GetAllCacheKey;
   public MemoryCacheEntryOptions? Options => CardTypeCacheKey.MemoryCacheEntryOptions;
}

public class GetAllCardTypesQueryHandler :
     IRequestHandler<GetAllCardTypesQuery, IEnumerable<CardTypeDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<GetAllCardTypesQueryHandler> _localizer;

    public GetAllCardTypesQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IStringLocalizer<GetAllCardTypesQueryHandler> localizer
        )
    {
        _context = context;
        _mapper = mapper;
        _localizer = localizer;
    }

    public async Task<IEnumerable<CardTypeDto>> Handle(GetAllCardTypesQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.CardTypes
                     .ProjectTo<CardTypeDto>(_mapper.ConfigurationProvider)
                     .AsNoTracking()
                     .ToListAsync(cancellationToken);
        return data;
    }
}


