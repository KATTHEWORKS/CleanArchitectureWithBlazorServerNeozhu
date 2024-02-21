// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.VotingSystem.VoteSummaries.DTOs;
using CleanArchitecture.Blazor.Application.Features.VotingSystem.VoteSummaries.Caching;
using CleanArchitecture.Blazor.Application.Features.VotingSystem.VoteSummaries.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.VoteSummaries.Queries.GetById;

public class GetVoteSummaryByIdQuery : ICacheableRequest<VoteSummaryDto>
{
   public required int Id { get; set; }
   public string CacheKey => VoteSummaryCacheKey.GetByIdCacheKey($"{Id}");
   public MemoryCacheEntryOptions? Options => VoteSummaryCacheKey.MemoryCacheEntryOptions;
}

public class GetVoteSummaryByIdQueryHandler :
     IRequestHandler<GetVoteSummaryByIdQuery, VoteSummaryDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<GetVoteSummaryByIdQueryHandler> _localizer;

    public GetVoteSummaryByIdQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IStringLocalizer<GetVoteSummaryByIdQueryHandler> localizer
        )
    {
        _context = context;
        _mapper = mapper;
        _localizer = localizer;
    }

    public async Task<VoteSummaryDto> Handle(GetVoteSummaryByIdQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.VoteSummaries.ApplySpecification(new VoteSummaryByIdSpecification(request.Id))
                     .ProjectTo<VoteSummaryDto>(_mapper.ConfigurationProvider)
                     .FirstAsync(cancellationToken) ?? throw new NotFoundException($"VoteSummary with id: [{request.Id}] not found.");
        return data;
    }
}
