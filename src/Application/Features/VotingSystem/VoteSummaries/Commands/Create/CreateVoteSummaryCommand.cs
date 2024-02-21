// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using System.ComponentModel;
using CleanArchitecture.Blazor.Application.Features.VotingSystem.VoteSummaries.DTOs;
using CleanArchitecture.Blazor.Application.Features.VotingSystem.VoteSummaries.Caching;

namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.VoteSummaries.Commands.Create;

public class CreateVoteSummaryCommand: ICacheInvalidatorRequest<Result<int>>
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
             CreateMap<VoteSummaryDto,CreateVoteSummaryCommand>(MemberList.None);
             CreateMap<CreateVoteSummaryCommand,VoteSummary>(MemberList.None);
        }
    }
}
    
    public class CreateVoteSummaryCommandHandler : IRequestHandler<CreateVoteSummaryCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<CreateVoteSummaryCommand> _localizer;
        public CreateVoteSummaryCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<CreateVoteSummaryCommand> localizer,
            IMapper mapper
            )
        {
            _context = context;
            _localizer = localizer;
            _mapper = mapper;
        }
        public async Task<Result<int>> Handle(CreateVoteSummaryCommand request, CancellationToken cancellationToken)
        {
           var item = _mapper.Map<VoteSummary>(request);
           // raise a create domain event
	       item.AddDomainEvent(new VoteSummaryCreatedEvent(item));
           _context.VoteSummaries.Add(item);
           await _context.SaveChangesAsync(cancellationToken);
           return  await Result<int>.SuccessAsync(item.Id);
        }
    }

