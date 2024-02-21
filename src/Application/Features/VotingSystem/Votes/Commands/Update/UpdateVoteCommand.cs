// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using System.ComponentModel;
using CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.DTOs;
using CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.Caching;

namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.Commands.Update;

public class UpdateVoteCommand: ICacheInvalidatorRequest<Result<int>>
{
      [Description("Id")]
      public int Id { get; set; }
            [Description("User Id")]
    public string? UserId {get;set;} 
    [Description("Constituency Id")]
    public int ConstituencyId {get;set;} 
    [Description("Constituency Id Delta")]
    public int? ConstituencyIdDelta {get;set;} 

        public string CacheKey => VoteCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => VoteCacheKey.SharedExpiryTokenSource();
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<VoteDto,UpdateVoteCommand>(MemberList.None);
            CreateMap<UpdateVoteCommand,Vote>(MemberList.None);
        }
    }
}

    public class UpdateVoteCommandHandler : IRequestHandler<UpdateVoteCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<UpdateVoteCommandHandler> _localizer;
        public UpdateVoteCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<UpdateVoteCommandHandler> localizer,
             IMapper mapper
            )
        {
            _context = context;
            _localizer = localizer;
            _mapper = mapper;
        }
        public async Task<Result<int>> Handle(UpdateVoteCommand request, CancellationToken cancellationToken)
        {

           var item =await _context.Votes.FindAsync( new object[] { request.Id }, cancellationToken)?? throw new NotFoundException($"Vote with id: [{request.Id}] not found.");
           item = _mapper.Map(request, item);
		    // raise a update domain event
		   item.AddDomainEvent(new VoteUpdatedEvent(item));
           await _context.SaveChangesAsync(cancellationToken);
           return await Result<int>.SuccessAsync(item.Id);
        }
    }

