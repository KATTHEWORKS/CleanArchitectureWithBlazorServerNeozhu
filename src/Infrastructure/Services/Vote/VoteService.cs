//#define HOSPITAL_SYSTEM
#define VOTING_SYSTEM


using CleanArchitecture.Blazor.Infrastructure.Common.Extensions;
using FluentEmail.Core;
using static CleanArchitecture.Blazor.Domain.Entities.V_VoteSummary;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Vote;

public interface IVoteService
{
    Task<V_Vote?> AddOrUpdate(V_Vote vote);
    Task<bool> Delete(int id);
    Task<V_Vote?> ReadByUserId(string userId);
    Task<V_Vote?> ReadByUserId(string userId, int constituencyId);
    Task<V_Vote?> ReadByVoteId(int id);

}
#if VOTING_SYSTEM
public class VoteService(IApplicationDbContext context, IVoteSummaryService summaryService) : IVoteService
{
    private readonly IApplicationDbContext _context = context;
    private readonly IVoteSummaryService _summaryServices = summaryService;
    //for self read all properties
    //ideally 1 user can updatedVote at one place only 
    //later we can allow for multiple

    //TODO Need to maintiaing system_type of MP...
    public async Task<V_Vote?> ReadByUserId(string userId)
    {
        if (userId.IsNullOrEmptyAndTrimSelf())
            return null;
        var res = await _context.V_Votes.AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId);
        //.FirstAsync(x => x.UserId == userId);//fails if not found

        if (res!=null && res.VoteKPIComments!=null &&  res.VoteKPIComments.Count > 0)
        {
            res.VoteKPIComments = [];
            res.VoteKPIComments.ForEach(x => { res.VoteKPIRatingComments!.Find(k => k.KPI == x.KPI).Comment = x.Comment; });
        }
           
        return res;
    }


    public async Task<V_Vote?> ReadByUserId(string userId, int constituencyId)
    {
        if (userId.IsNullOrEmptyAndTrimSelf() || constituencyId == 0)
            return null;
        var res = await _context.V_Votes.AsNoTracking()
            .Where(x => x.UserId == userId && x.ConstituencyId == constituencyId).FirstOrDefaultAsync();
        return res;
    }
    public async Task<V_Vote?> ReadByVoteId(int id)
    {
        var res = await _context.V_Votes.FindAsync(id);
        return res;
    }
    public async Task<V_Vote?> AddOrUpdate(V_Vote vote)
    {
        if (vote is null || vote.UserId is null || vote.ConstituencyId <= 0) return null;

        //make sure only 1 updatedVote exists,if anything else remove previous or block itself to add on screen... but currently allowing multiple votes

        var existing = await ReadByUserIdAll(vote.UserId);//this can be bypasssed based on cache considering for improvements
        //if same constituency exists then go for update
        //if different contt exists then delete & add here

        vote.VoteKPIComments = vote.VoteKPIRatingComments?.Where(c => c.Rating != null && !string.IsNullOrEmpty(c.Comment)).Select(x => new VoteKPIComment(x.KPI, x.Comment!)).ToList();
        if (vote.VoteKPIComments != null && vote.VoteKPIComments.Count > 0)
            vote.VoteKPIRatingComments?.Where(c => c.Rating != null && !string.IsNullOrEmpty(c.Comment)).ForEach(k =>
            {
                //vote.VoteKPIComments.Add(new VoteKPIComment(k.KPI, k.Comment!));//this is not required,its already done
                k.Comment = null;
            });
        //todo can add Modified condition to track
        //todo check if no change then dont call save
        //even this is required on front end screen also with some intelligence like space check and all

        //TODO dont go for deletion instead always go for update only except if multiple row exists
        if (existing is not null && existing.Count > 0)//means something already exists
        {
            if (existing.Count > 1)
            {    //delete all except first
                var deleted = await _context.V_Votes.Where(x => x.UserId == vote.UserId).Skip(1).ExecuteDeleteAsync();
            }
            var existingVote = existing.First();

            if (existingVote.VoteKPIRatingComments != null && existingVote.VoteKPIRatingCommentsDelta == null)
            {
                existingVote.VoteKPIRatingCommentsDelta = existingVote.VoteKPIRatingComments;
                existingVote.ConstituencyIdDelta = existingVote.ConstituencyId;

                var result1 = await _context.V_Votes.Where(x => x.Id == existingVote.Id)
                .ExecuteUpdateAsync(x => x
                .SetProperty(u => u.Modified, DateTime.Now)
                .SetProperty(u => u.VoteKPIRatingCommentsDelta, existingVote.VoteKPIRatingCommentsDelta)
                .SetProperty(u => u.ConstituencyIdDelta, existingVote.ConstituencyIdDelta)
                .SetProperty(u => u.VoteKPIComments, vote.VoteKPIComments)
                .SetProperty(u => u.ConstituencyId, vote.ConstituencyId)
                .SetProperty(u => u.Rating, vote.Rating)//HAD TO CACLULATE here before saving or on display also but had to make sure
                .SetProperty(u => u.VoteKPIRatingComments, vote.VoteKPIRatingComments)
                );
            }
            else
            {
                var result1 = await _context.V_Votes.Where(x => x.Id == existingVote.Id)
                   .ExecuteUpdateAsync(x => x
                   .SetProperty(u => u.Modified, DateTime.Now)
                   .SetProperty(u => u.VoteKPIComments, vote.VoteKPIComments)
                   .SetProperty(u => u.ConstituencyId, vote.ConstituencyId)
                   .SetProperty(u => u.Rating, vote.Rating)//HAD TO CACLULATE here before saving or on display also but had to make sure
                   .SetProperty(u => u.VoteKPIRatingComments, vote.VoteKPIRatingComments)
                   );
            }

            return vote;

        }
        vote.Id = 0;//todo need to verify here
        var addedVote = await _context.V_Votes.AddAsync(vote);

        var result = await _context.SaveChangesAsync();
        //await _summaryServices.AddForNewVote(addedVote.Entity);
        //dont call summary for every vote,instead load summary for every 15 min once either call based or trigger based
        return addedVote.Entity;
    }
    private async Task<List<V_Vote>> ReadByUserIdAll(string userId)
    {
        if (userId.IsNullOrEmptyAndTrimSelf())
            return [];
        var res = await _context.V_Votes.Where(x => x.UserId == userId).ToListAsync();
        return res;
    }

    private async Task<int> DeleteOfUser(string userId)
    {
        return await _context.V_Votes.Where(x => x.UserId == userId).ExecuteDeleteAsync();
    }
    //public async Task<int> Delete(int id)
    //{
    //    return await _context.V_Votes.Where(x => x.Id == id).ExecuteDeleteAsync();
    //    //here comments count or vote count not changing
    //}
    public async Task<bool> Delete(int id)//avoid this from frontend
    {
        var existingVote = _context.V_Votes.Find(id);
        if (existingVote == null) return false;

        _context.V_Votes.Remove(existingVote);
        var result = (await _context.SaveChangesAsync()) > 0;
        if (existingVote.VoteKPIRatingComments.Count > 0)
            await _summaryServices.Update(new ToAddRemove()
            {
                CommentCountDifference = -existingVote.VoteKPIComments.Count,
                ConstituencyId = existingVote.ConstituencyId,
                ToRemove = existingVote.VoteKPIRatingComments.Select(x => (x.KPI, x.Rating)).ToList()
            });
        return result;
    }
    /*
    private async Task<V_Vote> Update(V_Vote updatedVote, V_Vote existingVote)
    {
        // var existingVote = _context.V_Votes.Find(id);
        if (existingVote == null || updatedVote == null
            || !updatedVote.VoteKPIRatingComments.Any(x => x.Rating != null)) return null;

        var result = await _context.V_Votes.Where(x => x.Id == existingVote.Id)
        .ExecuteUpdateAsync(x => x
        .SetProperty(u => u.Modified, DateTime.Now)
        .SetProperty(u => u.CommentsJsonAsString, updatedVote.CommentsJsonAsString)
        .SetProperty(u => u.ConstituencyId, updatedVote.ConstituencyId)
        .SetProperty(u => u.Rating, updatedVote.Rating)//HAD TO CACLULATE here before saving or on display also but had to make sure
        .SetProperty(u => u.VotesJsonAsString, updatedVote.VotesJsonAsString)
        );

        //TODO instead of updating summary for each request,instead lets do it for every 15+ min request once by full scan as job
        //till then this logic remains
        if (existingVote.ConstituencyId == updatedVote.ConstituencyId)
        {
            var commentsDiff = updatedVote.VoteKPIComments?.Count - existingVote.VoteKPIComments?.Count;
            var diff = GetVoteDifference(existingVote, updatedVote);
            if (diff != null &&
                (
                diff.ToAdd != null && diff.ToAdd.Any(x => x.Rating != null)
                || diff.ToRemove != null && diff.ToRemove.Any(x => x.Rating != null)
                || diff.CommentCountDifference != null))//then dont do anything to summary
            {
                if (commentsDiff != null && commentsDiff != 0)
                    diff.CommentCountDifference = (byte)commentsDiff;
                diff.ConstituencyId = existingVote.ConstituencyId;
                await _summaryServices.Update(diff);
                //then call summary update
            }
        }
        else
        {
            var toRemove = new ToAddRemove()
            {
                ConstituencyId = existingVote.ConstituencyId,
                ToRemove = existingVote.VoteKPIRatingComments.Select(x => (x.KPI, x.Rating)).ToList()
            };

            var toAdd = new ToAddRemove()
            {
                ConstituencyId = existingVote.ConstituencyId,
                ToRemove = existingVote.VoteKPIRatingComments.Select(x => (x.KPI, x.Rating)).ToList()
            };
            throw new NotImplementedException("had to make one db update call toadd from 1 consti & remove from other const");
        }
        //return result.Entity;
        return updatedVote;
    }
    Expression<Func<SetPropertyCalls<V_Vote>, SetPropertyCalls<V_Vote>>> CombineSetters(
    IEnumerable<Expression<Func<SetPropertyCalls<V_Vote>, SetPropertyCalls<V_Vote>>>> setters
)
    {
        Expression<Func<SetPropertyCalls<V_Vote>, SetPropertyCalls<V_Vote>>> expr = sett => sett;

        foreach (var expr2 in setters)
        {
            var call = (MethodCallExpression)expr2.Body;
            expr = Expression.Lambda<Func<SetPropertyCalls<V_Vote>, SetPropertyCalls<V_Vote>>>(
                Expression.Call(expr.Body, call.Method, call.Arguments),
                expr2.Parameters
            );
        }

        return expr;
    }
   
    private static ToAddRemove GetVoteDifference(V_Vote existingVote, V_Vote updatedVote)
    {
        if (existingVote == null || existingVote.VoteKPIRatingComments is null || existingVote.VoteKPIRatingComments.Count == 0)
            return new ToAddRemove() { ToAdd = updatedVote?.VoteKPIRatingComments.Select(x => (x.KPI, x.Rating)).ToList() };//to add
        if (updatedVote == null || updatedVote.VoteKPIRatingComments.Count == 0)
            //return existingVote.VoteKPIRatingComments;//to remove
            return new ToAddRemove() { ToRemove = existingVote.VoteKPIRatingComments.Select(x => (x.KPI, x.Rating)).ToList() };
        if (existingVote.VoteKPIRatingComments.Count == 0 && updatedVote.VoteKPIRatingComments.Count == 0)
        {
            //then no summary change just return from here;
            return null;
        }
        else
        {
            var existingKpis = existingVote.VoteKPIRatingComments.Select(x => (x.KPI, x.Rating)).ToList();
            var newKpis = updatedVote.VoteKPIRatingComments.Select(x => (x.KPI, x.Rating)).ToList();
            if (existingKpis == newKpis) return null;

            var toRemoveItems = existingKpis.Except(newKpis);//this wont work bcz of equality comparer addiotional handling required & had to compare rating also,still we can test

            var toAdd = new List<(int KPI, sbyte? Rating)>();
            var toRemove = new List<(int KPI, sbyte? Rating)>();

            var commonExists = existingKpis.Select(x => x.KPI).Intersect(newKpis.Select(y => y.KPI));

            if (commonExists.Any())
                existingKpis.ForEach(e =>
                {
                    var sameKpi = newKpis.Find(n => n.KPI == e.KPI);
                    if (sameKpi.Rating != null)
                    {
                        if (sameKpi.Rating == e.Rating)//then no change leave this kpi skip
                            newKpis.Remove(sameKpi);
                        else  //here rating difference is there
                        {
                            sameKpi.Rating -= e.Rating;
                            toAdd.Add(sameKpi);
                            newKpis.Remove(sameKpi);
                            //    //to current kpi add this rating value as summary update
                            //    //old was 1  newRating 2 then now new-old=2-1=1 had to be added 
                            //    //old was -1  newRating 1 then now new-old=1-(-1)=2 had to be added 
                            //    //old was 1  newRating -1 then now new-old=-1-1=-2 had to be added 
                        }
                    }
                    //else if (newKpis.Any(n => n.KPI == e.KPI))
                    //{
                    //    //add these kpi-rating values
                    //    var nn = newKpis.First(n => n.KPI == e.KPI);
                    //    nn.Rating -= e.Rating;
                    //    toAdd.Add(nn);

                    //}
                    else //means user removed particular KPI rating now,so had to remove
                    {
                        toRemove.Add(e);
                    }
                });
            if (newKpis != null && newKpis.Count > 0)
            {
                toAdd.AddRange(newKpis);
            }
            return new ToAddRemove() { ToAdd = toAdd, ToRemove = toRemove };
        }
    }
    */
}
#endif