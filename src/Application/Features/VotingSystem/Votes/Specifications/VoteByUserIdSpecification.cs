namespace CleanArchitecture.Blazor.Application.Features.VotingSystem.Votes.Specifications;
#nullable disable warnings
public class VoteByUserIdSpecification : Specification<Vote>
{
    public VoteByUserIdSpecification(string id)
    {
       Query.Where(q => q.UserId == id);
    }
}