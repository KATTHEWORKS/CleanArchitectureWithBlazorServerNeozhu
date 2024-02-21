namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.Specifications;
#nullable disable warnings
public class VoteByIdSpecification : Specification<Vote>
{
    public VoteByIdSpecification(int id)
    {
       Query.Where(q => q.Id == id);
    }
}