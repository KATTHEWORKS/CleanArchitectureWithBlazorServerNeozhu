using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Domain.Identity;
using PublicCommon;

namespace CleanArchitecture.Blazor.Domain.Entities;
//https://sansad.in/

//dbtable3
public class V_VoteSummary//each location one row as summary
{
    public V_VoteSummary()
    {
        Created = DateTime.Now;
    }

    //[Key]
    public int ConstituencyId { get; set; }

    [ForeignKey(nameof(ConstituencyId))]
    public V_Constituency Constituency { get; set; }
    [Required]
    public int CommentsCount { get; set; }

    [Required]
    public int VotesCount { get { return KPIVotes.Sum(x => x.RatingTypeCountsList.Sum(c => c.Count)); } }



    [Required]
    public DateTime Created { get; protected set; }
    public DateTime? Modified { get; set; }


    public void UpdateModified()
    {
        Modified = DateTime.UtcNow;
    }
    //below propertiues will be used in Dtos,so here commenting
    //[Column(TypeName = "jsonb")] //this wont working mssql Adjust based on your database
    //public string KPIVotesAsJsonString { get; set; }
    //[NotMapped]
    public List<VoteSummary_KPIVote> KPIVotes { get; set; }
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
    //this is for total aggregate of all KPI values
    public sbyte AggregateRatingOfConstituency => CalculateAggregateRatingOfConstituency();//if this created db column then below is not required
    ////public float AggregateVote { get { return CalculateAggregateVote(); } }

    private sbyte CalculateAggregateRatingOfConstituency()
    {
        if (KPIVotes == null || KPIVotes.Count == 0) return 0; // or any default value //todo need to think what could be, but mostly this wont come any time
        var sumOfAggregateKPIs = KPIVotes.Sum(x => x.AggregateRatingOfKPI);
        var totalCount = KPIVotes.Count;
        return totalCount != 0 ? (sbyte)Math.Min(3, Math.Max(-2, sumOfAggregateKPIs / totalCount)) : (sbyte)0;
    }
    public class ToAddRemove
    {

        public int? CommentCountDifference { get; set; }
        public List<(int KPI, sbyte? Rating)>? ToAdd { get; set; }
        public int ConstituencyIdToAdd { get; set; }
        public List<(int KPI, sbyte? Rating)>? ToRemove { get; set; }
        public int ConstituencyIdToRemove { get; set; }
    }
}
