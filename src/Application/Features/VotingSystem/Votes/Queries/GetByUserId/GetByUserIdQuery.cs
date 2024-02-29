// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.DTOs;
using CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.Caching;
using CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.Queries.GetByUserId;

public class GetByUserIdQuery : ICacheableRequest<VoteDto>
{
   public required string UserId { get; set; }
   public string CacheKey => VoteCacheKey.GetByUserIdCacheKey($"{UserId}");
   public MemoryCacheEntryOptions? Options => VoteCacheKey.MemoryCacheEntryOptions;
}

public class GetByUserIdQueryHandler :
     IRequestHandler<GetByUserIdQuery, VoteDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<GetByUserIdQueryHandler> _localizer;

    public GetByUserIdQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IStringLocalizer<GetByUserIdQueryHandler> localizer
        )
    {
        _context = context;
        _mapper = mapper;
        _localizer = localizer;
    }

    public async Task<VoteDto> Handle(GetByUserIdQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Votes.ApplySpecification(new VoteByUserIdSpecification(request.UserId))
                     .ProjectTo<VoteDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
        //?? throw new NotFoundException($"Vote with UserId: [{request.UserId}] not found.");
        return data;
    }
}
