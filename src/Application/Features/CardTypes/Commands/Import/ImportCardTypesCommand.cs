// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.CardTypes.DTOs;
using CleanArchitecture.Blazor.Application.Features.CardTypes.Caching;

namespace CleanArchitecture.Blazor.Application.Features.CardTypes.Commands.Import;

    public class ImportCardTypesCommand: ICacheInvalidatorRequest<Result<int>>
    {
        public string FileName { get; set; }
        public byte[] Data { get; set; }
        public string CacheKey => CardTypeCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => CardTypeCacheKey.GetOrCreateTokenSource();
        public ImportCardTypesCommand(string fileName,byte[] data)
        {
           FileName = fileName;
           Data = data;
        }
    }
    public record class CreateCardTypesTemplateCommand : IRequest<Result<byte[]>>
    {
 
    }

    public class ImportCardTypesCommandHandler : 
                 IRequestHandler<CreateCardTypesTemplateCommand, Result<byte[]>>,
                 IRequestHandler<ImportCardTypesCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<ImportCardTypesCommandHandler> _localizer;
        private readonly IExcelService _excelService;
        private readonly CardTypeDto _dto = new();

        public ImportCardTypesCommandHandler(
            IApplicationDbContext context,
            IExcelService excelService,
            IStringLocalizer<ImportCardTypesCommandHandler> localizer,
            IMapper mapper
            )
        {
            _context = context;
            _localizer = localizer;
            _excelService = excelService;
            _mapper = mapper;
        }
        #nullable disable warnings
        public async Task<Result<int>> Handle(ImportCardTypesCommand request, CancellationToken cancellationToken)
        {

           var result = await _excelService.ImportAsync(request.Data, mappers: new Dictionary<string, Func<DataRow, CardTypeDto, object?>>
            {
                { _localizer[_dto.GetMemberDescription(x=>x.Name)], (row, item) => item.Name = row[_localizer[_dto.GetMemberDescription(x=>x.Name)]].ToString() }, 
{ _localizer[_dto.GetMemberDescription(x=>x.ShortName)], (row, item) => item.ShortName = row[_localizer[_dto.GetMemberDescription(x=>x.ShortName)]].ToString() }, 
{ _localizer[_dto.GetMemberDescription(x=>x.Description)], (row, item) => item.Description = row[_localizer[_dto.GetMemberDescription(x=>x.Description)]].ToString() },
{ _localizer[_dto.GetMemberDescription(x=>x.Price)], (row, item) => item.Price = _dto.Price }, 
//{ _localizer[_dto.GetMemberDescription(x=>x.Price)], (row, item) => item.Price = row[_localizer[_dto.GetMemberDescription(x=>x.Price)]].ToString() }, 

            }, _localizer[_dto.GetClassDescription()]);
            if (result.Succeeded && result.Data is not null)
            {
                foreach (var dto in result.Data)
                {
                    var exists = await _context.CardTypes.AnyAsync(x => x.Name == dto.Name, cancellationToken);
                    if (!exists)
                    {
                        var item = _mapper.Map<CardType>(dto);
                        // add create domain events if this entity implement the IHasDomainEvent interface
				        // item.AddDomainEvent(new CardTypeCreatedEvent(item));
                        await _context.CardTypes.AddAsync(item, cancellationToken);
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
        public async Task<Result<byte[]>> Handle(CreateCardTypesTemplateCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement ImportCardTypesCommandHandler method 
            var fields = new string[] {
                   // TODO: Define the fields that should be generate in the template, for example:
                   _localizer[_dto.GetMemberDescription(x=>x.Name)], 
_localizer[_dto.GetMemberDescription(x=>x.ShortName)], 
_localizer[_dto.GetMemberDescription(x=>x.Description)], 
_localizer[_dto.GetMemberDescription(x=>x.Price)], 

                };
            var result = await _excelService.CreateTemplateAsync(fields, _localizer[_dto.GetClassDescription()]);
            return await Result<byte[]>.SuccessAsync(result);
        }
    }

