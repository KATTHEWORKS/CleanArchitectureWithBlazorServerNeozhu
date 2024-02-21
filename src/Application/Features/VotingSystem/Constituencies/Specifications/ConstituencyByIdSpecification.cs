namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.Constituencies.Specifications;
#nullable disable warnings
public class ConstituencyByIdSpecification : Specification<Constituency>
{
    public ConstituencyByIdSpecification(int id)
    {
       Query.Where(q => q.Id == id);
    }
}