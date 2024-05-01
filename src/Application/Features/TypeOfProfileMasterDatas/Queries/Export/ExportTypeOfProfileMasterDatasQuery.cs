// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.TypeOfProfileMasterDatas.DTOs;
using CleanArchitecture.Blazor.Application.Features.TypeOfProfileMasterDatas.Specifications;
using CleanArchitecture.Blazor.Application.Features.TypeOfProfileMasterDatas.Queries.Pagination;

namespace CleanArchitecture.Blazor.Application.Features.TypeOfProfileMasterDatas.Queries.Export;

public class ExportTypeOfProfileMasterDatasQuery : TypeOfProfileMasterDataAdvancedFilter, IRequest<Result<byte[]>>
{
      public TypeOfProfileMasterDataAdvancedSpecification Specification => new TypeOfProfileMasterDataAdvancedSpecification(this);
}
    
public class ExportTypeOfProfileMasterDatasQueryHandler :
         IRequestHandler<ExportTypeOfProfileMasterDatasQuery, Result<byte[]>>
{
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IExcelService _excelService;
        private readonly IStringLocalizer<ExportTypeOfProfileMasterDatasQueryHandler> _localizer;
        private readonly TypeOfProfileMasterDataDto _dto = new();
        public ExportTypeOfProfileMasterDatasQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IExcelService excelService,
            IStringLocalizer<ExportTypeOfProfileMasterDatasQueryHandler> localizer
            )
        {
            _context = context;
            _mapper = mapper;
            _excelService = excelService;
            _localizer = localizer;
        }
        #nullable disable warnings
        public async Task<Result<byte[]>> Handle(ExportTypeOfProfileMasterDatasQuery request, CancellationToken cancellationToken)
        {
            var data = await _context.TypeOfProfileMasterDatas.ApplySpecification(request.Specification)
                       .OrderBy($"{request.OrderBy} {request.SortDirection}")
                       .ProjectTo<TypeOfProfileMasterDataDto>(_mapper.ConfigurationProvider)
                       .AsNoTracking()
                       .ToListAsync(cancellationToken);
            var result = await _excelService.ExportAsync(data,
                new Dictionary<string, Func<TypeOfProfileMasterDataDto, object?>>()
                {
                    // TODO: Define the fields that should be exported, for example:
                    {_localizer[_dto.GetMemberDescription(x=>x.SystemTypeId)],item => item.SystemTypeId}, 
{_localizer[_dto.GetMemberDescription(x=>x.Name)],item => item.Name}, 
{_localizer[_dto.GetMemberDescription(x=>x.ShortName)],item => item.ShortName}, 
{_localizer[_dto.GetMemberDescription(x=>x.Description)],item => item.Description}, 
{_localizer[_dto.GetMemberDescription(x=>x.Price)],item => item.Price}, 

                }
                , _localizer[_dto.GetClassDescription()]);
            return await Result<byte[]>.SuccessAsync(result);
        }
}
