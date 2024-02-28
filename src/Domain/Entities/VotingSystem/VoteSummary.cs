using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Domain.Common.Entities;
using CleanArchitecture.Blazor.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using PublicCommon;

namespace CleanArchitecture.Blazor.Domain.Entities.VotingSystem;
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
    public int CommentsCount { get; set; } = 0;

    [Required]
    public int VotesCount { get; set; } = 0;
    public sbyte? Rating { get; set; } = null;//this can be null when added user removed vote
    public int? VotesCountForExistingMp { get; set; } = 0;//from vote table  count(WishToReElectMp==true)
    public int? VotesCountAgainstExistingMp { get; set; } = 0;//from vote table  count(WishToReElectMp==false)



    public void UpdateModified()
    {
        //VotesCount = KPIVotes.Sum(x => x.RatingTypeCountsList.Sum(c => c.Count));
        LastModified = DateTime.Now;
    }
    //below propertiues will be used in Dtos,so here commenting
    //[Column(TypeName = "jsonb")] //this wont working mssql Adjust based on your database
    //public string KPIVotesAsJsonString { get; set; }
    //[NotMapped]
    //public List<KPIVote> KPIVotes { get; set; }
    private List<KPIVote> _kpiVotes;
    public List<KPIVote> KPIVotes
    {
        get => _kpiVotes;

        set
        {
            _kpiVotes = value;
            if (value != null && value.Count > 0)
            {
                VotesCount = value.Sum(x => x.RatingTypeCountsList.Sum(c => c.Count));
                var sumOfAggregateKPIs = value.Sum(x => x.AggregateRatingOfKPI);
                var totalKpis = value.Count;
                Rating = totalKpis != 0 ? (sbyte)Math.Min(3, Math.Max(-2, sumOfAggregateKPIs / totalKpis)) : (sbyte)0;
            }
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
    //        //CommentCountForMpId //this cant be added here,instead at services
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

    public int? CommentCountDifference { get; set; }
    public List<(int KPI, sbyte? Rating)>? ToAdd { get; set; }
    public int ConstituencyIdToAdd { get; set; }
    public List<(int KPI, sbyte? Rating)>? ToRemove { get; set; }
    public int ConstituencyIdToRemove { get; set; }
}
