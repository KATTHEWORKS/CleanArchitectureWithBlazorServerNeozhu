// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.VotingSystem.Constituencies.DTOs;
using CleanArchitecture.Blazor.Application.Features.VotingSystem.Constituencies.Caching;
using CleanArchitecture.Blazor.Application.Features.VotingSystem.Constituencies.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.Constituencies.Queries.GetById;

public class GetConstituencyByIdQuery : ICacheableRequest<ConstituencyDto>
{
   public required int Id { get; set; }
   public string CacheKey => ConstituencyCacheKey.GetByIdCacheKey($"{Id}");
   public MemoryCacheEntryOptions? Options => ConstituencyCacheKey.MemoryCacheEntryOptions;
}

public class GetConstituencyByIdQueryHandler :
     IRequestHandler<GetConstituencyByIdQuery, ConstituencyDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<GetConstituencyByIdQueryHandler> _localizer;

    public GetConstituencyByIdQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IStringLocalizer<GetConstituencyByIdQueryHandler> localizer
        )
    {
        _context = context;
        _mapper = mapper;
        _localizer = localizer;
    }

    public async Task<ConstituencyDto> Handle(GetConstituencyByIdQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Constituencies.ApplySpecification(new ConstituencyByIdSpecification(request.Id))
                     .ProjectTo<ConstituencyDto>(_mapper.ConfigurationProvider)
                     .FirstAsync(cancellationToken) ?? throw new NotFoundException($"Constituency with id: [{request.Id}] not found.");
        return data;
    }
}
