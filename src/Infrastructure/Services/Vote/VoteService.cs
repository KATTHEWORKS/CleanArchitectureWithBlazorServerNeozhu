//#define HOSPITAL_SYSTEM
#define VOTING_SYSTEM


using CleanArchitecture.Blazor.Infrastructure.Common.Extensions;
using FluentEmail.Core;
using static CleanArchitecture.Blazor.Domain.Entities.V_VoteSummary;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Vote;

public interface IVoteService
{
    Task<V_Vote?> AddOrUpdate(V_Vote vote);
    Task<int> DeleteOfUser(string userId);//avoid from frontend
    Task<V_Vote?> ReadByUserId(string userId);
    //Task<V_Vote?> ReadByUserId(string userId, int constituencyId);
    //Task<V_Vote?> ReadByVoteId(int id);

}
#if VOTING_SYSTEM
public class VoteService(IApplicationDbContext context) : IVoteService
{
    // private readonly IVoteSummaryService _summaryServices = summaryService;
    //for self read all properties
    //ideally 1 user can updatedVote at one place only 
    //later we can allow for multiple

    //TODO Need to maintiaing system_type of MP...
    public async Task<V_Vote?> ReadByUserId(string userId)
    {
        if (userId.IsNullOrEmptyAndTrimSelf())
            return null;
        var res = await context.V_Votes.AsNoTracking()//add selection of specific columns rather all
            .FirstOrDefaultAsync(x => x.UserId == userId);
        //.FirstAsync(x => x.UserId == userId);//fails if not found

        if (res != null && res.VoteKPIComments != null && res.VoteKPIComments.Count > 0)
        {
            res.VoteKPIComments.ForEach(x => { res.VoteKPIRatingComments!.Find(k => k.KPI == x.KPI).Comment = x.Comment; });
            res.VoteKPIComments = [];
        }

        return res;
    }
    public async Task<V_Vote?> AddOrUpdate(V_Vote vote)
    {
        if (vote is null || vote.UserId is null || vote.ConstituencyId <= 0) return null;

        //make sure only 1 updatedVote exists,if anything else remove previous or block itself to add on screen... but currently allowing multiple votes

        var existing = await ReadByUserIdAll(vote.UserId);//this can be bypasssed based on cache considering for improvements
        //if same constituency exists then go for update
        //if different contt exists then delete & add here

        //todo need to confirm this,if ok do same in UI also on any change immediately 
        vote.Rating = (sbyte)(vote.VoteKPIRatingComments.Sum(x => x.Rating) / vote.VoteKPIRatingComments.Count(x => x.Rating != null));
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
                var deleted = await context.V_Votes.Where(x => x.UserId == vote.UserId).Skip(1).ExecuteDeleteAsync();
            }
            var existingVote = existing.First();

            if (existingVote.VoteKPIRatingComments != null && existingVote.VoteKPIRatingCommentsDelta == null)
            {
                existingVote.VoteKPIRatingCommentsDelta = existingVote.VoteKPIRatingComments;
                existingVote.ConstituencyIdDelta = existingVote.ConstituencyId;

                var result1 = await context.V_Votes.Where(x => x.Id == existingVote.Id)
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
                var result1 = await context.V_Votes.Where(x => x.Id == existingVote.Id)
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
        var addedVote = await context.V_Votes.AddAsync(vote);

        var result = await context.SaveChangesAsync();
        //await _summaryServices.AddForNewVote(addedVote.Entity);
        //dont call summary for every vote,instead load summary for every 15 min once either call based or trigger based
        return addedVote.Entity;
    }


    //since now 1 user-1vote so mostly this is not required
    //public async Task<V_Vote?> ReadByUserId(string userId, int constituencyId)
    //{
    //    if (userId.IsNullOrEmptyAndTrimSelf() || constituencyId == 0)
    //        return null;
    //    var res = await _context.V_Votes.AsNoTracking()
    //        .Where(x => x.UserId == userId && x.ConstituencyId == constituencyId).FirstOrDefaultAsync();
    //    return res;
    //}
    //public async Task<V_Vote?> ReadByVoteId(int id)
    //{
    //    var res = await _context.V_Votes.FindAsync(id);
    //    return res;
    //}

    //this is only for internal purpose with tracking
    private async Task<List<V_Vote>> ReadByUserIdAll(string userId)
    {
        if (userId.IsNullOrEmptyAndTrimSelf())
            return [];
        var res = await context.V_Votes.AsNoTracking().Where(x => x.UserId == userId).ToListAsync();
        return res;
    }


    public async Task<int> DeleteOfUser(string userId)//avoid this from frontend
    {
        if (await context.V_Votes.AnyAsync(x => x.UserId == userId))
        {
            //instead of hard delete,just marking value as null by moving to delta for later removal case
            // _context.V_Votes.Remove(existingVote);

            var result = await context.V_Votes.Where(x => x.UserId == userId).ExecuteUpdateAsync(x =>
            x.SetProperty(y => y.ConstituencyIdDelta, z => z.ConstituencyId)
            .SetProperty(y => y.ConstituencyId, 0)
            .SetProperty(y => y.VoteKPIRatingCommentsDelta, z => z.VoteKPIRatingComments)
            .SetProperty(y => y.VoteKPIRatingComments, new List<VoteKPIRatingComment>())
            );

            //var result = (await _context.SaveChangesAsync()) > 0;
            ////ideally not to do this
            //if (existingVote.VoteKPIRatingComments.Count > 0)
            //    await _summaryServices.Update(new ToAddRemove()
            //    {
            //        CommentCountDifference = -existingVote.VoteKPIComments.Count,
            //        ConstituencyIdToRemove = existingVote.ConstituencyId,
            //        ToRemove = existingVote.VoteKPIRatingComments.Select(x => (x.KPI, x.Rating)).ToList()
            //    });
            return result;
        }
        return 0;
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
   
   
    */
}
#endif