// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using System.ComponentModel;
using CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.DTOs;
using CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.Caching;

namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.Commands.Create;

public class CreateVoteCommand : ICacheInvalidatorRequest<Result<int>>
{
    [Description("Id")]
    public int Id { get; set; }
    [Description("User Id")]
    public string? UserId { get; set; }
    [Description("Constituency Id")]
    public int ConstituencyId { get; set; }
    [Description("Constituency Id Delta")]
    public int? ConstituencyIdDelta { get; set; }

    public string CacheKey => VoteCacheKey.GetAllCacheKey;
    public CancellationTokenSource? SharedExpiryTokenSource => VoteCacheKey.SharedExpiryTokenSource();
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<VoteDto, CreateVoteCommand>(MemberList.None);
            CreateMap<CreateVoteCommand, Vote>(MemberList.None);
        }
    }
}

public class CreateVoteCommandHandler : IRequestHandler<CreateVoteCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<CreateVoteCommand> _localizer;
    private readonly IVoteSummaryService _voteSummaryService;
    public CreateVoteCommandHandler(
        IApplicationDbContext context,
        IStringLocalizer<CreateVoteCommand> localizer,
        IMapper mapper,
        IVoteSummaryService voteSummaryService
        )
    {
        _context = context;
        _localizer = localizer;
        _mapper = mapper;
        _voteSummaryService = voteSummaryService;
    }
    public async Task<Result<int>> Handle(CreateVoteCommand request, CancellationToken cancellationToken)
    {
        var item = _mapper.Map<Vote>(request);
        // raise a create domain event
        item.AddDomainEvent(new VoteCreatedEvent(item));
        _context.Votes.Add(item);
        await _context.SaveChangesAsync(cancellationToken);
        //if (await _context.SaveChangesAsync(cancellationToken) > 0)
        //    await _voteSummaryService.RefreshSummary();//dont wait for result
        //may be only wait for first 5 votes .
        return await Result<int>.SuccessAsync(item.Id);
    }
}

