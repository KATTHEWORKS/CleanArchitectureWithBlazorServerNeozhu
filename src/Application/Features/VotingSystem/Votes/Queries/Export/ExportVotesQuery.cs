//// Licensed to the .NET Foundation under one or more agreements.
//// The .NET Foundation licenses this file to you under the MIT license.

//using CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.DTOs;
//using CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.Specifications;
//using CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.Queries.Pagination;

//namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.Queries.Export;

//public class ExportVotesQuery : VoteAdvancedFilter, IRequest<Result<byte[]>>
//{
//      public VoteAdvancedSpecification Specification => new VoteAdvancedSpecification(this);
//}
    
//public class ExportVotesQueryHandler :
//         IRequestHandler<ExportVotesQuery, Result<byte[]>>
//{
//        private readonly IApplicationDbContext _context;
//        private readonly IMapper _mapper;
//        private readonly IExcelService _excelService;
//        private readonly IStringLocalizer<ExportVotesQueryHandler> _localizer;
//        private readonly VoteDto _dto = new();
//        public ExportVotesQueryHandler(
//            IApplicationDbContext context,
//            IMapper mapper,
//            IExcelService excelService,
//            IStringLocalizer<ExportVotesQueryHandler> localizer
//            )
//        {
//            _context = context;
//            _mapper = mapper;
//            _excelService = excelService;
//            _localizer = localizer;
//        }
//        #nullable disable warnings
//        public async Task<Result<byte[]>> Handle(ExportVotesQuery request, CancellationToken cancellationToken)
//        {
//            var data = await _context.Votes.ApplySpecification(request.Specification)
//                       .OrderBy($"{request.OrderBy} {request.SortDirection}")
//                       .ProjectTo<VoteDto>(_mapper.ConfigurationProvider)
//                       .AsNoTracking()
//                       .ToListAsync(cancellationToken);
//            var result = await _excelService.ExportAsync(data,
//                new Dictionary<string, Func<VoteDto, object?>>()
//                {
//                    // TODO: Define the fields that should be exported, for example:
//                    {_localizer[_dto.GetMemberDescription(x=>x.UserId)],item => item.UserId}, 
//{_localizer[_dto.GetMemberDescription(x=>x.ConstituencyId)],item => item.ConstituencyId}, 
//{_localizer[_dto.GetMemberDescription(x=>x.ConstituencyIdDelta)],item => item.ConstituencyIdDelta}, 

//                }
//                , _localizer[_dto.GetClassDescription()]);
//            return await Result<byte[]>.SuccessAsync(result);
//        }
//}
