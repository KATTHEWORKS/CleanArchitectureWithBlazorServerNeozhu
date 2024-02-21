// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.VotingSystem.VoteSummaries.DTOs;
using CleanArchitecture.Blazor.Application.Features.VotingSystem.VoteSummaries.Specifications;
using CleanArchitecture.Blazor.Application.Features.VotingSystem.VoteSummaries.Queries.Pagination;

namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.VoteSummaries.Queries.Export;

public class ExportVoteSummariesQuery : VoteSummaryAdvancedFilter, IRequest<Result<byte[]>>
{
      public VoteSummaryAdvancedSpecification Specification => new VoteSummaryAdvancedSpecification(this);
}
    
public class ExportVoteSummariesQueryHandler :
         IRequestHandler<ExportVoteSummariesQuery, Result<byte[]>>
{
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IExcelService _excelService;
        private readonly IStringLocalizer<ExportVoteSummariesQueryHandler> _localizer;
        private readonly VoteSummaryDto _dto = new();
        public ExportVoteSummariesQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IExcelService excelService,
            IStringLocalizer<ExportVoteSummariesQueryHandler> localizer
            )
        {
            _context = context;
            _mapper = mapper;
            _excelService = excelService;
            _localizer = localizer;
        }
        #nullable disable warnings
        public async Task<Result<byte[]>> Handle(ExportVoteSummariesQuery request, CancellationToken cancellationToken)
        {
            var data = await _context.VoteSummaries.ApplySpecification(request.Specification)
                       .OrderBy($"{request.OrderBy} {request.SortDirection}")
                       .ProjectTo<VoteSummaryDto>(_mapper.ConfigurationProvider)
                       .AsNoTracking()
                       .ToListAsync(cancellationToken);
            var result = await _excelService.ExportAsync(data,
                new Dictionary<string, Func<VoteSummaryDto, object?>>()
                {
                    // TODO: Define the fields that should be exported, for example:
                    {_localizer[_dto.GetMemberDescription(x=>x.ConstituencyId)],item => item.ConstituencyId}, 
{_localizer[_dto.GetMemberDescription(x=>x.CommentsCount)],item => item.CommentsCount}, 
{_localizer[_dto.GetMemberDescription(x=>x.VotesCount)],item => item.VotesCount}, 

                }
                , _localizer[_dto.GetClassDescription()]);
            return await Result<byte[]>.SuccessAsync(result);
        }
}
