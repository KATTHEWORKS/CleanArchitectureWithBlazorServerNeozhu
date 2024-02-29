//#define HOSPITAL_SYSTEM
#define VOTING_SYSTEM


namespace CleanArchitecture.Blazor.Application.Features.VotingSystem;

public interface IVoteSummaryService
{
    Task<VoteSummary?> ReadByConstituencyId(int constituencyId);
    Task<List<VoteSummary>> All();
    Task RefreshSummary();
}
#if VOTING_SYSTEM
public class VoteSummaryService(IApplicationDbContext context, IAppCache cache) : IVoteSummaryService
{
    //private readonly IApplicationDbContext _context = context;

    private const string VoteSummaryCacheKey = "all-Summary";
    private static DateTime lastLoadedOn = DateTime.Now.AddHours(-2);
    private static bool DeltaLoadingInProgress = false;
    private const int RefreshFrequncyInMinutes = 1;
    //private const string VoteSummaryLoadTimeCacheKey = "all-Summary-loadedTime";
    //private static bool IsFirsttime = false;

    public async Task<VoteSummary?> ReadByConstituencyId(int constituencyId)
    {
        return (await All()).Find(x => x.ConstituencyId == constituencyId);
    }

    public async Task<List<VoteSummary>> All()
    {
        if (cache is not null && context is not null)
        {
            if (DeltaLoadingInProgress)
            {
                var r = await cache.GetAsync<List<VoteSummary>>(VoteSummaryCacheKey);
                if (r != null)
                    return r;
                else
                {
                    Console.WriteLine("had to wait,but some improvement needed");
                }
            }

            //todo had to add logic of reading from vote db and converting into summary
            //return await cache.GetOrAddAsync(VoteSummaryCacheKey,
            //    async () => await context.V_VoteSummarys!.AsNoTracking().AnyAsync() ? await context.V_VoteSummarys.AsNoTracking().ToListAsync() : [], TimeSpan.FromMinutes(RefreshFrequncyInMinutes * 2));
            return await context.VoteSummaries!.AsNoTracking().AnyAsync() ? await context.VoteSummaries.AsNoTracking().ToListAsync() : [];

        }
        return [];
    }

    public async Task RefreshSummary()
    {
        //Vote for mp,DoNotValidateAgainstSchema had to be each time calculate whole db once read all from vote & SummaryLength & QuickAccessToolbar update 
        try
        {
            //todo make it dbcontext of new so second issue will be avoided
            if (DateTime.Now.Subtract(lastLoadedOn).TotalMinutes > RefreshFrequncyInMinutes && DeltaLoadingInProgress == false)
            {
                DeltaLoadingInProgress = true;
                var result = "";
                var changesResultCount = 0;
                if (await context.VoteSummaries.AsNoTracking().AnyAsync())//not first time
                {
                    changesResultCount = await LoadDeltaDifferenceToSummary();
                    result += "Delta loading:" + changesResultCount;
                }

                else //firsttime
                {
                    changesResultCount = await LoadSummaryFromAllVotesFirstTime();
                    result += "Updated:" + changesResultCount;
                }
                if (changesResultCount > 0)
                    await UpdateVotesCount();
                lastLoadedOn = DateTime.Now;
                result += " at " + lastLoadedOn.ToString();
                DeltaLoadingInProgress = false;
                Console.WriteLine(result);
            }
        }
        catch (Exception e)
        {
            DeltaLoadingInProgress = false;
            Console.WriteLine(e.ToString());
            throw;
        }
    }
    private async Task UpdateVotesCount()
    {
        var constituencyVotes = await context.Votes.AsNoTracking().GroupBy(x => x.ConstituencyId).ToListAsync();
        if (constituencyVotes != null && constituencyVotes.Count > 0)
        {
            //todo here this can be check and update kind of instead of direct update later on 
            foreach (var item in constituencyVotes)
            {
                var votesCount = item.Count();
                var votesCountAgainst = item.Count(i => i.WishToReElectMp == false);
                var votesCountFor = item.Count(i => i.WishToReElectMp == true);
                var res = await context.VoteSummaries.Where(s => s.ConstituencyId == item.Key)
                       .ExecuteUpdateAsync(s => s
                       .SetProperty(p => p.VotesCount, votesCount)
                       .SetProperty(p => p.VotesCountAgainstExistingMp, votesCountAgainst)
                       .SetProperty(p => p.VotesCountForExistingMp, votesCountFor)
                       );
            }
            return;
        }
    }
    //summary loadings
    private async Task<int> LoadSummaryFromAllVotesFirstTime()
    //first time or weekly once to reload complete summary db ondemand only with backup
    {//very costly heavy operation
        var constituencyVotes = await context.Votes.AsNoTracking().GroupBy(x => x.ConstituencyId).ToListAsync();
        if (constituencyVotes == null || constituencyVotes.Count == 0)
        {
            return 0;
        }
        //improve this whole logic based on fetching constituency wise votes ,while fetching itself write to result set and make a sigle call to summary db... prefarrably delta db...once ready shift all to main table...using some switch in between

        var totalChangesCount = 0;
        foreach (var votes in constituencyVotes)
        {
            // var isFirstTime = false;
            var existing = await context.VoteSummaries.AsNoTracking().FirstOrDefaultAsync(x => x.ConstituencyId == votes.Key);
            foreach (var vote in votes)//running for 1 constituency at one time
            {
                if (existing == null || existing.ConstituencyId == 0)//case1
                {
                    //isFirstTime = true;
                    var temp = new List<KPIVote>();
                    if (vote.KPIRatingComments != null && vote.KPIRatingComments.Count > 0)
                        vote.KPIRatingComments.ForEach(k => temp.Add(new KPIVote() { KPI = k.KPI_Id, RatingTypeCountsList = [new((sbyte)k.Rating, 1)] }));
                    var new1 = new VoteSummary()
                    {
                        ConstituencyId = vote.ConstituencyId,
                        KPIVotes = temp,
                        CommentsCount = (vote.KPIComments == null || vote.KPIComments.Count == 0 ? 0 : 1)
                        //,AggregateVote  had top check whether its added to db by generating or not
                    };
                    //await context.V_VoteSummarys.AddAsync(new1);
                    //instead of writing to db everytime updating existing value itslef and finally updatedb
                    existing = new1;
                    //return (await context.SaveChangesAsync()) > 0;
                }
                else//case2 paqrticular MP details existing,now adding particular user vote counts only
                {
                    if (vote.KPIComments != null && vote.KPIComments.Count > 0)
                        existing.CommentsCount += 1;

                    if (vote.KPIRatingComments != null && vote.KPIRatingComments.Count > 0)
                        vote.KPIRatingComments.ForEach((Action<KPIRatingComment>)(k =>
                    {
                        //MP details exists but again 2 case
                        //case2.1 kpi details existing for mp
                        var kpiDetailsExisiting = existing.KPIVotes.FirstOrDefault<KPIVote>(x => x.KPI == k.KPI_Id);
                        if (kpiDetailsExisiting is null)
                        {
                            var newSummary = new KPIVote()
                            {
                                KPI = k.KPI_Id,
                                RatingTypeCountsList = [new((sbyte)k.Rating, 1)]

                            };
                            existing.KPIVotes.Add(newSummary);
                        }
                        else
                        {
                            var existingRatingType = kpiDetailsExisiting.RatingTypeCountsList.Find((Predicate<KPIVote.RatingCounts>)(x => x.Rating == k.Rating));
                            if (existingRatingType is not null)
                            {
                                existingRatingType.Count += 1;
                            }
                            else//its null so not existing RatingTypeCountsList for particular 
                            {
                                if (k.Rating is not null)
                                    kpiDetailsExisiting.RatingTypeCountsList.Add(new KPIVote.RatingCounts(k.Rating ?? (sbyte)RatingEnum.OkOk, 1));
                            }
                        }

                    }));

                    //var result = context.V_VoteSummarys.Update(existing);
                    //await context.SaveChangesAsync();
                    //instead of writing to db everytime updating existing value itslef and finally updatedb
                }
            }

            //var existingEntity = await context.V_VoteSummarys.FirstOrDefaultAsync(v => v.ConstituencyId == existing.ConstituencyId);
            if (!context.ExistsLocally<VoteSummary>(existing) && existing.Id == 0)
            {
                await context.AddEntityAsync<VoteSummary>(existing);
            }
            else
            {
                context.UpdateEntity<VoteSummary>(existing);
            }

            //if (isFirstTime)
            //{
            //    var res = await context.V_VoteSummarys.AddAsync(existing);

            //}
            //else
            //{
            //    var result = context.V_VoteSummarys.Update(existing);
            //}
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


        //TODO had to add trnasaction & rollback,since IApplciationDbcontext using so need some more ground work
        //using (var transaction = await context..Database.BeginTransactionAsync())
        //{
        //    try {
        //        await transaction.CommitAsync();
        //    }
        //    catch (Exception e)
        //    {
        //        await transaction.RollbackAsync();
        //    }
        //}
        //step1
        var lastCreatedSummaryTime = await context.VoteSummaries.AsNoTracking().MaxAsync(x => x.Created);
        var lastModifiedSummaryTime = await context.VoteSummaries.AsNoTracking().MaxAsync(x => x.LastModified);
        //todo improve make single db call to extract both created or modified data like GREATEST()
        var latestSummaryTime = (lastModifiedSummaryTime == null ? lastCreatedSummaryTime : (lastCreatedSummaryTime > lastModifiedSummaryTime ? lastCreatedSummaryTime : lastModifiedSummaryTime))
            ?? DateTime.Now.AddMinutes(-RefreshFrequncyInMinutes);

        //step2
        //mostly adding votes are more in that case we can separate for adding like this then separately for updates
        //var addedVotesToLoad = await context.V_Votes.AsNoTracking().Where(x => x.Created > timeToFilter).ToListAsync();
        //in this just adding to summary nothing to bother about delta
        //extract all newly added votes 
        //add to summary

        //if created handled above then here created condition shoudl be removed
        var deltaVotesToLoad = await context.Votes.AsNoTracking().Where(x => x.Created > latestSummaryTime || x.LastModified > latestSummaryTime || x.ConstituencyIdDelta != null || x.KPIRatingCommentsDelta != null).ToListAsync();

        if (deltaVotesToLoad == null || deltaVotesToLoad.Count == 0)
        {
            return 0;
        }
        //step3- getting difference
        var deltasToAddRemove = new List<ToAddRemove>();
        foreach (var vote in deltaVotesToLoad)
        {
            var res = GetDeltaDifference(vote, filterTime: latestSummaryTime);
            if (res != null)
                deltasToAddRemove.Add(res);
        }
        if (deltasToAddRemove.Count == 0) { DeltaLoadingInProgress = true; return 0; }
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
        var nlss = new List<KPIRatingComment>();
        if (nl != 5) nlss = null;
        var result = await context.Votes.Where(x => x.ConstituencyIdDelta != null || x.KPIRatingCommentsDelta != null)
              .ExecuteUpdateAsync(x => x
                .SetProperty(u => u.ConstituencyIdDelta, nl)
                .SetProperty(u => u.KPIRatingCommentsDelta, nlss)
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
        if ((diff.ToRemove == null || diff.ToRemove.Count == 0) && (diff.ToAdd == null || diff.ToAdd.Count == 0))
            return false;//no data to operate

        var resultCount = 0;
        if (diff == null || (diff.ConstituencyIdToAdd == 0 && diff.ConstituencyIdToRemove == 0)) return false;
        try
        {
            //2 cases 
            //case1 same constituency id for both add/remove
            //case2 different constituencyid for add remove
            //case2 scenario wont handled here,so always for this method will recieved of same constituencyid only

            diff.ConstituencyIdToAdd = diff.ConstituencyIdToAdd > 0 ? diff.ConstituencyIdToAdd : diff.ConstituencyIdToRemove;
            //above line bcz some times only removal ,no adding

            //var existing = await ReadByConstituencyId(diff.ConstituencyIdToAdd);
            var existing = await context.VoteSummaries.AsNoTracking().FirstOrDefaultAsync(x => x.ConstituencyId == diff.ConstituencyIdToAdd);
            if (existing == null)// constituencySummary data not exists,go for insert
            {//assuming this case wont appear anytime still //todo had to verify
             //since insertion nothing to bother about toRemove items
                var temp = new List<KPIVote>();
                if (diff.ToAdd is not null)
                {
                    diff.ToAdd.ForEach(k => temp.Add(new KPIVote() { KPI = k.KPI, RatingTypeCountsList = [new((sbyte)k.Rating!, 1)] }));
                    var new1 = new VoteSummary()
                    {
                        ConstituencyId = diff.ConstituencyIdToAdd,
                        KPIVotes = temp,

                        //if VoteSummary constructor does this job then this can be removed
                        //  VotesCount = temp.Sum(x => x.RatingTypeCountsList.Sum(c => c.Count)),

                        //for,against had to be fetched directly from votes table direct data only
                        //VotesCountForExistingMp= temp.Sum(x => x.RatingTypeCountsList.Sum(c => c.Count)),
                        CommentsCount = diff.CommentCountDifference > 0 ? 1 : 0
                        //,AggregateVote  had top check whether its added to db by generating or not
                    };
                    await context.AddEntityAsync(new1);
                    //entityEntry = await context.V_VoteSummarys.AddAsync(new1);
                }
            }
            else//constituencySummary data already exist,go for update
            {
                //case1 same const id to add or remove
                if (diff.ConstituencyIdToAdd == diff.ConstituencyIdToRemove || diff.ConstituencyIdToRemove == 0)
                {
                    if (diff.CommentCountDifference > 0)
                        existing.CommentsCount += diff.CommentCountDifference ?? 0;

                    ToAddToExistingSummary(diff, existing);
                    //if same constituency then same logic
                    //else different constitunecy then different

                    ToRemoveFromExistingSummary(diff, existing);

                    existing.UpdateModified();
                    //entityEntry = context.V_VoteSummarys.Update(existing);
                    context.UpdateEntity(existing);
                    //return (await context.SaveChangesAsync()) > 0;

                }
                else //different const id 
                {
                    ToAddToExistingSummary(diff, existing);
                    if (diff.ToRemove == null || diff.ToRemove.Count == 0)
                    {//nothing to remove
                        existing.UpdateModified();
                        //entityEntry = context.V_VoteSummarys.Update(existing);
                        context.UpdateEntity(existing);
                    }
                    else
                    { //had to remove in different
                        var existingToRemove = await ReadByConstituencyId(diff.ConstituencyIdToRemove);
                        if (existingToRemove != null)
                        {
                            ToRemoveFromExistingSummary(diff, existingToRemove);
                            existingToRemove.UpdateModified();
                            //var entityEntryRemoved = context.V_VoteSummarys.Update(existingToRemove);
                            context.UpdateEntity(existingToRemove);
                        }
                    }

                }
            }
            if (saveChangesToDbCall)
                resultCount += await context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            throw;
            // return false;
        }
    }

    private static void ToAddToExistingSummary(ToAddRemove diff, VoteSummary existing)
    {
        if (diff.ConstituencyIdToAdd > 0 && diff.ToAdd != null && diff.ToAdd.Count > 0)
        {
            if (diff.ToAdd != null)
            {
                foreach (var a in diff.ToAdd)
                {
                    var existingKpi = existing.KPIVotes.Find(e => e.KPI == a.KPI);
                    if (existingKpi is not null)//kpi exists
                    {//under kpi 2 cases,
                        //c1> same rating exists then increase
                        //c2> non-existing rating then add it 
                        var toAdd = existingKpi.RatingTypeCountsList.Find(k => k.Rating == a.Rating);
                        if (toAdd is not null)
                            toAdd.Count += 1;
                        else if (a.Rating != null) //same rating not exists
                        {//may be earlier it was,now can be removed but thats not here possible .for that toRemove item makes 

                            existingKpi.RatingTypeCountsList.Add(new KPIVote.RatingCounts(a.Rating ?? 0));
                        }
                        //TODO if not existing then had to add
                    }
                    else//kpi not exists,so add it
                    {
                        if (a.Rating != null)
                        {
                            existing.KPIVotes.Add(new KPIVote()
                            {
                                KPI = a.KPI,
                                RatingTypeCountsList = [new KPIVote.RatingCounts(a.Rating ?? 0, 1)]
                            });
                        }
                    }
                }
            }
            //    diff.ToAdd?.ForEach(a =>
            //{
            //    var existingKpi = existing.KPIVotes.Find(e => e.KPI == a.KPI);
            //    if (existingKpi is not null)
            //    {
            //        var toAdd = existingKpi.RatingTypeCountsList.Find(k => k.RatingTypeByte == a.Rating);
            //        if (toAdd is not null)
            //            toAdd.Count += 1;
            //        //TODO if not existing then had to add
            //    }
            //});
        }
    }

    private static void ToRemoveFromExistingSummary(ToAddRemove diff, VoteSummary existing)
    {//this is when user changed constituency to other constituency
        if (diff.ConstituencyIdToRemove > 0 && diff.ToRemove != null && diff.ToRemove.Count > 0)
        {
            diff.ToRemove?.ForEach(a =>
        {
            var existingKpi = existing.KPIVotes.Find(e => e.KPI == a.KPI);
            if (existingKpi is not null)//kpi exists
            {
                var toRemove = existingKpi.RatingTypeCountsList.Find(k => k.Rating == a.Rating);
                if (toRemove is not null)//kpi data exists,so reduce by 1
                {
                    toRemove.Count -= 1;
                    if (toRemove.Count <= 0)
                    {
                        existingKpi.RatingTypeCountsList.Remove(toRemove);
                    }
                }
                //else //no rating exists ,may be already removed
                //{//ideally this should never come

                //}
            }
            //else // kpi not exists ,ideally this should never come
        });
        }
    }

    //madhu continue here
    private static ToAddRemove GetDeltaDifference(Vote vote, DateTime filterTime)
    {//this is for the case of having delta kpi only ,not for fresh i.e FirstTime() sewparate method 

        //case1> new vote added recently
        //case2 old vote now got updated with modifications

        //if (vote.Created <= filterTime)//recently added vote
        //{ //so just adding to existing summary,nothing about removing
        //  //dont bother about delta
        //    vote.ConstituencyIdDelta = vote.ConstituencyId;
        //    vote.KPIRatingCommentsDelta = null;
        //}
        //either it shoudlbe be newly added vote or existing with delta
        //else
        if (
            (vote.Created < filterTime && vote.LastModified < filterTime)
           &&
            (vote.ConstituencyIdDelta == null || vote.KPIRatingCommentsDelta == null
            || vote.KPIRatingComments == null ||
            (vote.KPIRatingComments.Count == 0 && vote.KPIRatingCommentsDelta.Count == 0))
            )
            return null;
        var toAddRemove = new ToAddRemove();

        if (vote.ConstituencyId != vote.ConstituencyIdDelta && vote.ConstituencyIdDelta > 0)//different constituency
        {
            if (vote.KPIRatingCommentsDelta != null && vote.KPIRatingCommentsDelta.Count > 0)
            {
                toAddRemove.ConstituencyIdToRemove = vote.ConstituencyIdDelta ?? 0;
                toAddRemove.ToRemove = vote.KPIRatingCommentsDelta.Select(x => (x.KPI_Id, x.Rating)).ToList();
            }
            if (vote.KPIRatingComments != null && vote.KPIRatingComments.Count > 0)
            {
                toAddRemove.ConstituencyIdToAdd = vote.ConstituencyId;
                toAddRemove.ToAdd = vote.KPIRatingComments.Select(x => (x.KPI_Id, x.Rating)).ToList();
            }
        }
        else//below section is for same constituncy
        {
            toAddRemove.ConstituencyIdToAdd = vote.ConstituencyId;
            toAddRemove.ConstituencyIdToRemove = vote.ConstituencyId;
            //if (vote.VoteKPIRatingComments.Count == 0 && vote.KPIRatingCommentsDelta.Count == 0)
            //{//already covered at top
            //    //then no summary change just return from here;
            //    return null;
            //}
            //else 
            if ((vote.KPIRatingComments == null || vote.KPIRatingComments.Count == 0) &&
                vote.KPIRatingCommentsDelta != null && vote.KPIRatingCommentsDelta.Count > 0)
            { //means nothing present ,only old exists...so has to remove now

                //mostly this case wont come ,because now no deletion of kpi
                toAddRemove.ConstituencyIdToRemove = vote.ConstituencyIdDelta ?? 0;
                toAddRemove.ToRemove = vote.KPIRatingCommentsDelta.Select(x => (x.KPI_Id, x.Rating)).ToList();
                //nothing to add,just removing 
            }
            else if (vote.KPIRatingComments != null && vote.KPIRatingComments.Count > 0 &&
                (vote.KPIRatingCommentsDelta == null || vote.KPIRatingCommentsDelta.Count == 0))
            {//means nothing in old ,so now add all to summary 
                //this case is for recently added vote
                toAddRemove.ConstituencyIdToAdd = vote.ConstituencyId;
                toAddRemove.ToAdd = vote.KPIRatingComments.Select(x => (x.KPI_Id, x.Rating)).ToList();
                //nothing to remove or nothing on even different constituency id nothing to bother
            }
            else //means old has some data & present also has some data
            {//had to do difference... mostly this is common scenario

                var oldDeltaKpis = vote.KPIRatingCommentsDelta.Select(x => (x.KPI_Id, x.Rating)).ToList();
                var presentKpis = vote.KPIRatingComments.Select(x => (x.KPI_Id, x.Rating)).ToList();
                if (oldDeltaKpis == presentKpis) return null;

                toAddRemove.ToRemove = oldDeltaKpis.Except(presentKpis).ToList();
                toAddRemove.ToAdd = presentKpis.Except(oldDeltaKpis).ToList();
                /*
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
                */
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
    private async Task<bool> AddForNewVote(V_Vote vote, bool saveChangesToDbCall = false)
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