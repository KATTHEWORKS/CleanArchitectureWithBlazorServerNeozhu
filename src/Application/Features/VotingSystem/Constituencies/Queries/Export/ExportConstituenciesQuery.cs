// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.VotingSystem.Constituencies.DTOs;
using CleanArchitecture.Blazor.Application.Features.VotingSystem.Constituencies.Specifications;
using CleanArchitecture.Blazor.Application.Features.VotingSystem.Constituencies.Queries.Pagination;

namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.Constituencies.Queries.Export;

public class ExportConstituenciesQuery : ConstituencyAdvancedFilter, IRequest<Result<byte[]>>
{
      public ConstituencyAdvancedSpecification Specification => new ConstituencyAdvancedSpecification(this);
}
    
public class ExportConstituenciesQueryHandler :
         IRequestHandler<ExportConstituenciesQuery, Result<byte[]>>
{
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IExcelService _excelService;
        private readonly IStringLocalizer<ExportConstituenciesQueryHandler> _localizer;
        private readonly ConstituencyDto _dto = new();
        public ExportConstituenciesQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IExcelService excelService,
            IStringLocalizer<ExportConstituenciesQueryHandler> localizer
            )
        {
            _context = context;
            _mapper = mapper;
            _excelService = excelService;
            _localizer = localizer;
        }
        #nullable disable warnings
        public async Task<Result<byte[]>> Handle(ExportConstituenciesQuery request, CancellationToken cancellationToken)
        {
            var data = await _context.VoteConstituencies.ApplySpecification(request.Specification)
                       .OrderBy($"{request.OrderBy} {request.SortDirection}")
                       .ProjectTo<ConstituencyDto>(_mapper.ConfigurationProvider)
                       .AsNoTracking()
                       .ToListAsync(cancellationToken);
            var result = await _excelService.ExportAsync(data,
                new Dictionary<string, Func<ConstituencyDto, object?>>()
                {
                    // TODO: Define the fields that should be exported, for example:
{_localizer[_dto.GetMemberDescription(x=>x.State)],item => item.State}, 
{_localizer[_dto.GetMemberDescription(x=>x.Name)],item => item.Name},
{_localizer[_dto.GetMemberDescription(x=>x.Description)],item => item.Description},

{_localizer[_dto.GetMemberDescription(x=>x.MpNameExisting)],item => item.MpNameExisting},
{_localizer[_dto.GetMemberDescription(x=>x.ExistingMpParty)],item => item.ExistingMpParty},
{_localizer[_dto.GetMemberDescription(x=>x.ExistingMpTerms)],item => item.ExistingMpTerms},

{_localizer[_dto.GetMemberDescription(x=>x.OtherPastMps)],item => item.OtherPastMps}, 

{_localizer[_dto.GetMemberDescription(x=>x.ReadsCount)],item => item.ReadsCount}, 
{_localizer[_dto.GetMemberDescription(x=>x.VoteCount)],item => item.VoteCount}, 

                }
                , _localizer[_dto.GetClassDescription()]);
            return await Result<byte[]>.SuccessAsync(result);
        }
}
