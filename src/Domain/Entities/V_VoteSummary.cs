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
    public float AggregateVote => CalculateAggregateVote();//if this created db column then below is not required
    //public float AggregateVote { get { return CalculateAggregateVote(); } }

    private float CalculateAggregateVote()
    {
        if (KPIVotes == null || KPIVotes.Count == 0)
        {
            return 0; // or any default value
        }

        float totalWeight = 0;

        foreach (var item in KPIVotes)
        {
            totalWeight += GetParticularEachKpiValue(item.KPI) * item.AggregateKPIVote;
        }

        static int GetParticularEachKpiValue(int kpi)
        {
            //here mostly value changes for each type of KPI ,but inside CalculateKPIAggregateVote() mostly always 1
            // Your logic to map ratingType to actual vote value
            // Example: switch (ratingType) { case 0: return 100; case 1: return 50; ... }

            return 1; // Default value, update based on your logic
        }
        return (float)totalWeight / KPIVotes.Count;
    }
    public class ToAddRemove
    {
        public int ConstituencyId { get; set; }
        public int? CommentCountDifference { get; set; }
        public List<(int KPI, sbyte? Rating)>? ToAdd { get; set; }
        public List<(int KPI, sbyte? Rating)>? ToRemove { get; set; }
    }
}
