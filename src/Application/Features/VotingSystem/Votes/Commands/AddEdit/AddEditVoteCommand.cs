// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.DTOs;
using CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.Caching;
using CleanArchitecture.Blazor.Domain.Events.VotingSystem;
namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.Commands.AddEdit;

public class AddEditVoteCommand : ICacheInvalidatorRequest<Result<int>>
{
    [Description("Id")]
    public int Id { get; set; }

    [Description("User Id")]
    public string? UserId { get; set; }//not in UI,set from logged in

    [Description("Constituency Id")]
    public int ConstituencyId { get; set; } //not in UI,set from selected constituency

    [Description("Constituency Name")]
    public string? ConstituencyName { get; set; }//only for UI

    [Description("Existing MP Name")]
    public string? MpNameExisting { get; set; }//only for UI
    [Description("Existing MP Terms")]
    public string? ExistingMpTerms { get; set; }//only for UI

    [Description("Open Issues/Problems Needs to be addressed")]
    public string? OpenIssues { get; set; }
    //public sbyte Rating { get;set  }//auto-generate based on kpi values//make it readonly from user but 

    [Description("Do you wish to Re-Elect Current MP again?")]
    public bool? WishToReElectMp { get; set; } = null;//yes/no/no answer


    public DateTime? Created { get; set; } = DateTime.Now;//for display only

    public DateTime? LastModified { get; set; }//for display only

    public List<KPIComment>? KPIComments { get; set; }//for the sake of summary page,this will appear without any person name,so stays anonymous
    //public bool IsCommentExists() => VoteKPIRatingComments.Any(x => !string.IsNullOrEmpty(x.Comment) && x.Comment.Length > 3);
    //since comments of others also visible to all in summary page but not votes usually
    public List<KPIRatingComment>? KPIRatingComments { get; set; }//mostly for self

    public string CacheKey => VoteCacheKey.GetAllCacheKey;
    public CancellationTokenSource? SharedExpiryTokenSource => VoteCacheKey.SharedExpiryTokenSource();

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<VoteDto, AddEditVoteCommand>(MemberList.None);
            CreateMap<AddEditVoteCommand, Vote>(MemberList.None);

        }
    }
}

public class AddEditVoteCommandHandler : IRequestHandler<AddEditVoteCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<AddEditVoteCommandHandler> _localizer;
    private readonly IVoteSummaryService _voteSummaryService;
    public AddEditVoteCommandHandler(
        IApplicationDbContext context,
        IStringLocalizer<AddEditVoteCommandHandler> localizer,
        IMapper mapper,
        IVoteSummaryService voteSummaryService
        )
    {
        _context = context;
        _localizer = localizer;
        _mapper = mapper;
        _voteSummaryService = voteSummaryService;
    }
    public async Task<Result<int>> Handle(AddEditVoteCommand request, CancellationToken cancellationToken)
    {
        if (request.Id > 0)
        {
            var item = await _context.Votes.FindAsync(new object[] { request.Id }, cancellationToken) ?? throw new NotFoundException($"Vote with id: [{request.Id}] not found.");
            item = _mapper.Map(request, item);
            // raise a update domain event
            item.AddDomainEvent(new VoteUpdatedEvent(item));
            await _context.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(item.Id);
        }
        else
        {
            var item = _mapper.Map<Vote>(request);
            // raise a create domain event
            item.AddDomainEvent(new VoteCreatedEvent(item));
            _context.Votes.Add(item);
            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                _voteSummaryService.RefreshSummary();//dont wait for result
                                                     //may be only wait for first 5 votes .
            return await Result<int>.SuccessAsync(item.Id);
        }

    }
}

