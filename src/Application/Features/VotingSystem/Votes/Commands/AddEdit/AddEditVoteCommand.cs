// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.DTOs;
using CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.Caching;
using CleanArchitecture.Blazor.Domain.Events.VotingSystem;
using CleanArchitecture.Blazor.Application.Features.VotingSystem.Constituencies.Caching;
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
    private readonly IAppCache _cache;
    public AddEditVoteCommandHandler(
        IApplicationDbContext context,
        IStringLocalizer<AddEditVoteCommandHandler> localizer,
        IMapper mapper,
        IVoteSummaryService voteSummaryService,
        IAppCache cache
        )
    {
        _context = context;
        _localizer = localizer;
        _mapper = mapper;
        _voteSummaryService = voteSummaryService;
        _cache = cache;
    }
    public async Task<Result<int>> Handle(AddEditVoteCommand request, CancellationToken cancellationToken)
    {
        if (request.KPIRatingComments != null && request.KPIRatingComments.Count > 0)
            request.KPIRatingComments = request.KPIRatingComments.Where(x => x.Rating != null).ToList();


        var existingVoteOfUser = await _context.Votes.FirstOrDefaultAsync(x => x.UserId == request.UserId, cancellationToken);
        if (existingVoteOfUser != null)
        {//means update

            //ideally this should never come
            //if (await _context.Votes.CountAsync(x => x.UserId == request.UserId, cancellationToken) > 1)
            //{
            //    //had to remove all otherthan first one
            //}

            if (request.Id != existingVoteOfUser.Id)//means different vote exists
            {
                //existingVoteOfUser.RemoveDomainEvent(new VoteDeletedEvent(existingVoteOfUser));
                //instead of removing lets update
                request.Id = existingVoteOfUser.Id;
            }
            //else //means same vote id
            //{

            //}
            //for both above case go for update thats it,since MyVote fetch is also based on userid only
            existingVoteOfUser.ConstituencyIdDelta = existingVoteOfUser.ConstituencyId;
            //existingVoteOfUser.KPIRatingCommentsDelta = existingVoteOfUser.KPIRatingComments;//this is problematic,dont use
            //if (existingVoteOfUser.KPIRatingComments != null)
            //    existingVoteOfUser.KPIRatingCommentsDelta = existingVoteOfUser.KPIRatingComments.Select(x => new KPIRatingComment(x.KPI_Id, x.Rating ?? 0)).ToList();
            existingVoteOfUser = _mapper.Map(request, existingVoteOfUser);

            // raise a update domain event
            existingVoteOfUser.AddDomainEvent(new VoteUpdatedEvent(existingVoteOfUser));
            _context.UpdateEntity(existingVoteOfUser);//this line is missing in autogeneration
            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                VoteSummaryBackgroundService.DatabaseChanged = true;
            return await Result<int>.SuccessAsync(existingVoteOfUser.Id);
        }
        else //first time adding vote,so direct insert
        {
            var item = _mapper.Map<Vote>(request);
            // raise a create domain event
            item.AddDomainEvent(new VoteCreatedEvent(item));
            _context.Votes.Add(item);
            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                VoteSummaryBackgroundService.DatabaseChanged = true;
            //    await _voteSummaryService.RefreshSummary();//ideally shouldnot wait but its giving efcore issue ,so waiting
            //                                               //may be only wait for first 5 votes .
            //_cache.Remove(ConstituencyCacheKey.GetAllCacheKey);
            return await Result<int>.SuccessAsync(item.Id);
        }


        //if (request.Id > 0)//existing
        //{
        //    var item = await _context.Votes.FindAsync(new object[] { request.Id }, cancellationToken) ?? throw new NotFoundException($"Vote with id: [{request.Id}] not found.");
        //    item.ConstituencyIdDelta = item.ConstituencyId;
        //    item.KPIRatingCommentsDelta = item.KPIRatingComments;
        //    item = _mapper.Map(request, item);

        //    // raise a update domain event
        //    item.AddDomainEvent(new VoteUpdatedEvent(item));
        //    if (await _context.SaveChangesAsync(cancellationToken) > 0)
        //        VoteSummaryBackgroundService.DatabaseChanged = true;
        //    //    await _voteSummaryService.RefreshSummary();//ideally shouldnot wait but its giving efcore issue ,so waiting
        //    //                                               //may be only wait for first 5 votes .

        //    //_cache.Remove(ConstituencyCacheKey.GetAllCacheKey);

        //    return await Result<int>.SuccessAsync(item.Id);
        //}
        //else
        //{

        //    var item = _mapper.Map<Vote>(request);
        //    // raise a create domain event
        //    item.AddDomainEvent(new VoteCreatedEvent(item));
        //    _context.Votes.Add(item);
        //    if (await _context.SaveChangesAsync(cancellationToken) > 0)
        //        VoteSummaryBackgroundService.DatabaseChanged = true;
        //    //    await _voteSummaryService.RefreshSummary();//ideally shouldnot wait but its giving efcore issue ,so waiting
        //    //                                               //may be only wait for first 5 votes .
        //    //_cache.Remove(ConstituencyCacheKey.GetAllCacheKey);
        //    return await Result<int>.SuccessAsync(item.Id);
        //}

    }
}

