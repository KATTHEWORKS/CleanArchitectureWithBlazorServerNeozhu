//#define HOSPITAL_SYSTEM
#define VOTING_SYSTEM


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Domain.Common.Enums;
using CleanArchitecture.Blazor.Domain.Entities;
using DocumentFormat.OpenXml.Drawing.Charts;
using LazyCache;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using static CleanArchitecture.Blazor.Domain.Entities.V_VoteSummary;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Vote;

public interface IVoteSummaryService
{
    Task<V_VoteSummary?> ReadByConstituencyId(int constituencyId);
    Task<List<V_VoteSummary>> All();
}
#if VOTING_SYSTEM
public class VoteSummaryService(IApplicationDbContext context, IAppCache cache) : IVoteSummaryService
{
    //private readonly IApplicationDbContext _context = context;

    private const string VoteSummaryCacheKey = "all-Summary";
    private static DateTime lastLoadedOn = DateTime.Now.AddHours(-2);
    private static bool DeltaLoadingInProgress = false;
    private const int RefreshFrequncyInMinutes = 2;
    //private const string VoteSummaryLoadTimeCacheKey = "all-Summary-loadedTime";
    //private static bool IsFirsttime = false;

    public async Task<V_VoteSummary?> ReadByConstituencyId(int constituencyId)
    {
        return (await All()).Find(x => x.ConstituencyId == constituencyId);
    }

    public async Task<List<V_VoteSummary>> All()
    {
        if (cache is not null && context is not null)
        {
            await RefreshDb();
            //todo had to add logic of reading from vote db and converting into summary
            return await cache.GetOrAddAsync(VoteSummaryCacheKey,
                async () => await context.V_VoteSummarys!.AnyAsync() ? await context.V_VoteSummarys.ToListAsync() : [], TimeSpan.FromMinutes(14));

        }
        return [];
    }

    private async Task RefreshDb()
    {
        try
        {
            if (DateTime.Now.Subtract(lastLoadedOn).TotalMinutes > RefreshFrequncyInMinutes && DeltaLoadingInProgress == false)
            {
                var result = "";
                if (await context.V_VoteSummarys.AnyAsync())//not first time
                    result += "Added:" + await LoadDeltaDifferenceToSummary();

                else //firsttime
                    result += "Updated:" + await LoadSummaryFromAllVotesFirstTime();
                lastLoadedOn = DateTime.Now;
                result += " at " + lastLoadedOn.ToString();
                Console.WriteLine(result);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            throw;
        }
    }
    //summary loadings
    private async Task<int> LoadSummaryFromAllVotesFirstTime()
    //first time or weekly once to reload complete summary db ondemand only with backup
    {//very costly heavy operation
        DeltaLoadingInProgress = true;
        var constituencyVotes = await context.V_Votes.GroupBy(x => x.ConstituencyId).ToListAsync();
        //improve this whole logic based on fetching constituency wise votes ,while fetching itself write to result set and make a sigle call to summary db... prefarrably delta db...once ready shift all to main table...using some switch in between

        var totalChangesCount = 0;
        foreach (var votes in constituencyVotes)
        {
            var isFirstTime = false;
            var existing = await context.V_VoteSummarys.Where(x => x.ConstituencyId == votes.Key).FirstOrDefaultAsync();
            foreach (var vote in votes)//running for 1 constituency at one time
            {
                if (existing == null)//case1
                {
                    isFirstTime = true;
                    var temp = new List<VoteSummary_KPIVote>();
                    vote.VoteKPIRatingComments.ForEach(k => temp.Add(new VoteSummary_KPIVote() { KPI = k.KPI, RatingTypeCountsList = [new((sbyte)k.Rating, 1)] }));
                    var new1 = new V_VoteSummary()
                    {
                        ConstituencyId = vote.ConstituencyId,
                        KPIVotes = temp,
                        CommentsCount = vote.VoteKPIComments.Count == 0 ? 0 : 1
                        //,AggregateVote  had top check whether its added to db by generating or not
                    };
                    //await context.V_VoteSummarys.AddAsync(new1);
                    //instead of writing to db everytime updating existing value itslef and finally updatedb
                    existing = new1;
                    //return (await context.SaveChangesAsync()) > 0;
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

                    //var result = context.V_VoteSummarys.Update(existing);
                    //await context.SaveChangesAsync();
                    //instead of writing to db everytime updating existing value itslef and finally updatedb
                }
            }
            if (isFirstTime)
            {
                var res = await context.V_VoteSummarys.AddAsync(existing);
            }
            else
            {
                var result = context.V_VoteSummarys.Update(existing);
            }
            var finalCountOf1Constituency = await context.SaveChangesAsync();
            if (finalCountOf1Constituency > 0)
            {
                totalChangesCount += finalCountOf1Constituency;
            }
        }
        return totalChangesCount;
    }

    private async Task<int> LoadDeltaDifferenceToSummary()
    //this executes for every frequency of 10 minues ones
    {
        //1.take last update time of summary & get least time among all(-5 minutes threshould)
        //2.fetch all records later than that summary leasttime
        //created>summTime or modified>sumtime or delta!=null
        //3.get difference with delta and attach to db
        //4.add those to db
        //5.update back the step2 records delta(id & string both) as null

        DeltaLoadingInProgress = true;
        //step1
        var lastCreated = await context.V_VoteSummarys.MaxAsync(x => x.Created);
        var lastModified = await context.V_VoteSummarys.MaxAsync(x => x.Modified);
        //todo improve make single db call to extract both created or modified data like GREATEST()
        lastCreated = lastCreated.AddMinutes(-5);
        var timeToFilter = lastModified == null ? lastCreated : (lastCreated > lastModified ? lastCreated : lastModified);

        //step2
        var deltaVotesToLoad = await context.V_Votes.Where(x => x.Created > timeToFilter || x.Modified > timeToFilter || x.ConstituencyIdDelta != null || x.VoteKPIRatingCommentsDelta != null).ToListAsync();

        //step3- getting difference
        var deltasToAddRemove = new List<ToAddRemove>();
        foreach (var vote in deltaVotesToLoad)
        {
            deltasToAddRemove.Add(GetDeltaDifference(vote));
        }

        //step4
        //group all toadd/remove to 1-1 single list by constituencyid grouping
        // Union of ConstituencyIds
        var allConstituencyIds = deltasToAddRemove.SelectMany(d => new[] { d.ConstituencyIdToAdd, d.ConstituencyIdToRemove }).Distinct();

        //Group the items by ConstituencyId
        var groupedItems = allConstituencyIds.Select(id =>
        {
            var toAdd = deltasToAddRemove.Where(d => d.ConstituencyIdToAdd == id).SelectMany(d => d.ToAdd ?? Enumerable.Empty<(int, sbyte?)>()).ToList();
            var toRemove = deltasToAddRemove.Where(d => d.ConstituencyIdToRemove == id).SelectMany(d => d.ToRemove ?? Enumerable.Empty<(int, sbyte?)>()).ToList();
            return new ToAddRemove
            {
                ConstituencyIdToAdd = id,
                ConstituencyIdToRemove = id,
                ToAdd = toAdd.Count != 0 ? toAdd : null,
                ToRemove = toRemove.Count != 0 ? toRemove : null
            };
        }).ToList();
        //UpdateEntyInDb by inserting & removing
        foreach (var toAddRemoveOf1Constituency in groupedItems)
        {
            var isAllFine = await UpdateEntyInDb(toAddRemoveOf1Constituency, saveChangesToDbCall: false);
        }
        var resultCount = await context.SaveChangesAsync();

        //step5 update delta as null 
        //temp & quick is update all delta as null by asssuming all executed now

        int? nl = 0;
        if (nl != 5) nl = null;
        var nlss = new List<VoteKPIRatingComment>();
        if (nl != 5) nlss = null;
        var result = await context.V_Votes.Where(x => x.ConstituencyIdDelta != null || x.VoteKPIRatingCommentsDelta != null)
              .ExecuteUpdateAsync(x => x
                .SetProperty(u => u.ConstituencyIdDelta, nl)
                .SetProperty(u => u.VoteKPIRatingCommentsDelta, nlss)
                );
        //for best case go on each
        //reason is in between execution time delta will be missed at some extenct

        //foreach (var vote in deltaVotesToLoad)
        //{
        //    context.V_Votes.executeu
        //}
        return result;
    }

    //this can be cached at client side and refresh their itself after every 15 minuts from client side interval as well

    //for self read all properties
    //ideally 1 user can vote at one place only 
    //later we can allow for multiple

    //madhu continue here for updating dii to summary
    public async Task<bool> UpdateEntyInDb(ToAddRemove diff, bool saveChangesToDbCall = false)
    {
        var resultCount = 0;
        EntityEntry<V_VoteSummary> entityEntry;
        if (diff == null) return false;
        try
        {
            //2 cases 
            //case1 same constituency id for both add/remove
            //case2 different constituencyid for add remove
            //case2 scenario wont handled here,so always for this method will recieved of same constituencyid only

            var existing = await ReadByConstituencyId(diff.ConstituencyIdToAdd);
            //case1 same const id to add or remove
            if (diff.ConstituencyIdToAdd == diff.ConstituencyIdToRemove || diff.ConstituencyIdToRemove == 0)
            {
                if (existing == null)// constituencySummary data not exists,go for insert
                {//assuming this case wont appear anytime still //todo had to verify
                    //since insertion nothing to bother about toRemove items
                    var temp = new List<VoteSummary_KPIVote>();
                    if (diff.ToAdd is not null)
                    {
                        diff.ToAdd.ForEach(k => temp.Add(new VoteSummary_KPIVote() { KPI = k.KPI, RatingTypeCountsList = [new((sbyte)k.Rating!, 1)] }));
                        var new1 = new V_VoteSummary()
                        {
                            ConstituencyId = diff.ConstituencyIdToAdd,
                            KPIVotes = temp,
                            CommentsCount = diff.CommentCountDifference > 0 ? 1 : 0
                            //,AggregateVote  had top check whether its added to db by generating or not
                        };
                        entityEntry = await context.V_VoteSummarys.AddAsync(new1);
                    }
                }
                else//constituencySummary data already exist,go for update
                {
                    if (diff.CommentCountDifference > 0)
                        existing.CommentsCount += diff.CommentCountDifference ?? 0;

                    ToAddToExistingSummary(diff, existing);
                    //if same constituency then same logic
                    //else different constitunecy then different

                    ToRemoveFromExistingSummary(diff, existing);

                    entityEntry = context.V_VoteSummarys.Update(existing);
                    //return (await context.SaveChangesAsync()) > 0;
                }
            }
            else //different const id 
            {
                Console.WriteLine("This situation should not come in the method,as its not addressed");

                ToAddToExistingSummary(diff, existing);
                entityEntry = context.V_VoteSummarys.Update(existing);
                var existingToRemove = await ReadByConstituencyId(diff.ConstituencyIdToRemove);
                //call removal
                ToRemoveFromExistingSummary(diff, existingToRemove);
                var entityEntryRemoved = context.V_VoteSummarys.Update(existingToRemove);
            }
            if (saveChangesToDbCall)
                resultCount += await context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            return false;
        }
    }

    private static void ToAddToExistingSummary(ToAddRemove diff, V_VoteSummary existing)
    {
        if (diff.ConstituencyIdToAdd > 0 && diff.ToAdd != null && diff.ToAdd.Count > 0)
        {
            diff.ToAdd?.ForEach(a =>
        {
            var existingKpi = existing.KPIVotes.Find(e => e.KPI == a.KPI);
            if (existingKpi is not null)
            {
                var toAdd = existingKpi.RatingTypeCountsList.Find(k => k.RatingTypeByte == a.Rating);
                if (toAdd is not null)
                    toAdd.Count += 1;
                //TODO if not existing then had to add
            }
        });
        }
    }

    private static void ToRemoveFromExistingSummary(ToAddRemove diff, V_VoteSummary existing)
    {//this is when user changed constituency to other constituency
        if (diff.ConstituencyIdToRemove > 0 && diff.ToRemove != null && diff.ToRemove.Count > 0)
        {
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
                        Console.WriteLine("//TODO in that case it can be removed but still leaving to know for tracking purpose");

                    }
                }
            }
        });
        }
    }

    //madhu continue here
    private static ToAddRemove GetDeltaDifference(V_Vote vote)
    {//this is for the case of having delta kpi only ,not for fresh i.e FirstTime() sewparate method 
        if (vote.ConstituencyIdDelta == null || vote.VoteKPIRatingCommentsDelta == null
            || (vote.VoteKPIRatingComments.Count == 0 && vote.VoteKPIRatingCommentsDelta.Count == 0)) return null;
        ToAddRemove toAddRemove = new ToAddRemove();

        if (vote.ConstituencyId != vote.ConstituencyIdDelta)//different constituency
        {
            toAddRemove.ConstituencyIdToRemove = vote.ConstituencyIdDelta ?? 0;
            toAddRemove.ToRemove = vote.VoteKPIRatingCommentsDelta.Select(x => (x.KPI, x.Rating)).ToList();

            toAddRemove.ConstituencyIdToAdd = vote.ConstituencyId;
            toAddRemove.ToAdd = vote.VoteKPIRatingComments.Select(x => (x.KPI, x.Rating)).ToList();
        }
        else//below section is for same constituncy
        {
            toAddRemove.ConstituencyIdToAdd = vote.ConstituencyId;
            toAddRemove.ConstituencyIdToRemove = vote.ConstituencyId;
            //if (vote.VoteKPIRatingComments.Count == 0 && vote.VoteKPIRatingCommentsDelta.Count == 0)
            //{//already covered at top
            //    //then no summary change just return from here;
            //    return null;
            //}
            //else 
            if (vote.VoteKPIRatingComments.Count == 0 && vote.VoteKPIRatingCommentsDelta.Count > 0)
            { //means nothing present ,only old exists...so has to remove now

                //mostly this case wont come ,because now no deletion of kpi
                toAddRemove.ConstituencyIdToRemove = vote.ConstituencyIdDelta ?? 0;
                toAddRemove.ToRemove = vote.VoteKPIRatingCommentsDelta.Select(x => (x.KPI, x.Rating)).ToList();
                //nothing to add,just removing 
            }
            else if (vote.VoteKPIRatingComments.Count > 0 && vote.VoteKPIRatingCommentsDelta.Count == 0)
            {//means nothing in old ,so now add all to summary 
                toAddRemove.ConstituencyIdToAdd = vote.ConstituencyId;
                toAddRemove.ToAdd = vote.VoteKPIRatingComments.Select(x => (x.KPI, x.Rating)).ToList();
                //nothing to remove or nothing on even different constituency id nothing to bother
            }
            else //means old has some data & present also has some data
            {//had to do difference... mostly this is common scenario

                var oldDeltaKpis = vote.VoteKPIRatingCommentsDelta.Select(x => (x.KPI, x.Rating)).ToList();
                var presentKpis = vote.VoteKPIRatingComments.Select(x => (x.KPI, x.Rating)).ToList();
                if (oldDeltaKpis == presentKpis) return null;

                var toRemoveItems = oldDeltaKpis.Except(presentKpis);//this wont work bcz of equality comparer addiotional handling required & had to compare rating also,still we can test

                var toAdd = new List<(int KPI, sbyte? Rating)>();
                var toRemove = new List<(int KPI, sbyte? Rating)>();

                var commonExists = oldDeltaKpis.Select(x => x.KPI).Intersect(presentKpis.Select(y => y.KPI));

                if (commonExists.Any())
                    oldDeltaKpis.ForEach(e =>
                    {
                        var sameKpi = presentKpis.Find(n => n.KPI == e.KPI);
                        if (sameKpi.Rating != null)
                        {
                            if (sameKpi.Rating == e.Rating)//then no change leave this kpi skip
                                presentKpis.Remove(sameKpi);
                            else  //here rating difference is there
                            {
                                sameKpi.Rating -= e.Rating;
                                toAdd.Add(sameKpi);
                                presentKpis.Remove(sameKpi);
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
                            toRemove.Add(e);//mostly this wont happen as of now no removal
                        }
                    });
                if (presentKpis != null && presentKpis.Count > 0)
                {
                    toAdd.AddRange(presentKpis);
                }
                toAddRemove.ToAdd = toAdd;
                toAddRemove.ToRemove = toRemove;
            }
        }
        return toAddRemove;
    }




    //mostly this will not be using anymore
    /*
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

    //mostly this will not be using anymore
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
    */
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