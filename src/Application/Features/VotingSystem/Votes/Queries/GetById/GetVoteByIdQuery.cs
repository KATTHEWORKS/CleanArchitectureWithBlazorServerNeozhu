// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.DTOs;
using CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.Caching;
using CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.Queries.GetById;

public class GetVoteByIdQuery : ICacheableRequest<VoteDto>
{
   public required int Id { get; set; }
   public string CacheKey => VoteCacheKey.GetByIdCacheKey($"{Id}");
   public MemoryCacheEntryOptions? Options => VoteCacheKey.MemoryCacheEntryOptions;
}

public class GetVoteByIdQueryHandler :
     IRequestHandler<GetVoteByIdQuery, VoteDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<GetVoteByIdQueryHandler> _localizer;

    public GetVoteByIdQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IStringLocalizer<GetVoteByIdQueryHandler> localizer
        )
    {
        _context = context;
        _mapper = mapper;
        _localizer = localizer;
    }

    public async Task<VoteDto> Handle(GetVoteByIdQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Votes.ApplySpecification(new VoteByIdSpecification(request.Id))
                     .ProjectTo<VoteDto>(_mapper.ConfigurationProvider)
                     .FirstAsync(cancellationToken) ?? throw new NotFoundException($"Vote with id: [{request.Id}] not found.");
        return data;
    }
}
