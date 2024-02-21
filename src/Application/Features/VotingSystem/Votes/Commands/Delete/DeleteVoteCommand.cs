// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.Caching;


namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.Commands.Delete;

    public class DeleteVoteCommand:  ICacheInvalidatorRequest<Result<int>>
    {
      public int[] Id {  get; }
      public string CacheKey => VoteCacheKey.GetAllCacheKey;
      public CancellationTokenSource? SharedExpiryTokenSource => VoteCacheKey.SharedExpiryTokenSource();
      public DeleteVoteCommand(int[] id)
      {
        Id = id;
      }
    }

    public class DeleteVoteCommandHandler : 
                 IRequestHandler<DeleteVoteCommand, Result<int>>

    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<DeleteVoteCommandHandler> _localizer;
        public DeleteVoteCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<DeleteVoteCommandHandler> localizer,
             IMapper mapper
            )
        {
            _context = context;
            _localizer = localizer;
            _mapper = mapper;
        }
        public async Task<Result<int>> Handle(DeleteVoteCommand request, CancellationToken cancellationToken)
        {
            var items = await _context.Votes.Where(x=>request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
            foreach (var item in items)
            {
			    // raise a delete domain event
				item.AddDomainEvent(new VoteDeletedEvent(item));
                _context.Votes.Remove(item);
            }
            var result = await _context.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(result);
        }

    }

