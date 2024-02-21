// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.VotingSystem.VoteSummaries.DTOs;
using CleanArchitecture.Blazor.Application.Features.VotingSystem.VoteSummaries.Caching;
namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.VoteSummaries.Commands.AddEdit;

public class AddEditVoteSummaryCommand: ICacheInvalidatorRequest<Result<int>>
{
      [Description("Id")]
      public int Id { get; set; }
          [Description("Constituency Id")]
    public int ConstituencyId {get;set;} 
    [Description("Comments Count")]
    public int CommentsCount {get;set;} 
    [Description("Votes Count")]
    public int VotesCount {get;set;} 


      public string CacheKey => VoteSummaryCacheKey.GetAllCacheKey;
      public CancellationTokenSource? SharedExpiryTokenSource => VoteSummaryCacheKey.SharedExpiryTokenSource();

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<VoteSummaryDto,AddEditVoteSummaryCommand>(MemberList.None);
            CreateMap<AddEditVoteSummaryCommand,VoteSummary>(MemberList.None);
         
        }
    }
}

    public class AddEditVoteSummaryCommandHandler : IRequestHandler<AddEditVoteSummaryCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<AddEditVoteSummaryCommandHandler> _localizer;
        public AddEditVoteSummaryCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<AddEditVoteSummaryCommandHandler> localizer,
            IMapper mapper
            )
        {
            _context = context;
            _localizer = localizer;
            _mapper = mapper;
        }
        public async Task<Result<int>> Handle(AddEditVoteSummaryCommand request, CancellationToken cancellationToken)
        {
            if (request.Id > 0)
            {
                var item = await _context.VoteSummaries.FindAsync(new object[] { request.Id }, cancellationToken) ?? throw new NotFoundException($"VoteSummary with id: [{request.Id}] not found.");
                item = _mapper.Map(request, item);
				// raise a update domain event
				item.AddDomainEvent(new VoteSummaryUpdatedEvent(item));
                await _context.SaveChangesAsync(cancellationToken);
                return await Result<int>.SuccessAsync(item.Id);
            }
            else
            {
                var item = _mapper.Map<VoteSummary>(request);
                // raise a create domain event
				item.AddDomainEvent(new VoteSummaryCreatedEvent(item));
                _context.VoteSummaries.Add(item);
                await _context.SaveChangesAsync(cancellationToken);
                return await Result<int>.SuccessAsync(item.Id);
            }
           
        }
    }

