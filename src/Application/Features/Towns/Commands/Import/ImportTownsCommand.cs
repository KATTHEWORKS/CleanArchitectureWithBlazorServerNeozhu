// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Towns.DTOs;
using CleanArchitecture.Blazor.Application.Features.Towns.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Towns.Commands.Import;

public class ImportTownsCommand : ICacheInvalidatorRequest<Result<int>>
{
    public string FileName { get; set; }
    public byte[] Data { get; set; }
    public string CacheKey => TownCacheKey.GetAllCacheKey;
    public CancellationTokenSource? SharedExpiryTokenSource => TownCacheKey.SharedExpiryTokenSource();
    public ImportTownsCommand(string fileName, byte[] data)
    {
        FileName = fileName;
        Data = data;
    }
}
public record class CreateTownsTemplateCommand : IRequest<Result<byte[]>>
{

}

public class ImportTownsCommandHandler :
             IRequestHandler<CreateTownsTemplateCommand, Result<byte[]>>,
             IRequestHandler<ImportTownsCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<ImportTownsCommandHandler> _localizer;
    private readonly IExcelService _excelService;
    private readonly TownDto _dto = new();

    public ImportTownsCommandHandler(
        IApplicationDbContext context,
        IExcelService excelService,
        IStringLocalizer<ImportTownsCommandHandler> localizer,
        IMapper mapper
        )
    {
        _context = context;
        _localizer = localizer;
        _excelService = excelService;
        _mapper = mapper;
    }
#nullable disable warnings
    public async Task<Result<int>> Handle(ImportTownsCommand request, CancellationToken cancellationToken)
    {

        var result = await _excelService.ImportAsync(request.Data, mappers: new Dictionary<string, Func<DataRow, TownDto, object?>>
            {
                { _localizer[_dto.GetMemberDescription(x=>x.District)], (row, item) => item.District = row[_localizer[_dto.GetMemberDescription(x=>x.District)]].ToString() },
{ _localizer[_dto.GetMemberDescription(x=>x.State)], (row, item) => item.State = row[_localizer[_dto.GetMemberDescription(x=>x.State)]].ToString() },
{ _localizer[_dto.GetMemberDescription(x=>x.UrlName1)], (row, item) => item.UrlName1 = row[_localizer[_dto.GetMemberDescription(x=>x.UrlName1)]].ToString() },
{ _localizer[_dto.GetMemberDescription(x=>x.UrlName2)], (row, item) => item.UrlName2 = row[_localizer[_dto.GetMemberDescription(x=>x.UrlName2)]].ToString() },
{ _localizer[_dto.GetMemberDescription(x=>x.Active)], (row, item) => item.Active =Convert.ToBoolean(row[_localizer[_dto.GetMemberDescription(x=>x.Active)]]) },
{ _localizer[_dto.GetMemberDescription(x=>x.Name)], (row, item) => item.Name = row[_localizer[_dto.GetMemberDescription(x=>x.Name)]].ToString() },
{ _localizer[_dto.GetMemberDescription(x=>x.SubTitle)], (row, item) => item.SubTitle = row[_localizer[_dto.GetMemberDescription(x=>x.SubTitle)]].ToString() },
{ _localizer[_dto.GetMemberDescription(x=>x.Description)], (row, item) => item.Description = row[_localizer[_dto.GetMemberDescription(x=>x.Description)]].ToString() },
{ _localizer[_dto.GetMemberDescription(x=>x.ImageUrl)], (row, item) => item.ImageUrl = row[_localizer[_dto.GetMemberDescription(x=>x.ImageUrl)]].ToString() },
{ _localizer[_dto.GetMemberDescription(x=>x.Address)], (row, item) => item.Address = row[_localizer[_dto.GetMemberDescription(x=>x.Address)]].ToString() },
{ _localizer[_dto.GetMemberDescription(x=>x.MobileNumber)], (row, item) => item.MobileNumber = row[_localizer[_dto.GetMemberDescription(x=>x.MobileNumber)]].ToString() },
{ _localizer[_dto.GetMemberDescription(x=>x.GoogleMapAddressUrl)], (row, item) => item.GoogleMapAddressUrl = row[_localizer[_dto.GetMemberDescription(x=>x.GoogleMapAddressUrl)]].ToString() },
{ _localizer[_dto.GetMemberDescription(x=>x.EndDateToShow)], (row, item) => item.EndDateToShow = DateTime.TryParse( row[_localizer[_dto.GetMemberDescription(x=>x.EndDateToShow)]].ToString() , out DateTime res) ? res : DateTime.Now },
{ _localizer[_dto.GetMemberDescription(x=>x.PriotiryOrder)], (row, item) => item.PriotiryOrder =  int.TryParse(row[_localizer[_dto.GetMemberDescription(x=>x.PriotiryOrder)]].ToString() , out int resId) ? resId : 0 },
{ _localizer[_dto.GetMemberDescription(x=>x.GoogleProfileUrl)], (row, item) => item.GoogleProfileUrl = row[_localizer[_dto.GetMemberDescription(x=>x.GoogleProfileUrl)]].ToString() },
{ _localizer[_dto.GetMemberDescription(x=>x.FaceBookUrl)], (row, item) => item.FaceBookUrl = row[_localizer[_dto.GetMemberDescription(x=>x.FaceBookUrl)]].ToString() },
{ _localizer[_dto.GetMemberDescription(x=>x.YouTubeUrl)], (row, item) => item.YouTubeUrl = row[_localizer[_dto.GetMemberDescription(x=>x.YouTubeUrl)]].ToString() },
{ _localizer[_dto.GetMemberDescription(x=>x.InstagramUrl)], (row, item) => item.InstagramUrl = row[_localizer[_dto.GetMemberDescription(x=>x.InstagramUrl)]].ToString() },
{ _localizer[_dto.GetMemberDescription(x=>x.TwitterUrl)], (row, item) => item.TwitterUrl = row[_localizer[_dto.GetMemberDescription(x=>x.TwitterUrl)]].ToString() },
{ _localizer[_dto.GetMemberDescription(x=>x.OtherReferenceUrl)], (row, item) => item.OtherReferenceUrl = row[_localizer[_dto.GetMemberDescription(x=>x.OtherReferenceUrl)]].ToString() },

            }, _localizer[_dto.GetClassDescription()]);
        if (result.Succeeded && result.Data is not null)
        {
            foreach (var dto in result.Data)
            {
                var exists = await _context.Towns.AnyAsync(x => x.Name == dto.Name, cancellationToken);
                if (!exists)
                {
                    var item = _mapper.Map<Town>(dto);
                    // add create domain events if this entity implement the IHasDomainEvent interface
                    // item.AddDomainEvent(new TownCreatedEvent(item));
                    await _context.Towns.AddAsync(item, cancellationToken);
                }
            }
            await _context.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(result.Data.Count());
        }
        else
        {
            return await Result<int>.FailureAsync(result.Errors);
        }
    }
    public async Task<Result<byte[]>> Handle(CreateTownsTemplateCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement ImportTownsCommandHandler method 
        var fields = new string[] {
                   // TODO: Define the fields that should be generate in the template, for example:
                   _localizer[_dto.GetMemberDescription(x=>x.District)],
_localizer[_dto.GetMemberDescription(x=>x.State)],
_localizer[_dto.GetMemberDescription(x=>x.UrlName1)],
_localizer[_dto.GetMemberDescription(x=>x.UrlName2)],
_localizer[_dto.GetMemberDescription(x=>x.Active)],
_localizer[_dto.GetMemberDescription(x=>x.Name)],
_localizer[_dto.GetMemberDescription(x=>x.SubTitle)],
_localizer[_dto.GetMemberDescription(x=>x.Description)],
_localizer[_dto.GetMemberDescription(x=>x.ImageUrl)],
_localizer[_dto.GetMemberDescription(x=>x.Address)],
_localizer[_dto.GetMemberDescription(x=>x.MobileNumber)],
_localizer[_dto.GetMemberDescription(x=>x.GoogleMapAddressUrl)],
_localizer[_dto.GetMemberDescription(x=>x.EndDateToShow)],
_localizer[_dto.GetMemberDescription(x=>x.PriotiryOrder)],
_localizer[_dto.GetMemberDescription(x=>x.GoogleProfileUrl)],
_localizer[_dto.GetMemberDescription(x=>x.FaceBookUrl)],
_localizer[_dto.GetMemberDescription(x=>x.YouTubeUrl)],
_localizer[_dto.GetMemberDescription(x=>x.InstagramUrl)],
_localizer[_dto.GetMemberDescription(x=>x.TwitterUrl)],
_localizer[_dto.GetMemberDescription(x=>x.OtherReferenceUrl)],

                };
        var result = await _excelService.CreateTemplateAsync(fields, _localizer[_dto.GetClassDescription()]);
        return await Result<byte[]>.SuccessAsync(result);
    }
}

