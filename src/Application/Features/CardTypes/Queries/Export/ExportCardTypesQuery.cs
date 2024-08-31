// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.CardTypes.DTOs;
using CleanArchitecture.Blazor.Application.Features.CardTypes.Specifications;
using CleanArchitecture.Blazor.Application.Features.CardTypes.Queries.Pagination;

namespace CleanArchitecture.Blazor.Application.Features.CardTypes.Queries.Export;

public class ExportCardTypesQuery : CardTypeAdvancedFilter, IRequest<Result<byte[]>>
{
      public CardTypeAdvancedSpecification Specification => new CardTypeAdvancedSpecification(this);
}
    
public class ExportCardTypesQueryHandler :
         IRequestHandler<ExportCardTypesQuery, Result<byte[]>>
{
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IExcelService _excelService;
        private readonly IStringLocalizer<ExportCardTypesQueryHandler> _localizer;
        private readonly CardTypeDto _dto = new();
        public ExportCardTypesQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IExcelService excelService,
            IStringLocalizer<ExportCardTypesQueryHandler> localizer
            )
        {
            _context = context;
            _mapper = mapper;
            _excelService = excelService;
            _localizer = localizer;
        }
        #nullable disable warnings
        public async Task<Result<byte[]>> Handle(ExportCardTypesQuery request, CancellationToken cancellationToken)
        {
            var data = await _context.CardTypes.ApplySpecification(request.Specification)
                       .OrderBy($"{request.OrderBy} {request.SortDirection}")
                       .ProjectTo<CardTypeDto>(_mapper.ConfigurationProvider)
                       .AsNoTracking()
                       .ToListAsync(cancellationToken);
            var result = await _excelService.ExportAsync(data,
                new Dictionary<string, Func<CardTypeDto, object?>>()
                {
                    // TODO: Define the fields that should be exported, for example:
                    {_localizer[_dto.GetMemberDescription(x=>x.Name)],item => item.Name}, 
{_localizer[_dto.GetMemberDescription(x=>x.ShortName)],item => item.ShortName}, 
{_localizer[_dto.GetMemberDescription(x=>x.Description)],item => item.Description}, 
{_localizer[_dto.GetMemberDescription(x=>x.Price)],item => item.Price}, 

                }
                , _localizer[_dto.GetClassDescription()]);
            return await Result<byte[]>.SuccessAsync(result);
        }
}
