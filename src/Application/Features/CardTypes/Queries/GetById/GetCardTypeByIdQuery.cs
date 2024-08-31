// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.CardTypes.DTOs;
using CleanArchitecture.Blazor.Application.Features.CardTypes.Caching;
using CleanArchitecture.Blazor.Application.Features.CardTypes.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.CardTypes.Queries.GetById;

public class GetCardTypeByIdQuery : ICacheableRequest<CardTypeDto>
{
   public required int Id { get; set; }
   public string CacheKey => CardTypeCacheKey.GetByIdCacheKey($"{Id}");
   public MemoryCacheEntryOptions? Options => CardTypeCacheKey.MemoryCacheEntryOptions;
}

public class GetCardTypeByIdQueryHandler :
     IRequestHandler<GetCardTypeByIdQuery, CardTypeDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<GetCardTypeByIdQueryHandler> _localizer;

    public GetCardTypeByIdQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IStringLocalizer<GetCardTypeByIdQueryHandler> localizer
        )
    {
        _context = context;
        _mapper = mapper;
        _localizer = localizer;
    }

    public async Task<CardTypeDto> Handle(GetCardTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.CardTypes.ApplySpecification(new CardTypeByIdSpecification(request.Id))
                     .ProjectTo<CardTypeDto>(_mapper.ConfigurationProvider)
                     .FirstAsync(cancellationToken) ?? throw new NotFoundException($"CardType with id: [{request.Id}] not found.");
        return data;
    }
}
