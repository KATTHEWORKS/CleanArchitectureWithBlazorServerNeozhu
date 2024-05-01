// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.TownProfiles.DTOs;
using CleanArchitecture.Blazor.Application.Features.TownProfiles.Caching;
using CleanArchitecture.Blazor.Application.Features.TownProfiles.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.TownProfiles.Queries.GetById;

public class GetTownProfileByIdQuery : ICacheableRequest<TownProfileDto>
{
   public required int Id { get; set; }
   public string CacheKey => TownProfileCacheKey.GetByIdCacheKey($"{Id}");
   public MemoryCacheEntryOptions? Options => TownProfileCacheKey.MemoryCacheEntryOptions;
}

public class GetTownProfileByIdQueryHandler :
     IRequestHandler<GetTownProfileByIdQuery, TownProfileDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<GetTownProfileByIdQueryHandler> _localizer;

    public GetTownProfileByIdQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IStringLocalizer<GetTownProfileByIdQueryHandler> localizer
        )
    {
        _context = context;
        _mapper = mapper;
        _localizer = localizer;
    }

    public async Task<TownProfileDto> Handle(GetTownProfileByIdQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.TownProfiles.ApplySpecification(new TownProfileByIdSpecification(request.Id))
                     .ProjectTo<TownProfileDto>(_mapper.ConfigurationProvider)
                     .FirstAsync(cancellationToken) ?? throw new NotFoundException($"TownProfile with id: [{request.Id}] not found.");
        return data;
    }
}
