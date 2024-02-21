namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.VoteSummaries.Specifications;
#nullable disable warnings
public class VoteSummaryByIdSpecification : Specification<VoteSummary>
{
    public VoteSummaryByIdSpecification(int id)
    {
       Query.Where(q => q.Id == id);
    }
}