namespace CleanArchitecture.Blazor.Application.Features.CardTypes.Specifications;
#nullable disable warnings
/// <summary>
/// Specification class for filtering CardTypes by their ID.
/// </summary>
public class CardTypeByIdSpecification : Specification<CardType>
{
    public CardTypeByIdSpecification(int id)
    {
       Query.Where(q => q.Id == id);
    }
}