// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.VotingSystem.VoteSummaries.Caching;
using CleanArchitecture.Blazor.Domain.Events.VotingSystem;


namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.VoteSummaries.Commands.Delete;

    public class DeleteVoteSummaryCommand:  ICacheInvalidatorRequest<Result<int>>
    {
      public int[] Id {  get; }
      public string CacheKey => VoteSummaryCacheKey.GetAllCacheKey;
      public CancellationTokenSource? SharedExpiryTokenSource => VoteSummaryCacheKey.SharedExpiryTokenSource();
      public DeleteVoteSummaryCommand(int[] id)
      {
        Id = id;
      }
    }

    public class DeleteVoteSummaryCommandHandler : 
                 IRequestHandler<DeleteVoteSummaryCommand, Result<int>>

    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<DeleteVoteSummaryCommandHandler> _localizer;
        public DeleteVoteSummaryCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<DeleteVoteSummaryCommandHandler> localizer,
             IMapper mapper
            )
        {
            _context = context;
            _localizer = localizer;
            _mapper = mapper;
        }
        public async Task<Result<int>> Handle(DeleteVoteSummaryCommand request, CancellationToken cancellationToken)
        {
            var items = await _context.VoteSummaries.Where(x=>request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
            foreach (var item in items)
            {
			    // raise a delete domain event
				item.AddDomainEvent(new VoteSummaryDeletedEvent(item));
                _context.VoteSummaries.Remove(item);
            }
            var result = await _context.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(result);
        }

    }

