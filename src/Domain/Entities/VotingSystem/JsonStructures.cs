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
public class KPIRatingComment
{
    public KPIRatingComment()
    {
        Created = DateTime.Now;
        //below is not making any use bcz here KPI is 0 only always
        if (KPI > 0 && V_KPI is null)
            V_KPI = V_KPI.Get(KPI);
    }
    public KPIRatingComment(int kpiId) : this()
    {  ////this is for d sake of UI auto generating list purpose
        KPI = kpiId;
        V_KPI = V_KPI.Get(kpiId);
    }
    public KPIRatingComment(V_KPI v_KPI) : this()
    {  ////this is for d sake of UI auto generating list purpose
        KPI = v_KPI.Id;
        V_KPI = v_KPI;
    }
    public KPIRatingComment(int kpiId, sbyte rating) : this(kpiId)
    {
        Rating = rating;
    }
    //if no value(null) at any place then dont store
    public int KPI { get; set; }//KPIEnum this can be enum but due to expansion its not

    [JsonIgnore]
    public V_KPI? V_KPI { get; set; }//this is only for UI purpose nothing else
    public sbyte? Rating { get; set; } //-2-3

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Comment { get; set; }//moved to other ,only used for carrying from frontend screen

    public DateTime Created { get; protected set; }
    public DateTime? LastModified { get; set; }

    public void UpdateModified()
    {
        LastModified = DateTime.UtcNow;
    }
}





//currently not used,will do later
public class KPIComment(int kpi, string comment)
{
    public KPIComment() : this(default, default)
    {

    }
    public int KPI { get; set; } = kpi;
    public string Comment { get; set; } = comment;//this supposed to be like List<KPI-comment>
}

public class Summary_KPIVote
{

    public class RatingTypeCounts
    {
        //old style, since primary constructors not supporting deserialization without default parameterless constructor
        public RatingTypeCounts()
        {

        }
        public RatingTypeCounts(sbyte ratingTypeByte, int voteCount = 1)
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
    public sbyte AggregateRatingOfKPI => CalculateAggregateRatingOfKPI();

    private sbyte CalculateAggregateRatingOfKPI()
    {
        if (RatingTypeCountsList == null || RatingTypeCountsList.Count == 0)
        {
            return 0; // or any default value //todo need to think what could be, but mostly this wont come any t
        }

        //todo can make different value for each kpi type later case
        var kpiValue = 1;//if kpi 1 value is more then more it value like 3 ,if less then make it like 0.5
        var totalVotes = RatingTypeCountsList.Sum(r => r.Count);
        var weightSum = RatingTypeCountsList.Sum(r => r.RatingTypeByte * r.Count) * kpiValue;
        var aggregateKPIVote = totalVotes != 0 ? (sbyte)Math.Min(3, Math.Max(-2, weightSum / totalVotes)) : (sbyte)0;
        return aggregateKPIVote;
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

