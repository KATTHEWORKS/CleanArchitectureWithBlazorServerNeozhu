// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.VotingSystem.Constituencies.DTOs;
using CleanArchitecture.Blazor.Application.Features.VotingSystem.Constituencies.Caching;

namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.Constituencies.Commands.Import;

    public class ImportConstituenciesCommand: ICacheInvalidatorRequest<Result<int>>
    {
        public string FileName { get; set; }
        public byte[] Data { get; set; }
        public string CacheKey => ConstituencyCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => ConstituencyCacheKey.SharedExpiryTokenSource();
        public ImportConstituenciesCommand(string fileName,byte[] data)
        {
           FileName = fileName;
           Data = data;
        }
    }
    public record class CreateConstituenciesTemplateCommand : IRequest<Result<byte[]>>
    {
 
    }

    public class ImportConstituenciesCommandHandler : 
                 IRequestHandler<CreateConstituenciesTemplateCommand, Result<byte[]>>,
                 IRequestHandler<ImportConstituenciesCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<ImportConstituenciesCommandHandler> _localizer;
        private readonly IExcelService _excelService;
        private readonly ConstituencyDto _dto = new();

        public ImportConstituenciesCommandHandler(
            IApplicationDbContext context,
            IExcelService excelService,
            IStringLocalizer<ImportConstituenciesCommandHandler> localizer,
            IMapper mapper
            )
        {
            _context = context;
            _localizer = localizer;
            _excelService = excelService;
            _mapper = mapper;
        }
        #nullable disable warnings
        public async Task<Result<int>> Handle(ImportConstituenciesCommand request, CancellationToken cancellationToken)
        {

           var result = await _excelService.ImportAsync(request.Data, mappers: new Dictionary<string, Func<DataRow, ConstituencyDto, object?>>
            {
                { _localizer[_dto.GetMemberDescription(x=>x.State)], (row, item) => item.State = row[_localizer[_dto.GetMemberDescription(x=>x.State)]].ToString() }, 
{ _localizer[_dto.GetMemberDescription(x=>x.Name)], (row, item) => item.Name = row[_localizer[_dto.GetMemberDescription(x=>x.Name)]].ToString() },
{ _localizer[_dto.GetMemberDescription(x=>x.Description)], (row, item) => item.Description = row[_localizer[_dto.GetMemberDescription(x=>x.Description)]].ToString() },
{ _localizer[_dto.GetMemberDescription(x=>x.MpNameExisting)], (row, item) => item.MpNameExisting = row[_localizer[_dto.GetMemberDescription(x=>x.MpNameExisting)]].ToString() },
{ _localizer[_dto.GetMemberDescription(x=>x.ExistingMpParty)], (row, item) => item.ExistingMpParty = row[_localizer[_dto.GetMemberDescription(x=>x.ExistingMpParty)]].ToString() },
{ _localizer[_dto.GetMemberDescription(x=>x.ExistingMpTerms)], (row, item) => item.ExistingMpTerms = row[_localizer[_dto.GetMemberDescription(x=>x.ExistingMpTerms)]].ToString() },


{ _localizer[_dto.GetMemberDescription(x=>x.OtherPastMps)], (row, item) => item.OtherPastMps = row[_localizer[_dto.GetMemberDescription(x=>x.OtherPastMps)]].ToString() }, 

//{ _localizer[_dto.GetMemberDescription(x=>x.ReadCount)], (row, item) => item.ReadCount = row[_localizer[_dto.GetMemberDescription(x=>x.ReadCount)]].ToString() }, 
//{ _localizer[_dto.GetMemberDescription(x=>x.WriteCount)], (row, item) => item.WriteCount = row[_localizer[_dto.GetMemberDescription(x=>x.WriteCount)]].ToString() }, 

            }, _localizer[_dto.GetClassDescription()]);
            if (result.Succeeded && result.Data is not null)
            {
                foreach (var dto in result.Data)
                {
                    var exists = await _context.VoteConstituencies.AnyAsync(x => x.Name == dto.Name, cancellationToken);
                    if (!exists)
                    {
                        var item = _mapper.Map<VoteConstituency>(dto);
                        // add create domain events if this entity implement the IHasDomainEvent interface
				        // item.AddDomainEvent(new ConstituencyCreatedEvent(item));
                        await _context.VoteConstituencies.AddAsync(item, cancellationToken);
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
        public async Task<Result<byte[]>> Handle(CreateConstituenciesTemplateCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement ImportConstituenciesCommandHandler method 
            var fields = new string[] {
                   // TODO: Define the fields that should be generate in the template, for example:
                   _localizer[_dto.GetMemberDescription(x=>x.State)], 
_localizer[_dto.GetMemberDescription(x=>x.Name)], 
_localizer[_dto.GetMemberDescription(x=>x.MpNameExisting)], 
_localizer[_dto.GetMemberDescription(x=>x.OtherPastMps)], 
_localizer[_dto.GetMemberDescription(x=>x.Description)], 
_localizer[_dto.GetMemberDescription(x=>x.ReadsCount)], 
_localizer[_dto.GetMemberDescription(x=>x.VoteCount)], 

                };
            var result = await _excelService.CreateTemplateAsync(fields, _localizer[_dto.GetClassDescription()]);
            return await Result<byte[]>.SuccessAsync(result);
        }
    }

