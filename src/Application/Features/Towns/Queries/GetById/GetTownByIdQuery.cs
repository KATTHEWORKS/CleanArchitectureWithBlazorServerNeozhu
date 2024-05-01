// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Towns.DTOs;
using CleanArchitecture.Blazor.Application.Features.Towns.Caching;
using CleanArchitecture.Blazor.Application.Features.Towns.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.Towns.Queries.GetById;

public class GetTownByIdQuery : ICacheableRequest<TownDto>
{
   public required int Id { get; set; }
   public string CacheKey => TownCacheKey.GetByIdCacheKey($"{Id}");
   public MemoryCacheEntryOptions? Options => TownCacheKey.MemoryCacheEntryOptions;
}

public class GetTownByIdQueryHandler :
     IRequestHandler<GetTownByIdQuery, TownDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<GetTownByIdQueryHandler> _localizer;

    public GetTownByIdQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IStringLocalizer<GetTownByIdQueryHandler> localizer
        )
    {
        _context = context;
        _mapper = mapper;
        _localizer = localizer;
    }

    public async Task<TownDto> Handle(GetTownByIdQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Towns.ApplySpecification(new TownByIdSpecification(request.Id))
                     .ProjectTo<TownDto>(_mapper.ConfigurationProvider)
                     .FirstAsync(cancellationToken) ?? throw new NotFoundException($"Town with id: [{request.Id}] not found.");
        return data;
    }
}
