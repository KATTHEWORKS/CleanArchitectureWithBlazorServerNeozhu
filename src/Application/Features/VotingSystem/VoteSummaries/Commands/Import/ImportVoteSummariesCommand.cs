// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.VotingSystem.VoteSummaries.DTOs;
using CleanArchitecture.Blazor.Application.Features.VotingSystem.VoteSummaries.Caching;

namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.VoteSummaries.Commands.Import;

    public class ImportVoteSummariesCommand: ICacheInvalidatorRequest<Result<int>>
    {
        public string FileName { get; set; }
        public byte[] Data { get; set; }
        public string CacheKey => VoteSummaryCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => VoteSummaryCacheKey.SharedExpiryTokenSource();
        public ImportVoteSummariesCommand(string fileName,byte[] data)
        {
           FileName = fileName;
           Data = data;
        }
    }
    public record class CreateVoteSummariesTemplateCommand : IRequest<Result<byte[]>>
    {
 
    }

    public class ImportVoteSummariesCommandHandler : 
                 IRequestHandler<CreateVoteSummariesTemplateCommand, Result<byte[]>>,
                 IRequestHandler<ImportVoteSummariesCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<ImportVoteSummariesCommandHandler> _localizer;
        private readonly IExcelService _excelService;
        private readonly VoteSummaryDto _dto = new();

        public ImportVoteSummariesCommandHandler(
            IApplicationDbContext context,
            IExcelService excelService,
            IStringLocalizer<ImportVoteSummariesCommandHandler> localizer,
            IMapper mapper
            )
        {
            _context = context;
            _localizer = localizer;
            _excelService = excelService;
            _mapper = mapper;
        }
        #nullable disable warnings
        public async Task<Result<int>> Handle(ImportVoteSummariesCommand request, CancellationToken cancellationToken)
        {
        var result = new VoteSummaryDto();
//           var result = await _excelService.ImportAsync(request.Data, mappers: new Dictionary<string, Func<DataRow, VoteSummaryDto, object?>>
//            {
//                { _localizer[_dto.GetMemberDescription(x=>x.ConstituencyId)], 
////                   (row, item) => item.ConstituencyId = row[_localizer[_dto.GetMemberDescription(x=>x.ConstituencyId)]].ToString() }, 
////{ _localizer[_dto.GetMemberDescription(x=>x.CommentsCount)], (row, item) => item.CommentsCount = row[_localizer[_dto.GetMemberDescription(x=>x.CommentsCount)]].ToString() }, 
////{ _localizer[_dto.GetMemberDescription(x=>x.VotesCount)], (row, item) => item.VotesCount = row[_localizer[_dto.GetMemberDescription(x=>x.VotesCount)]].ToString() }, 

//            }
//               //, _localizer[_dto.GetClassDescription()]
//            );
            if (result.Succeeded && result.Data is not null)
            {
                foreach (var dto in result.Data)
                {
                //var exists = await _context.VoteSummaries.AnyAsync(x => x.Name == dto.Name, cancellationToken);
                var exists = await _context.VoteSummaries.AnyAsync(x => x.Id == dto.Id, cancellationToken);
                if (!exists)
                    {
                        var item = _mapper.Map<VoteSummary>(dto);
                        // add create domain events if this entity implement the IHasDomainEvent interface
				        // item.AddDomainEvent(new VoteSummaryCreatedEvent(item));
                        await _context.VoteSummaries.AddAsync(item, cancellationToken);
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
        public async Task<Result<byte[]>> Handle(CreateVoteSummariesTemplateCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement ImportVoteSummariesCommandHandler method 
            var fields = new string[] {
                   // TODO: Define the fields that should be generate in the template, for example:
                   _localizer[_dto.GetMemberDescription(x=>x.ConstituencyId)], 
_localizer[_dto.GetMemberDescription(x=>x.CommentsCount)], 
_localizer[_dto.GetMemberDescription(x=>x.VotesCount)], 

                };
            var result = await _excelService.CreateTemplateAsync(fields, _localizer[_dto.GetClassDescription()]);
            return await Result<byte[]>.SuccessAsync(result);
        }
    }

