// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.TownProfiles.Caching;


namespace CleanArchitecture.Blazor.Application.Features.TownProfiles.Commands.Delete;

    public class DeleteTownProfileCommand:  ICacheInvalidatorRequest<Result<int>>
    {
      public int[] Id {  get; }
      public string CacheKey => TownProfileCacheKey.GetAllCacheKey;
      public CancellationTokenSource? SharedExpiryTokenSource => TownProfileCacheKey.SharedExpiryTokenSource();
      public DeleteTownProfileCommand(int[] id)
      {
        Id = id;
      }
    }

    public class DeleteTownProfileCommandHandler : 
                 IRequestHandler<DeleteTownProfileCommand, Result<int>>

    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<DeleteTownProfileCommandHandler> _localizer;
        public DeleteTownProfileCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<DeleteTownProfileCommandHandler> localizer,
             IMapper mapper
            )
        {
            _context = context;
            _localizer = localizer;
            _mapper = mapper;
        }
        public async Task<Result<int>> Handle(DeleteTownProfileCommand request, CancellationToken cancellationToken)
        {
            var items = await _context.TownProfiles.Where(x=>request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
            foreach (var item in items)
            {
			    // raise a delete domain event
				item.AddDomainEvent(new TownProfileDeletedEvent(item));
                _context.TownProfiles.Remove(item);
            }
            var result = await _context.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(result);
        }

    }

