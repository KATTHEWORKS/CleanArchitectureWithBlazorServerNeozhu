// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.TypeOfProfileMasterDatas.DTOs;
using CleanArchitecture.Blazor.Application.Features.TypeOfProfileMasterDatas.Caching;

namespace CleanArchitecture.Blazor.Application.Features.TypeOfProfileMasterDatas.Commands.Import;

    public class ImportTypeOfProfileMasterDatasCommand: ICacheInvalidatorRequest<Result<int>>
    {
        public string FileName { get; set; }
        public byte[] Data { get; set; }
        public string CacheKey => TypeOfProfileMasterDataCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => TypeOfProfileMasterDataCacheKey.SharedExpiryTokenSource();
        public ImportTypeOfProfileMasterDatasCommand(string fileName,byte[] data)
        {
           FileName = fileName;
           Data = data;
        }
    }
    public record class CreateTypeOfProfileMasterDatasTemplateCommand : IRequest<Result<byte[]>>
    {
 
    }

    public class ImportTypeOfProfileMasterDatasCommandHandler : 
                 IRequestHandler<CreateTypeOfProfileMasterDatasTemplateCommand, Result<byte[]>>,
                 IRequestHandler<ImportTypeOfProfileMasterDatasCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<ImportTypeOfProfileMasterDatasCommandHandler> _localizer;
        private readonly IExcelService _excelService;
        private readonly TypeOfProfileMasterDataDto _dto = new();

        public ImportTypeOfProfileMasterDatasCommandHandler(
            IApplicationDbContext context,
            IExcelService excelService,
            IStringLocalizer<ImportTypeOfProfileMasterDatasCommandHandler> localizer,
            IMapper mapper
            )
        {
            _context = context;
            _localizer = localizer;
            _excelService = excelService;
            _mapper = mapper;
        }
        #nullable disable warnings
        public async Task<Result<int>> Handle(ImportTypeOfProfileMasterDatasCommand request, CancellationToken cancellationToken)
        {

           var result = await _excelService.ImportAsync(request.Data, mappers: new Dictionary<string, Func<DataRow, TypeOfProfileMasterDataDto, object?>>
            {
                { _localizer[_dto.GetMemberDescription(x=>x.SystemTypeId)], (row, item) => item.SystemTypeId = int.Parse(row[_localizer[_dto.GetMemberDescription(x=>x.SystemTypeId)]].ToString()) }, 
{ _localizer[_dto.GetMemberDescription(x=>x.Name)], (row, item) => item.Name = row[_localizer[_dto.GetMemberDescription(x=>x.Name)]].ToString() }, 
{ _localizer[_dto.GetMemberDescription(x=>x.ShortName)], (row, item) => item.ShortName = row[_localizer[_dto.GetMemberDescription(x=>x.ShortName)]].ToString() }, 
{ _localizer[_dto.GetMemberDescription(x=>x.BriefDescription)], (row, item) => item.BriefDescription = row[_localizer[_dto.GetMemberDescription(x=>x.BriefDescription)]].ToString() }, 
{ _localizer[_dto.GetMemberDescription(x=>x.Price)], (row, item) => item.Price = 
int.Parse(row[_localizer[_dto.GetMemberDescription(x=>x.Price)]].ToString()) }, 

            }, _localizer[_dto.GetClassDescription()]);
            if (result.Succeeded && result.Data is not null)
            {
                foreach (var dto in result.Data)
                {
                    var exists = await _context.TypeOfProfileMasterDatas.AnyAsync(x => x.Name == dto.Name, cancellationToken);
                    if (!exists)
                    {
                        var item = _mapper.Map<TypeOfProfileMasterData>(dto);
                        // add create domain events if this entity implement the IHasDomainEvent interface
				        // item.AddDomainEvent(new TypeOfProfileMasterDataCreatedEvent(item));
                        await _context.TypeOfProfileMasterDatas.AddAsync(item, cancellationToken);
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
        public async Task<Result<byte[]>> Handle(CreateTypeOfProfileMasterDatasTemplateCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement ImportTypeOfProfileMasterDatasCommandHandler method 
            var fields = new string[] {
                   // TODO: Define the fields that should be generate in the template, for example:
                   _localizer[_dto.GetMemberDescription(x=>x.SystemTypeId)], 
_localizer[_dto.GetMemberDescription(x=>x.Name)], 
_localizer[_dto.GetMemberDescription(x=>x.ShortName)], 
_localizer[_dto.GetMemberDescription(x=>x.BriefDescription)], 
_localizer[_dto.GetMemberDescription(x=>x.Price)], 

                };
            var result = await _excelService.CreateTemplateAsync(fields, _localizer[_dto.GetClassDescription()]);
            return await Result<byte[]>.SuccessAsync(result);
        }
    }

