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

//this structure is for storing in db purpose,not for viewing
public class VoteKPIRatingComment
{
    public VoteKPIRatingComment()
    {
        Created = DateTime.Now;
        //below is not making any use bcz here KPI is 0 only always
        if (KPI > 0 && V_KPI is null)
            V_KPI = V_KPI.Get(KPI);
    }
    public VoteKPIRatingComment(int kpiId) : this()
    {  ////this is for d sake of UI auto generating list purpose
        KPI = kpiId;
        V_KPI = V_KPI.Get(kpiId);
    }
    public VoteKPIRatingComment(V_KPI v_KPI) : this()
    {  ////this is for d sake of UI auto generating list purpose
        KPI = v_KPI.Id;
        V_KPI = v_KPI;
    }
    public VoteKPIRatingComment(int kpiId, sbyte rating) : this(kpiId)
    {
        Rating = rating;
    }
    //if no value(null) at any place then dont store
    public int KPI { get; set; }//KPIEnum this can be enum but due to expansion its not

    [JsonIgnore]
    public V_KPI? V_KPI { get; set; }//this is only for UI purpose nothing else
    public sbyte? Rating { get; set; } //-2-3

    [JsonIgnore]
    public string? Comment { get; set; }//moved to other ,only used for carrying from frontend screen

    public DateTime Created { get; protected set; }
    public DateTime? Modified { get; set; }

    public void UpdateModified()
    {
        Modified = DateTime.UtcNow;
    }
}





//currently not used,will do later
public class VoteKPIComment(int kpi, string comment)
{
    public int KPI { get; set; } = kpi;
    public string Comment { get; set; } = comment;//this supposed to be like List<KPI-comment>
}

public class VoteSummary_KPIVote
{

    public class RatingTypeCounts
    {
        //old style, since primary constructors not supporting deserialization without default parameterless constructor
        public RatingTypeCounts()
        {

        }
        public RatingTypeCounts(sbyte ratingTypeByte, int voteCount)
        {
            RatingTypeByte = ratingTypeByte;
            Count = voteCount;
        }
        public sbyte RatingTypeByte { get; set; }
        public int Count { get; set; }
    }
    public int KPI { get; set; }

    public List<RatingTypeCounts> RatingTypeCountsList { get; set; }//(0=>1000,1=>24,2=>4,4=>15,5=>100)

    //this is for particular KPI average
    public float AggregateKPIVote => CalculateKPIAggregateVote();

    private float CalculateKPIAggregateVote()
    {
        if (RatingTypeCountsList == null || RatingTypeCountsList.Count == 0)
        {
            return 0; // or any default value
        }

        var totalVotes = 0;
        var totalWeightedVotes = 0;

        foreach (var item in RatingTypeCountsList)
        {
            totalVotes += item.Count;
            // totalWeightedVotes += ratingType * votes;
            totalWeightedVotes += GetEachVoteValue(item.RatingTypeByte) * item.Count;
        }

        static int GetEachVoteValue(sbyte ratingType)
        {
            //here mostly always 1 is value but changes in summary level
            // Your logic to map ratingType to actual vote value
            // Example: switch (ratingType) { case 0: return 100; case 1: return 50; ... }

            return 1; // Default value, update based on your logic
        }
        return (float)totalWeightedVotes / totalVotes;
    }

    //only top5 common comments... need to find some logic
    ////public List<VoteKPIComments> Comments { get; set; }//this will be huge so had to think of something else like selected only adding
}
//public class UserComments //1user 1 row
//{
//    [Key]
//    public int Id { get; set; }

//    [Required]
//    public string UserId { get; set; }

//    [ForeignKey(nameof(UserId))]
//    public ApplicationUser User { get; set; }

//    [Required]
//    public int MPId { get; set; }

//    [ForeignKey(nameof(MPId))]
//    public V_Constituency V_Constituency { get; set; }

//    public string GoodComment { get; set; }
//    public string BadComment { get; set; }
//    public string Complaints { get; set; }

//    [Required]
//    public DateTime Timestamp { get; set; }

//    // Add any other properties or validation as needed
//}

/*
 * //public enum VotingKeyPoints
//{
//    VeryBad = -1, //this is wrong
//    Bad = 0,
//    GoodPersonButNotDoneAnyThing = 1,
//    GoodPersonDoingNormalWork = 2,

//    VeryGoodDoneGreatWork = 5
//}

 V_Vote
- Id (Primary Key)
- UserId (Foreign Key referencing User.Id)
- MPId (Foreign Key referencing V_Constituency.Id)
- VotesJsonAsString (JSON)
- Timestamp

VotesJsonAsString Json structure
{
  "VotingKeyPoints": [
    {"KPI": 1, "VoteValue": 4, "Comment": "Good job!"},
    {"KPI": 2, "VoteValue": 3, "Comment": "Neutral"}
    // ... other points
  ]
}
by
-- Retrieve individual votes and comments for a user
SELECT UserId, MPId, JSON_VALUE(VotesJsonAsString, '$.VotingKeyPoints[0].VoteValue') AS VoteValue
FROM V_Vote
WHERE UserId = @UserId AND MPId = @MPId


V_VoteSummary
- Id (Primary Key)
- MPId (Foreign Key referencing V_Constituency.Id)
- VotesSummary (JSON)
- CommentCountForMpId (Int)

V_VoteSummary Json structure
{
  "VotingKeyPoints": [
    {
      "KPI": 1,
      "AggregateKPIVote": 4.2,
      "RatingTypeCountsList": {
        "1": 10,
        "2": 15,
        "3": 30,
        "4": 45,
        "5": 20
      }
    },
    {
      "KPI": 2,
      "AggregateKPIVote": 3.8,
      "RatingTypeCountsList": {
        "1": 5,
        "2": 20,
        "3": 25,
        "4": 50,
        "5": 15
      }
    },
    // ... other points
  ]
}

by
-- Retrieve average vote and counts for a specific voting point in a location
SELECT JSON_VALUE(VotesSummary, '$.VotingKeyPoints[0].AggregateKPIVote') AS AggregateKPIVote,
       JSON_VALUE(VotesSummary, '$.VotingKeyPoints[0].RatingTypeCountsList."1"') AS VoteCount1,
       JSON_VALUE(VotesSummary, '$.VotingKeyPoints[0].RatingTypeCountsList."2"') AS VoteCount2,
       -- ... other counts
FROM V_VoteSummary
WHERE MPId = @MPId

trigger to update summary table
CREATE TRIGGER UpdateLocationSummary
ON V_Vote
AFTER INSERT
AS
BEGIN
    -- Get the MPId and KPI for the newly inserted vote
    DECLARE @MPId INT, @KPI INT;
    SELECT @MPId = MPId, @KPI = KPI FROM inserted;

    -- Update or insert into V_VoteSummary
    MERGE INTO V_VoteSummary AS target
    USING (
        SELECT
            MPId,
            KPI,
            AVG(CAST(VoteValue AS DECIMAL(10, 2))) AS AggregateKPIVote,
            COUNT(*) AS Count
        FROM
            V_Vote
        WHERE
            MPId = @MPId
            AND KPI = @KPI
        GROUP BY
            MPId,
            KPI
    ) AS source
    ON
        target.MPId = source.MPId
        AND target.KPI = source.KPI
    WHEN MATCHED THEN
        UPDATE SET
            target.AggregateKPIVote = source.AggregateKPIVote,
            target.Count = source.Count
    WHEN NOT MATCHED THEN
        INSERT (MPId, KPI, AggregateKPIVote, Count)
        VALUES (source.MPId, source.KPI, source.AggregateKPIVote, source.Count);
END;

 */

//public class VotingKeyPoints //this are 10 kewycan be enum and use as byte
//since its count is minimal & keep using so better to maintian in code itself rather in db
//{
//    [Key]
//    public int Id { get; set; }

//    [Required]
//    public string PointName { get; set; }

//    [Required]
//    public string Description { get; set; }

//    // Optional: add a Weight property if points have different importance
//}

