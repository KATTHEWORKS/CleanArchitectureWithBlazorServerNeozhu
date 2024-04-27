using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using PublicCommon;
using PublicCommon.MyVote;

namespace Domain.MyVote;
//https://sansad.in/

//dbtable3
public class VoteSummary : BaseAuditableEntity //each location one row as summary
{//db structure
    public VoteSummary()
    {
        Created = DateTime.Now;
        //   CalculateAggregateRatingOfConstituency();

    }
    //[Key]
    ////[DatabaseGenerated(DatabaseGeneratedOption.Computed)]//this gives error so using fluentnt API .ValueGeneratedOnAdd();
    //public int Id { get; set; }//dont remove id,even its not directly needed.avoids tracking issues
    //[Key]
    public int ConstituencyId { get; set; }

    [ForeignKey(nameof(ConstituencyId))]
    public VoteConstituency Constituency { get; set; }

    [Required]
    public int MessagesCount { get; set; }

    [Required]
    public int VotesCount { get; set; }
    public sbyte? Rating { get; set; } //this can be null when added user removed vote
    public int? WishToReElectMemberTrueCount { get; set; } //from vote table  count(WishToReElectMember==true)

    //VotesCountAgainstCurrentMember
    public int? WishToReElectMemberFalseCount { get; set; }//from vote table  count(WishToReElectMember==false)



    public void UpdateModified()
    {
        //VotesCount = KPIVotes.Sum(x => x.RatingTypeCountsList.Sum(c => c.Count));
        Updated = DateTime.Now;
    }

    public void ResetSummary()
    {
        Rating = default;
        MessagesCount = 0;
        VotesCount = default;
        MessagesCount = default;
        WishToReElectMemberTrueCount = default;
        WishToReElectMemberFalseCount = default;
    }
    //below propertiues will be used in Dtos,so here messageing
    //[Column(TypeName = "jsonb")] //this wont working mssql Adjust based on your database
    //public string KPIVotesAsJsonString { get; set; }
    //[NotMapped]
    //public List<KPIVote> KPIVotes { get; set; }
    private List<KpiRatingCounts> _kpiRatingCounts;
    public List<KpiRatingCounts>? KpiRatingCounts
    {
        get => _kpiRatingCounts;

        set
        {
            _kpiRatingCounts = value?.Where(x => x.RatingCountsList.Count > 0).ToList() ?? [];
            if (_kpiRatingCounts.HasData())
            {
                Rating = _kpiRatingCounts.CalculateRating();
                return;
            }

            ResetSummary();//this executes only if zero result in above if condition
        }
    }
    //{
    //    //get => JsonSerializer.Deserialize<KPIVotes>(KPIVotesAsJsonString);
    //    get => JsonExtensions.TryDeserialize<List<VoteSummary_KPIVote>>(KPIVotesAsJsonString, out var result) ? result : [];
    //    set
    //    {
    //        //KPIVotesAsJsonString = JsonSerializer.Serialize(value);
    //        KPIVotesAsJsonString = JsonSerializer.Serialize(value, JsonExtensions.IgnoreNullSerializationOptions);
    //        VotesCount = value.Sum(x => x.RatingTypeCountsList.Sum(c => c.Count));
    //        //MessageCountForMemberId //this cant be added here,instead at services
    //    }
    //}


    //private void CalculateAggregateRatingOfConstituency()
    //{
    //    if (KPIVotes == null || KPIVotes.Count == 0) return; // or any default value //todo need to think what could be, but mostly this wont come any time
    //  //  VotesCount = KPIVotes.Sum(x => x.RatingTypeCountsList.Sum(c => c.Count));
    //    var sumOfAggregateKPIs = KPIVotes.Sum(x => x.AggregateRatingOfKPI);
    //    var totalKpis = KPIVotes.Count;
    //    Rating = totalKpis != 0 ? (sbyte)Math.Min(3, Math.Max(-2, sumOfAggregateKPIs / totalKpis)) : (sbyte)0;
    //}
}
public class ToAddRemove
{

    public int? MessageCountDifference { get; set; }
    public List<(int KPI, int? Rating)>? ToAdd { get; set; }
    public int ConstituencyIdToAdd { get; set; }
    public List<(int KPI, int? Rating)>? ToRemove { get; set; }
    public int ConstituencyIdToRemove { get; set; }
}
