// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.TypeOfProfileMasterDatas.DTOs;
using CleanArchitecture.Blazor.Application.Features.TypeOfProfileMasterDatas.Caching;
using CleanArchitecture.Blazor.Application.Features.TypeOfProfileMasterDatas.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.TypeOfProfileMasterDatas.Queries.GetById;

public class GetTypeOfProfileMasterDataByIdQuery : ICacheableRequest<TypeOfProfileMasterDataDto>
{
   public required int Id { get; set; }
   public string CacheKey => TypeOfProfileMasterDataCacheKey.GetByIdCacheKey($"{Id}");
   public MemoryCacheEntryOptions? Options => TypeOfProfileMasterDataCacheKey.MemoryCacheEntryOptions;
}

public class GetTypeOfProfileMasterDataByIdQueryHandler :
     IRequestHandler<GetTypeOfProfileMasterDataByIdQuery, TypeOfProfileMasterDataDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<GetTypeOfProfileMasterDataByIdQueryHandler> _localizer;

    public GetTypeOfProfileMasterDataByIdQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IStringLocalizer<GetTypeOfProfileMasterDataByIdQueryHandler> localizer
        )
    {
        _context = context;
        _mapper = mapper;
        _localizer = localizer;
    }

    public async Task<TypeOfProfileMasterDataDto> Handle(GetTypeOfProfileMasterDataByIdQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.TypeOfProfileMasterDatas.ApplySpecification(new TypeOfProfileMasterDataByIdSpecification(request.Id))
                     .ProjectTo<TypeOfProfileMasterDataDto>(_mapper.ConfigurationProvider)
                     .FirstAsync(cancellationToken) ?? throw new NotFoundException($"TypeOfProfileMasterData with id: [{request.Id}] not found.");
        return data;
    }
}
