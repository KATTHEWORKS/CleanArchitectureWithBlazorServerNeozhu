// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.DTOs;
using CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.Caching;

namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.Commands.Import;

    public class ImportVotesCommand: ICacheInvalidatorRequest<Result<int>>
    {
        public string FileName { get; set; }
        public byte[] Data { get; set; }
        public string CacheKey => VoteCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => VoteCacheKey.SharedExpiryTokenSource();
        public ImportVotesCommand(string fileName,byte[] data)
        {
           FileName = fileName;
           Data = data;
        }
    }
    public record class CreateVotesTemplateCommand : IRequest<Result<byte[]>>
    {
 
    }

    public class ImportVotesCommandHandler : 
                 IRequestHandler<CreateVotesTemplateCommand, Result<byte[]>>,
                 IRequestHandler<ImportVotesCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<ImportVotesCommandHandler> _localizer;
        private readonly IExcelService _excelService;
        private readonly VoteDto _dto = new();

        public ImportVotesCommandHandler(
            IApplicationDbContext context,
            IExcelService excelService,
            IStringLocalizer<ImportVotesCommandHandler> localizer,
            IMapper mapper
            )
        {
            _context = context;
            _localizer = localizer;
            _excelService = excelService;
            _mapper = mapper;
        }
        #nullable disable warnings
        public async Task<Result<int>> Handle(ImportVotesCommand request, CancellationToken cancellationToken)
        {

           var result = await _excelService.ImportAsync(request.Data, mappers: new Dictionary<string, Func<DataRow, VoteDto, object?>>
            {
                { _localizer[_dto.GetMemberDescription(x=>x.UserId)], (row, item) => item.UserId = row[_localizer[_dto.GetMemberDescription(x=>x.UserId)]].ToString() }, 
//{ _localizer[_dto.GetMemberDescription(x=>x.ConstituencyId)], (row, item) => item.ConstituencyId = row[_localizer[_dto.GetMemberDescription(x=>x.ConstituencyId)]].ToString() }, 
//{ _localizer[_dto.GetMemberDescription(x=>x.ConstituencyIdDelta)], (row, item) => item.ConstituencyIdDelta = row[_localizer[_dto.GetMemberDescription(x=>x.ConstituencyIdDelta)]].ToString() }, 

            }, _localizer[_dto.GetClassDescription()]);
            if (result.Succeeded && result.Data is not null)
            {
                foreach (var dto in result.Data)
                {
                    //var exists = await _context.Votes.AnyAsync(x => x.Name == dto.Name, cancellationToken);
                //todo
                var exists = await _context.Votes.AnyAsync(x => x.Id == 0, cancellationToken);
                if (!exists)
                    {
                        var item = _mapper.Map<Vote>(dto);
                        // add create domain events if this entity implement the IHasDomainEvent interface
				        // item.AddDomainEvent(new VoteCreatedEvent(item));
                        await _context.Votes.AddAsync(item, cancellationToken);
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
        public async Task<Result<byte[]>> Handle(CreateVotesTemplateCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement ImportVotesCommandHandler method 
            var fields = new string[] {
                   // TODO: Define the fields that should be generate in the template, for example:
                   _localizer[_dto.GetMemberDescription(x=>x.UserId)], 
_localizer[_dto.GetMemberDescription(x=>x.ConstituencyId)], 
_localizer[_dto.GetMemberDescription(x=>x.ConstituencyIdDelta)], 

                };
            var result = await _excelService.CreateTemplateAsync(fields, _localizer[_dto.GetClassDescription()]);
            return await Result<byte[]>.SuccessAsync(result);
        }
    }

