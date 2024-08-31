// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.CardTypes.Caching;


namespace CleanArchitecture.Blazor.Application.Features.CardTypes.Commands.Delete;

    public class DeleteCardTypeCommand:  ICacheInvalidatorRequest<Result<int>>
    {
      public int[] Id {  get; }
      public string CacheKey => CardTypeCacheKey.GetAllCacheKey;
      public CancellationTokenSource? SharedExpiryTokenSource => CardTypeCacheKey.GetOrCreateTokenSource();
      public DeleteCardTypeCommand(int[] id)
      {
        Id = id;
      }
    }

    public class DeleteCardTypeCommandHandler : 
                 IRequestHandler<DeleteCardTypeCommand, Result<int>>

    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<DeleteCardTypeCommandHandler> _localizer;
        public DeleteCardTypeCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<DeleteCardTypeCommandHandler> localizer,
             IMapper mapper
            )
        {
            _context = context;
            _localizer = localizer;
            _mapper = mapper;
        }
        public async Task<Result<int>> Handle(DeleteCardTypeCommand request, CancellationToken cancellationToken)
        {
            var items = await _context.CardTypes.Where(x=>request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
            foreach (var item in items)
            {
			    // raise a delete domain event
				item.AddDomainEvent(new CardTypeDeletedEvent(item));
                _context.CardTypes.Remove(item);
            }
            var result = await _context.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(result);
        }

    }

