using static PublicCommon.MyVote.KpiRatingCounts;

namespace PublicCommon.MyVote;

public static class VoteHelperExtensions
{

    public static void LoadMessages(this List<KPIRatingMessage>? kPIRatingMessages, List<KPIMessage>? messages)
    {
        if (kPIRatingMessages.IsEmpty() || messages.IsEmpty())
            return;
        else
            foreach (var item in messages!)
            {
                if (kPIRatingMessages!.Exists(x => x.KPI_Id == item.KPI))
                    kPIRatingMessages!.Find(x => x.KPI_Id == item.KPI).Message = item.Message;
            }
    }

    public static string RatingAsString(this int? rating)
    {
        if (rating == null) return "No Rating";
        //var str = string.Empty;
        ////5/5 will not be displayed
        //if (rating != (int)RatingEnum.GreatWork)
        //    str = $"[{rating}/{(int)RatingEnum.GreatWork}]";
        //return EnumExtensions.ParseToEnum<RatingEnum>(rating ?? 1)?.ToString() + str ?? "No Rating";
        return (rating ?? 1).ParseToEnum<RatingEnum>()?.ToString() ?? "No Rating";
    }
    public static string RatingAsString(this sbyte? rating) => ((int?)rating).RatingAsString();
    public static string RatingAsString(this sbyte rating) => RatingAsString((int)rating);

    public static List<KPIRatingMessage>? KPIRatingMessageNullify(this List<KPIRatingMessage>? me)
    {
        if (me.HasData())
        {
            me!.RemoveAll(x => x.KPI_Id != KPI.OpenIssuesKpiId && (x.Rating == 0 || x.Rating == null));
            me.ForEach(x => x.Message = null);
        }
        return me;
    }
    public static List<KpiRatingCounts>? ValidRatings(this IEnumerable<KpiRatingCounts>? kpis)
    {
        return kpis?.Where(x => x.RatingCountsList.Count > 0).ToList();
    }

    public static List<KPIMessage>? ValidRatingsNullifyMessageAndExtractMessagesToPostApi(this IEnumerable<KPIRatingMessage>? kpiRatingMessages)
    {
        if (kpiRatingMessages.IsEmpty()) return null;

        var kpiMessages = kpiRatingMessages.Where(x => x.Message.HasData() && (x.Rating > 0 || x.KPI_Id == KPI.OpenIssuesKpiId))
            .Select(x => new KPIMessage(x.KPI_Id, x.Message));

        kpiRatingMessages = kpiRatingMessages?.Where(x => x.Rating > 0 || x.KPI_Id == KPI.OpenIssuesKpiId && x.Message.HasData())
            .Select(x => new KPIRatingMessage(x.KPI_Id, x.Rating ?? 0));
        var cmt = kpiRatingMessages!.FirstOrDefault(x => x.KPI_Id == KPI.OpenIssuesKpiId && x.Rating > 0);
        if (cmt != null || cmt != default)
            cmt.Rating = 0;
        return kpiMessages.ToList();
    }

    public static List<KPIRatingMessage>? ValidRatings(this IEnumerable<KPIRatingMessage>? kpiRatingMessages)
    {
        if (kpiRatingMessages.IsEmpty()) return null;

        kpiRatingMessages = kpiRatingMessages?.Where(x => x.Rating > 0 || x.KPI_Id == KPI.OpenIssuesKpiId && x.Message.HasData()).ToList();
        var cmt = kpiRatingMessages!.FirstOrDefault(x => x.KPI_Id == KPI.OpenIssuesKpiId && x.Rating > 0);
        if (cmt != null || cmt != default)
            cmt.Rating = 0;
        return kpiRatingMessages!.ToList();
    }
    public static sbyte CalculateRating(this List<KpiRatingCounts>? kpiRatingCounts)
    {
        kpiRatingCounts = kpiRatingCounts.ValidRatings();
        if (kpiRatingCounts.HasData())
        {
            return ((double)kpiRatingCounts.Sum(x => x.Aggregate) / kpiRatingCounts.Count).GetUpperLimitRating();
        }
        else
        {
            return default; // or any default value if no ratings available
        }
    }

    public static sbyte CalculateRatingSumByVotes(this List<RatingCounts> ratingCounts)
    {
        if (ratingCounts == null || ratingCounts.Count == 0)
        {
            return 0; // or any default value //todo need to think what could be, but mostly this wont come any t
        }

        //todo can make different value for each kpi type later case
        var kpiValue = 1;//if kpi 1 value is more then more it value like 3 ,if less then make it like 0.5
        var totalVotes = ratingCounts.Sum(r => r.Count);
        var weightSum = ratingCounts.Sum(r => r.Rating * r.Count) * kpiValue;

        // Calculate the average rating (rounded up)
        return GetUpperLimitRating((float)weightSum / totalVotes);
        //sbyte roundedAverage = (sbyte)Math.Ceiling(averageRating);

        //var aggregateKPIVote = totalVotes != 0 ? Math.Min((sbyte)3, Math.Max((sbyte)-2, roundedAverage)) : (sbyte)0;
        //return aggregateKPIVote;
    }


    public static sbyte CalculateRating(this IEnumerable<KPIRatingMessage>? kpiRatingMessages)
    {
        kpiRatingMessages = kpiRatingMessages.ValidRatings();
        if (kpiRatingMessages.IsEmpty()) return default;

        var sum = 0;
        foreach (var kpiRatingMessage in kpiRatingMessages!.Where(x => x.Rating > 0 && x.KPI_Id != KPI.OpenIssuesKpiId))
        {
            if (kpiRatingMessage.Rating.HasValue)
            {
                sum += kpiRatingMessage.Rating.Value;
            }
        }
        //return (sbyte)(sum / kpiRatingMessages.Count(x => x.Rating != null));
        // Calculate average rating

        var lower1 = kpiRatingMessages.Count();

        var value1 = (double)sum / lower1;
        return value1.GetUpperLimitRating();
        //return GetUpperLimitRating(sum / kpiRatingMessages.Count(x => x.Rating != null));

    }
    public static Dictionary<int, (double totalRating, int totalCount)> SummaryAggregateRating(this List<KpiRatingCounts> kpiRatingCounts)
    {
        if (kpiRatingCounts.IsEmpty()) return null;
        var aggregateResults = new Dictionary<int, (double totalRating, int totalCount)>();

        foreach (var kpiRating in kpiRatingCounts)
        {
            if (!aggregateResults.ContainsKey(kpiRating.KPI))
            {
                aggregateResults[kpiRating.KPI] = (0, 0);
            }

            foreach (var ratingCount in kpiRating.RatingCountsList)
            {
                aggregateResults[kpiRating.KPI] = (
                    aggregateResults[kpiRating.KPI].totalRating + ratingCount.Rating * ratingCount.Count,
                    aggregateResults[kpiRating.KPI].totalCount + ratingCount.Count
                );
            }
        }

        foreach (var kpi in aggregateResults.Keys.ToList())
        {
            aggregateResults[kpi] = (
                aggregateResults[kpi].totalRating / aggregateResults[kpi].totalCount,
                aggregateResults[kpi].totalCount
            );
        }

        return aggregateResults;
    }
    public static sbyte GetUpperLimitRating(this double averageRating)
    {
        // Ceiling for upper limit
        return (sbyte)Math.Ceiling(averageRating);

        //return upperLimit switch
        //{
        //    >= -2 and < 1 => upperLimit,  // Handle values between -2 and 0 (inclusive)
        //    5 => 2,
        //    _ => (sbyte)(upperLimit - 3),          // Map values 1-3 to -2, -1, 0, and 1 using a single expression
        //};
        //if (upperLimit < 1 && upperLimit >= -2) return upperLimit;
        //else if (upperLimit > 5) return 2;
        //else if (upperLimit > 1 && upperLimit <= 5)
        //{
        //    switch (upperLimit)
        //    {
        //        case 1: return -2;
        //        case 2: return -1;
        //        case 3: return 0;
        //        case 4: return 1;
        //        case 5: return 2;
        //        default: return 0;
        //    }
        //}
        //else return 0;
    }

    ///// <summary>
    ///// means input is 1 to 5
    ///// </summary>
    ///// <param name="rating"></param>
    ///// <returns></returns>
    //public static int GetRatingActualValue(int rating)
    //{
    //    return (int)rating - 3; // Add 3 to all enum values to shift the negative range to positive (1-5)
    //}

    ////public static List<KPIRatingMessage>? RatingAdjustToUiPositiveRange(List<KPIRatingMessage>? kpiRatingMessages)
    ////{
    ////    if (kpiRatingMessages != null && kpiRatingMessages.Count > 0)
    ////    {
    ////        kpiRatingMessages = kpiRatingMessages.Where(x => x.Rating != null && x.Rating <= (int)RatingEnum.GreatWork && x.Rating >= (int)RatingEnum.VeryBad).ToList();
    ////        kpiRatingMessages.ForEach(x => x.Rating += 3);
    ////        kpiRatingMessages = [.. kpiRatingMessages.OrderByDescending(x => x.Rating)];
    ////    }
    ////    else kpiRatingMessages = null;
    ////    return KPI.MergeWithExistingVoteKPIRatingMessageList(kpiRatingMessages);
    ////}
    //public static List<KPIRatingMessage>? RatingSetBackToOriginalRange(List<KPIRatingMessage>? kpiRatingMessages)
    //{
    //    if (kpiRatingMessages != null && kpiRatingMessages.Count > 0)
    //    {
    //        kpiRatingMessages = kpiRatingMessages.Where(x => x.Rating != null && x.Rating <= (int)RatingEnum.GreatWork + 3 && x.Rating >= (int)RatingEnum.VeryBad + 3).ToList();
    //        kpiRatingMessages.ForEach(x => x.Rating -= 3);
    //    }
    //    else kpiRatingMessages = null;
    //    return kpiRatingMessages;
    //}
}
