//#define HOSPITAL_SYSTEM
#define VOTING_SYSTEM


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Domain.Common.Enums;
using CleanArchitecture.Blazor.Domain.Entities;
using LazyCache;
using static CleanArchitecture.Blazor.Domain.Entities.V_VoteSummary;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Vote;

public interface IVoteSummaryService
{
    Task<bool> AddForNewVote(V_Vote vote);
    Task<V_VoteSummary> ReadByConstituencyId(int constituencyId);
    Task<V_VoteSummary> ReadBySummaryId(int id);
    Task<bool> Update(ToAddRemove diff);
}
#if VOTING_SYSTEM
public class VoteSummaryService(IApplicationDbContext context, IAppCache cache) : IVoteSummaryService
{
    //private readonly IApplicationDbContext _context = context;

    private const string VoteSummaryCacheKey = "all-Summary";

    public async Task LoadSummaryFromAllVotesFirstTime()
    //first time or weekly once to reload complete summary db ondemand only with backup
    {//very costly heavy operation
        var allVotes = await context.V_Votes.ToListAsync();
        //improve this whole logic based on fetching constituency wise votes ,while fetching itself write to result set and make a sigle call to summary db... prefarrably delta db...once ready shift all to main table...using some switch in between
        foreach (var vote in allVotes)
        {

            var existing = await ReadByConstituencyId(vote.ConstituencyId);

            if (existing == null)//case1
            {
                var temp = new List<VoteSummary_KPIVote>();
                vote.VoteKPIRatingComments.ForEach(k => temp.Add(new VoteSummary_KPIVote() { KPI = k.KPI, RatingTypeCountsList = [new((sbyte)k.Rating, 1)] }));
                var new1 = new V_VoteSummary()
                {
                    ConstituencyId = vote.ConstituencyId,
                    KPIVotes = temp,
                    CommentsCount = vote.VoteKPIComments.Count == 0 ? 0 : 1
                    //,AggregateVote  had top check whether its added to db by generating or not
                };
                await context.V_VoteSummarys.AddAsync(new1);
                //return (await context.SaveChangesAsync()) > 0;
                continue;
            }
            else//case2 paqrticular MP details existing,now adding particular user vote counts only
            {
                if (vote.VoteKPIComments.Count > 0)
                    existing.CommentsCount += 1;

                vote.VoteKPIRatingComments.ForEach((Action<VoteKPIRatingComment>)(k =>
                {
                    //MP details exists but again 2 case
                    //case2.1 kpi details existing for mp
                    var kpiDetailsExisiting = existing.KPIVotes.FirstOrDefault<VoteSummary_KPIVote>(x => x.KPI == k.KPI);
                    if (kpiDetailsExisiting is null)
                    {
                        var newSummary = new VoteSummary_KPIVote()
                        {
                            KPI = k.KPI,
                            RatingTypeCountsList = [new((sbyte)k.Rating, 1)]

                        };
                        existing.KPIVotes.Add(newSummary);
                    }
                    else
                    {
                        var existingRatingType = kpiDetailsExisiting.RatingTypeCountsList.Find((Predicate<VoteSummary_KPIVote.RatingTypeCounts>)(x => x.RatingTypeByte == k.Rating));
                        if (existingRatingType is not null)
                        {
                            existingRatingType.Count += 1;
                        }
                        else//its null so not existing RatingTypeCountsList for particular 
                        {
                            if (k.Rating is not null)
                                kpiDetailsExisiting.RatingTypeCountsList.Add(new VoteSummary_KPIVote.RatingTypeCounts(k.Rating ?? (sbyte)RatingEnum.OkOk, 1));
                        }
                    }

                }));

                var result = context.V_VoteSummarys.Update(existing);
                await context.SaveChangesAsync();
                continue;
            }
        }
    }

    public async Task LoadDeltaDifferenceToSummary()
    //this executes for every frequency of 10 minues ones
    {
        //1.take last update time of summary & get least time among all(-5 minutes threshould)
        //2.fetch all records later than that summary leasttime
        //created>summTime or modified>sumtime or delta!=null
        //3.get difference with delta and attach to db
        //4.add those to db
        //5.update back the step2 records delta(id & string both) as null

        //step1
        var lastCreated = await context.V_VoteSummarys.MaxAsync(x => x.Created);
        var lastModified = await context.V_VoteSummarys.MaxAsync(x => x.Modified);

        lastCreated = lastCreated.AddMinutes(-5);
        if (lastModified != null)
            lastModified = lastModified.Value.AddMinutes(-5);

        var leastTime = lastCreated < lastModified ? lastCreated : lastModified;
        //step2
        var deltaToLoad = await context.V_Votes.Where(x => x.Created > leastTime || x.Modified > leastTime || x.ConstituencyIdDelta != null || x.VoteKPIRatingCommentsDelta != null).ToListAsync();


    }

    //this can be cached at client side and refresh their itself after every 15 minuts from client side interval as well
    public async Task<List<V_VoteSummary>> All()
    {
        if (cache is not null && context is not null)
        {
            //todo had to add logic of reading from vote db and converting into summary
            return await cache.GetOrAddAsync(VoteSummaryCacheKey,
                async () => await context.V_VoteSummarys!.AnyAsync() ? await context.V_VoteSummarys.ToListAsync() : [], TimeSpan.FromMinutes(14));

        }
        return [];
    }

    //for self read all properties
    //ideally 1 user can vote at one place only 
    //later we can allow for multiple
    public async Task<V_VoteSummary> ReadByConstituencyId(int constituencyId)
    {
        var res = await context.V_VoteSummarys.Where(x => x.ConstituencyId == constituencyId).FirstOrDefaultAsync();
        return res;
    }

    public async Task<V_VoteSummary> ReadBySummaryId(int id)
    {
        var res = await context.V_VoteSummarys.FindAsync(id);
        return res;
    }
    public async Task<bool> AddForNewVote(V_Vote vote)
    {
        //means user first time adding,2 cases
        //case1: partiucular MP earlier not existing
        //case2: particular MP existing ,so go for update votes

        var existing = await ReadByConstituencyId(vote.ConstituencyId);

        if (existing == null)//case1
        {
            var temp = new List<VoteSummary_KPIVote>();
            vote.VoteKPIRatingComments.ForEach(k => temp.Add(new VoteSummary_KPIVote() { KPI = k.KPI, RatingTypeCountsList = [new((sbyte)k.Rating, 1)] }));
            var new1 = new V_VoteSummary()
            {
                ConstituencyId = vote.ConstituencyId,
                KPIVotes = temp,
                CommentsCount = vote.VoteKPIComments.Count == 0 ? 0 : 1
                //,AggregateVote  had top check whether its added to db by generating or not
            };
            await context.V_VoteSummarys.AddAsync(new1);
            return (await context.SaveChangesAsync()) > 0;
        }
        else//case2 paqrticular MP details existing,now adding particular user vote counts only
        {
            if (vote.VoteKPIComments.Count > 0)
                existing.CommentsCount += 1;

            vote.VoteKPIRatingComments.ForEach((Action<VoteKPIRatingComment>)(k =>
            {
                //MP details exists but again 2 case
                //case2.1 kpi details existing for mp
                var kpiDetailsExisiting = existing.KPIVotes.FirstOrDefault<VoteSummary_KPIVote>(x => x.KPI == k.KPI);
                if (kpiDetailsExisiting is null)
                {
                    var newSummary = new VoteSummary_KPIVote()
                    {
                        KPI = k.KPI,
                        RatingTypeCountsList = [new((sbyte)k.Rating, 1)]

                    };
                    existing.KPIVotes.Add(newSummary);
                }
                else
                {
                    var existingRatingType = kpiDetailsExisiting.RatingTypeCountsList.Find((Predicate<VoteSummary_KPIVote.RatingTypeCounts>)(x => x.RatingTypeByte == k.Rating));
                    if (existingRatingType is not null)
                    {
                        existingRatingType.Count += 1;
                    }
                    else//its null so not existing RatingTypeCountsList for particular 
                    {
                        if (k.Rating is not null)
                            kpiDetailsExisiting.RatingTypeCountsList.Add(new VoteSummary_KPIVote.RatingTypeCounts(k.Rating ?? (sbyte)RatingEnum.OkOk, 1));
                    }
                }

            }));

            var result = context.V_VoteSummarys.Update(existing);
            return (await context.SaveChangesAsync()) > 0;
        }

    }

    public async Task<bool> Update(ToAddRemove diff)
    {
        if (diff == null) return false;
        var existing = await ReadByConstituencyId(diff.ConstituencyId);
        if (existing == null)
        {//assuming this case wont appear anytime still //todo had to verify
            var temp = new List<VoteSummary_KPIVote>();
            if (diff.ToAdd is not null)
            {
                diff.ToAdd.ForEach(k => temp.Add(new VoteSummary_KPIVote() { KPI = k.KPI, RatingTypeCountsList = [new((sbyte)k.Rating!, 1)] }));
                var new1 = new V_VoteSummary()
                {
                    ConstituencyId = diff.ConstituencyId,
                    KPIVotes = temp,
                    CommentsCount = diff.CommentCountDifference > 0 ? 1 : 0
                    //,AggregateVote  had top check whether its added to db by generating or not
                };
                await context.V_VoteSummarys.AddAsync(new1);
                return (await context.SaveChangesAsync()) > 0;
            }
            return false;
        }
        else
        {
            if (diff.CommentCountDifference > 0)
                existing.CommentsCount += diff.CommentCountDifference ?? 0;
            diff.ToAdd?.ForEach(a =>
            {
                var existingKpi = existing.KPIVotes.Find(e => e.KPI == a.KPI);
                if (existingKpi is not null)
                {
                    var toAdd = existingKpi.RatingTypeCountsList.Find(k => k.RatingTypeByte == a.Rating);
                    if (toAdd is not null)
                        toAdd.Count += 1;
                    //TODO if not exisitn then had to add
                }
            });
            diff.ToRemove?.ForEach(a =>
            {
                var existingKpi = existing.KPIVotes.Find(e => e.KPI == a.KPI);
                if (existingKpi is not null)
                {
                    var toRemove = existingKpi.RatingTypeCountsList.Find(k => k.RatingTypeByte == a.Rating);
                    if (toRemove is not null)
                    {
                        toRemove.Count -= 1;
                        if (toRemove.Count < 0)
                        {
                            //TODO in that case it can be removed but still leaving to know for tracking purpose
                        }
                    }
                }
            });
            var result = context.V_VoteSummarys.Update(existing);
            return (await context.SaveChangesAsync()) > 0;
        }
    }

    //this is for the sake of making 1 db call for existing constituency to different update case 
    /*
    public async Task<bool> Update(ToAddRemove toAdd, ToAddRemove toRemove)
    {
        if (toAdd == null && toRemove == null) return false;
        if (toAdd == null) return await Update(toRemove);
        if (toRemove == null) return await Update(toAdd);

        var existing = await ReadByConstituencyId(diff.MPId);
        if (existing == null)
        {//assuming this case wont appear anytime still //todo had to verify
            var temp = new List<VoteSummary_KPIVote>();
            if (diff.ToAdd is not null)
            {
                diff.ToAdd.ForEach(k => temp.Add(new VoteSummary_KPIVote() { KPI = k.KPI, RatingTypeCountsList = [new((sbyte)k.Rating!, 1)] }));
                var new1 = new V_VoteSummary()
                {
                    ConstituencyId = diff.MPId,
                    KPIVotes = temp,
                    CommentsCount = diff.CommentCountDifference > 0 ? 1 : 0
                    //,AggregateVote  had top check whether its added to db by generating or not
                };
                await _context.V_VoteSummarys.AddAsync(new1);
                return (await _context.SaveChangesAsync()) > 0;
            }
            return false;
        }
        else
        {
            if (diff.CommentCountDifference > 0)
                existing.CommentsCount += diff.CommentCountDifference ?? 0;
            diff.ToAdd?.ForEach(a =>
            {
                var existingKpi = existing.KPIVotes.Find(e => e.KPI == a.KPI);
                if (existingKpi is not null)
                {
                    var toAdd = existingKpi.RatingTypeCountsList.Find(k => k.RatingTypeByte == a.Rating);
                    if (toAdd is not null)
                        toAdd.Count += 1;
                    //TODO if not exisitn then had to add
                }
            });
            diff.ToRemove?.ForEach(a =>
            {
                var existingKpi = existing.KPIVotes.Find(e => e.KPI == a.KPI);
                if (existingKpi is not null)
                {
                    var toRemove = existingKpi.RatingTypeCountsList.Find(k => k.RatingTypeByte == a.Rating);
                    if (toRemove is not null)
                    {
                        toRemove.Count -= 1;
                        if (toRemove.Count < 0)
                        {
                            //TODO in that case it can be removed but still leaving to know for tracking purpose
                        }
                    }
                }
            });
            var result = _context.V_VoteSummarys.Update(existing);
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
    */







    //usually delete of summary wont be called anytime,so commenting
    //public async Task<bool> Delete(int id)
    //{
    //    var existingSummmary = _context.V_VoteSummarys.Find(id);
    //    if (existingSummmary == null) return false;

    //    _context.V_VoteSummarys.Remove(existingSummmary);
    //    return (await _context.SaveChangesAsync()) > 0;
    //}
}
#endif