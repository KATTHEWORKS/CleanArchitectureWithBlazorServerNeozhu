// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.VotingSystem.Constituencies.Caching;


namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.Constituencies.Commands.Delete;

    public class DeleteConstituencyCommand:  ICacheInvalidatorRequest<Result<int>>
    {
      public int[] Id {  get; }
      public string CacheKey => ConstituencyCacheKey.GetAllCacheKey;
      public CancellationTokenSource? SharedExpiryTokenSource => ConstituencyCacheKey.SharedExpiryTokenSource();
      public DeleteConstituencyCommand(int[] id)
      {
        Id = id;
      }
    }

    public class DeleteConstituencyCommandHandler : 
                 IRequestHandler<DeleteConstituencyCommand, Result<int>>

    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<DeleteConstituencyCommandHandler> _localizer;
        public DeleteConstituencyCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<DeleteConstituencyCommandHandler> localizer,
             IMapper mapper
            )
        {
            _context = context;
            _localizer = localizer;
            _mapper = mapper;
        }
        public async Task<Result<int>> Handle(DeleteConstituencyCommand request, CancellationToken cancellationToken)
        {
            var items = await _context.Constituencies.Where(x=>request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
            foreach (var item in items)
            {
			    // raise a delete domain event
				item.AddDomainEvent(new ConstituencyDeletedEvent(item));
                _context.Constituencies.Remove(item);
            }
            var result = await _context.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(result);
        }

    }

