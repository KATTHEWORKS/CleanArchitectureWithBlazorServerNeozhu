// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Towns.DTOs;
using CleanArchitecture.Blazor.Application.Features.Towns.Specifications;
using CleanArchitecture.Blazor.Application.Features.Towns.Queries.Pagination;

namespace CleanArchitecture.Blazor.Application.Features.Towns.Queries.Export;

public class ExportTownsQuery : TownAdvancedFilter, IRequest<Result<byte[]>>
{
      public TownAdvancedSpecification Specification => new TownAdvancedSpecification(this);
}
    
public class ExportTownsQueryHandler :
         IRequestHandler<ExportTownsQuery, Result<byte[]>>
{
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IExcelService _excelService;
        private readonly IStringLocalizer<ExportTownsQueryHandler> _localizer;
        private readonly TownDto _dto = new();
        public ExportTownsQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IExcelService excelService,
            IStringLocalizer<ExportTownsQueryHandler> localizer
            )
        {
            _context = context;
            _mapper = mapper;
            _excelService = excelService;
            _localizer = localizer;
        }
        #nullable disable warnings
        public async Task<Result<byte[]>> Handle(ExportTownsQuery request, CancellationToken cancellationToken)
        {
            var data = await _context.Towns.ApplySpecification(request.Specification)
                       .OrderBy($"{request.OrderBy} {request.SortDirection}")
                       .ProjectTo<TownDto>(_mapper.ConfigurationProvider)
                       .AsNoTracking()
                       .ToListAsync(cancellationToken);
            var result = await _excelService.ExportAsync(data,
                new Dictionary<string, Func<TownDto, object?>>()
                {
                    // TODO: Define the fields that should be exported, for example:
                    {_localizer[_dto.GetMemberDescription(x=>x.District)],item => item.District}, 
{_localizer[_dto.GetMemberDescription(x=>x.State)],item => item.State}, 
{_localizer[_dto.GetMemberDescription(x=>x.UrlName1)],item => item.UrlName1}, 
{_localizer[_dto.GetMemberDescription(x=>x.UrlName2)],item => item.UrlName2}, 
{_localizer[_dto.GetMemberDescription(x=>x.TypeOfProfileId)],item => item.TypeOfProfileId}, 
{_localizer[_dto.GetMemberDescription(x=>x.Active)],item => item.Active}, 
{_localizer[_dto.GetMemberDescription(x=>x.Name)],item => item.Name}, 
{_localizer[_dto.GetMemberDescription(x=>x.SubTitle)],item => item.SubTitle}, 
{_localizer[_dto.GetMemberDescription(x=>x.Description)],item => item.Description}, 
{_localizer[_dto.GetMemberDescription(x=>x.ImageUrl)],item => item.ImageUrl}, 
{_localizer[_dto.GetMemberDescription(x=>x.Address)],item => item.Address}, 
{_localizer[_dto.GetMemberDescription(x=>x.MobileNumber)],item => item.MobileNumber}, 
{_localizer[_dto.GetMemberDescription(x=>x.GoogleMapAddressUrl)],item => item.GoogleMapAddressUrl}, 
{_localizer[_dto.GetMemberDescription(x=>x.EndDateToShow)],item => item.EndDateToShow}, 
{_localizer[_dto.GetMemberDescription(x=>x.PriotiryOrder)],item => item.PriotiryOrder}, 
{_localizer[_dto.GetMemberDescription(x=>x.GoogleProfileUrl)],item => item.GoogleProfileUrl}, 
{_localizer[_dto.GetMemberDescription(x=>x.FaceBookUrl)],item => item.FaceBookUrl}, 
{_localizer[_dto.GetMemberDescription(x=>x.YouTubeUrl)],item => item.YouTubeUrl}, 
{_localizer[_dto.GetMemberDescription(x=>x.InstagramUrl)],item => item.InstagramUrl}, 
{_localizer[_dto.GetMemberDescription(x=>x.TwitterUrl)],item => item.TwitterUrl}, 
{_localizer[_dto.GetMemberDescription(x=>x.OtherReferenceUrl)],item => item.OtherReferenceUrl}, 

                }
                , _localizer[_dto.GetClassDescription()]);
            return await Result<byte[]>.SuccessAsync(result);
        }
}
