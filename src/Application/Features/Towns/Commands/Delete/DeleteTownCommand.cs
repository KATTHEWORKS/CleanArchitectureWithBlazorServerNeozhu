// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Towns.Caching;


namespace CleanArchitecture.Blazor.Application.Features.Towns.Commands.Delete;

    public class DeleteTownCommand:  ICacheInvalidatorRequest<Result<int>>
    {
      public int[] Id {  get; }
      public string CacheKey => TownCacheKey.GetAllCacheKey;
      public CancellationTokenSource? SharedExpiryTokenSource => TownCacheKey.SharedExpiryTokenSource();
      public DeleteTownCommand(int[] id)
      {
        Id = id;
      }
    }

    public class DeleteTownCommandHandler : 
                 IRequestHandler<DeleteTownCommand, Result<int>>

    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<DeleteTownCommandHandler> _localizer;
        public DeleteTownCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<DeleteTownCommandHandler> localizer,
             IMapper mapper
            )
        {
            _context = context;
            _localizer = localizer;
            _mapper = mapper;
        }
        public async Task<Result<int>> Handle(DeleteTownCommand request, CancellationToken cancellationToken)
        {
            var items = await _context.Towns.Where(x=>request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
            foreach (var item in items)
            {
			    // raise a delete domain event
				item.AddDomainEvent(new TownDeletedEvent(item));
                _context.Towns.Remove(item);
            }
            var result = await _context.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(result);
        }

    }

