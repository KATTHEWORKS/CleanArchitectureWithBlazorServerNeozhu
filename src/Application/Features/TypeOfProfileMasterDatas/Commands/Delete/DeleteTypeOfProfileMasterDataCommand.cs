// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.TypeOfProfileMasterDatas.Caching;


namespace CleanArchitecture.Blazor.Application.Features.TypeOfProfileMasterDatas.Commands.Delete;

    public class DeleteTypeOfProfileMasterDataCommand:  ICacheInvalidatorRequest<Result<int>>
    {
      public int[] Id {  get; }
      public string CacheKey => TypeOfProfileMasterDataCacheKey.GetAllCacheKey;
      public CancellationTokenSource? SharedExpiryTokenSource => TypeOfProfileMasterDataCacheKey.SharedExpiryTokenSource();
      public DeleteTypeOfProfileMasterDataCommand(int[] id)
      {
        Id = id;
      }
    }

    public class DeleteTypeOfProfileMasterDataCommandHandler : 
                 IRequestHandler<DeleteTypeOfProfileMasterDataCommand, Result<int>>

    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<DeleteTypeOfProfileMasterDataCommandHandler> _localizer;
        public DeleteTypeOfProfileMasterDataCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<DeleteTypeOfProfileMasterDataCommandHandler> localizer,
             IMapper mapper
            )
        {
            _context = context;
            _localizer = localizer;
            _mapper = mapper;
        }
        public async Task<Result<int>> Handle(DeleteTypeOfProfileMasterDataCommand request, CancellationToken cancellationToken)
        {
            var items = await _context.TypeOfProfileMasterDatas.Where(x=>request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
            foreach (var item in items)
            {
			    // raise a delete domain event
				item.AddDomainEvent(new TypeOfProfileMasterDataDeletedEvent(item));
                _context.TypeOfProfileMasterDatas.Remove(item);
            }
            var result = await _context.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(result);
        }

    }

