// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.TownProfiles.DTOs;
using CleanArchitecture.Blazor.Application.Features.TownProfiles.Specifications;
using CleanArchitecture.Blazor.Application.Features.TownProfiles.Queries.Pagination;

namespace CleanArchitecture.Blazor.Application.Features.TownProfiles.Queries.Export;

public class ExportTownProfilesQuery : TownProfileAdvancedFilter, IRequest<Result<byte[]>>
{
      public TownProfileAdvancedSpecification Specification => new TownProfileAdvancedSpecification(this);
}
    
public class ExportTownProfilesQueryHandler :
         IRequestHandler<ExportTownProfilesQuery, Result<byte[]>>
{
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IExcelService _excelService;
        private readonly IStringLocalizer<ExportTownProfilesQueryHandler> _localizer;
        private readonly TownProfileDto _dto = new();
        public ExportTownProfilesQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IExcelService excelService,
            IStringLocalizer<ExportTownProfilesQueryHandler> localizer
            )
        {
            _context = context;
            _mapper = mapper;
            _excelService = excelService;
            _localizer = localizer;
        }
        #nullable disable warnings
        public async Task<Result<byte[]>> Handle(ExportTownProfilesQuery request, CancellationToken cancellationToken)
        {
            var data = await _context.TownProfiles.ApplySpecification(request.Specification)
                       .OrderBy($"{request.OrderBy} {request.SortDirection}")
                       .ProjectTo<TownProfileDto>(_mapper.ConfigurationProvider)
                       .AsNoTracking()
                       .ToListAsync(cancellationToken);
            var result = await _excelService.ExportAsync(data,
                new Dictionary<string, Func<TownProfileDto, object?>>()
                {
                    // TODO: Define the fields that should be exported, for example:
                    {_localizer[_dto.GetMemberDescription(x=>x.TypeId)],item => item.TypeId}, 
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
{_localizer[_dto.GetMemberDescription(x=>x.TownId)],item => item.TownId}, 
{_localizer[_dto.GetMemberDescription(x=>x.ApprovedCount)],item => item.ApprovedCount}, 
{_localizer[_dto.GetMemberDescription(x=>x.RejectedCount)],item => item.RejectedCount}, 
{_localizer[_dto.GetMemberDescription(x=>x.LikeCount)],item => item.LikeCount}, 
{_localizer[_dto.GetMemberDescription(x=>x.DisLikeCount)],item => item.DisLikeCount}, 

                }
                , _localizer[_dto.GetClassDescription()]);
            return await Result<byte[]>.SuccessAsync(result);
        }
}
