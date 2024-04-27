using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PublicCommon.MyVote;
//https://sansad.in/

//this structure is for storing in db purpose,not for viewing
public class KPIRatingMessage
{
    public KPIRatingMessage()
    {
        Created = DateTime.Now;
        //below is not making any use bcz here KPI is 0 only always
        if (KPI_Id > 0 && KPI is null)
            KPI = KPI.Get(KPI_Id);
    }
    public KPIRatingMessage(int kpiId) : this()
    {  ////this is for d sake of UI auto generating list purpose
        KPI_Id = kpiId;
        KPI = KPI.Get(kpiId);
    }
    public KPIRatingMessage(KPI kpi) : this()
    {  ////this is for d sake of UI auto generating list purpose
        KPI_Id = kpi.Id;
        KPI = kpi;
    }
    public KPIRatingMessage(int kpiId, int rating) : this(kpiId)
    {
        Rating = rating;
    }
    public KPIRatingMessage(int kpiId, string message) : this(kpiId)
    {
        Message = message;
    }
    public KPIRatingMessage(int kpiId, int rating, string message) : this(kpiId, rating)
    {
        Message = message;
    }
    public static KPIRatingMessage KPIRatingMessageCombine(KPIRatingMessage me, string message)
    {
        me.Message = message;
        return me;
    }

    public static List<KPIRatingMessage>? KPIRatingMessageCombine(List<KPIRatingMessage>? me, List<KPIMessage>? messages)
    {
        if (me.IsEmpty() && messages.IsEmpty()) return null;

        if (messages.IsEmpty()) return me;
        if (me.IsEmpty()) return messages!.Select(c => new KPIRatingMessage(c.KPI, c.Message)).ToList();

        var differences = messages!.Select(x => x.KPI).Except(me!.Select(y => y.KPI_Id));
        if (differences.HasData())
        {
            if (differences!.Count(x => x != KPI.OpenIssuesKpiId) > 0)
                messages!.RemoveAll(c => differences.Where(x => x != KPI.OpenIssuesKpiId).Contains(c.KPI));
            if (differences.Any(x => x == KPI.OpenIssuesKpiId))
                me.Add(new KPIRatingMessage(KPI.OpenIssuesKpiId));
        }
        //me.ForEach(r => r.Message = messages.Find(c => c.KPI == r.KPI_Id)?.Message);

        messages!.ForEach(c => me!.Find(r => r.KPI_Id == c.KPI)!.Message = c.Message);//this can lead to errors so previous check
        if (messages.Any(x => x.KPI == KPI.OpenIssuesKpiId) && !me!.Any(x => x.KPI_Id == KPI.OpenIssuesKpiId))
        {
            me!.Add(new KPIRatingMessage(KPI.OpenIssuesKpiId, messages.Find(x => x.KPI == KPI.OpenIssuesKpiId)!.Message));
        }
        return me;
    }
    //if no value(null) at any place then dont store
    public int KPI_Id { get; set; }//KPIEnum this can be enum but due to expansion its not

    [JsonIgnore]
    public KPI? KPI { get; set; }//this is only for UI purpose nothing else
    public int? Rating { get; set; } //-2 to +2

    //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]//its not working
    [MaxLength(50)]
    public string? Message { get; set; }//moved to other ,only used for carrying from frontend screen

    [JsonIgnore]
    public bool IsMessageExpanded { get; set; } // Initially set to false by default for UI purpose only
    public DateTime Created { get; protected set; }
    public DateTime? LastModified { get; set; }

    public void UpdateModified()
    {
        LastModified = DateTime.Now;
    }
}





//currently not used,will do later
public class KPIMessage(int kpi, string message)
{
    public KPIMessage() : this(default, default)
    {

    }
    public int KPI { get; set; } = kpi;
    public string Message { get; set; } = message;//this supposed to be like List<KPI-Message>
}

public class KpiRatingCounts
{
    public KpiRatingCounts()
    {

    }
    public KpiRatingCounts(int kpiId)
    {
        KPI = kpiId;
    }
    public class RatingCounts
    {
        //old style, since primary constructors not supporting deserialization without default parameterless constructor
        public RatingCounts()
        {

        }
        public RatingCounts(int rating, int voteCount = 1)
        {
            Rating = rating;
            Count = voteCount;
        }
        public int Rating { get; set; }
        public int Count { get; set; }
    }
    public int KPI { get; set; }

    public List<RatingCounts> RatingCountsList { get; set; } = []; //(0=>1000,1=>24,2=>4,4=>15,5=>100)

    //this is for particular KPI average
    public int Aggregate => RatingCountsList.CalculateRatingSumByVotes();// CalculateAggregateRatingOfKPI();

    //only top5 common Messages... need to find some logic
    ////public List<VoteKPIMessages> Messages { get; set; }//this will be huge so had to think of something else like selected only adding
}
//public class UserMessages //1user 1 row
//{
//    [Key]
//    public int Id { get; set; }

//    [Required]
//    public string UserId { get; set; }

//    [ForeignKey(nameof(UserId))]
//    public ApplicationUser User { get; set; }

//    [Required]
//    public int MemberId { get; set; }

//    [ForeignKey(nameof(MemberId))]
//    public V_Constituency V_Constituency { get; set; }

//    public string GoodMessage { get; set; }
//    public string BadMessage { get; set; }
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
- MemberId (Foreign Key referencing V_Constituency.Id)
- VotesJsonAsString (JSON)
- Timestamp

VotesJsonAsString Json structure
{
  "VotingKeyPoints": [
    {"KPI": 1, "VoteValue": 4, "Message": "Good job!"},
    {"KPI": 2, "VoteValue": 3, "Message": "Neutral"}
    // ... other points
  ]
}
by
-- Retrieve individual votes and messages for a user
SELECT UserId, MemberId, JSON_VALUE(VotesJsonAsString, '$.VotingKeyPoints[0].VoteValue') AS VoteValue
FROM V_Vote
WHERE UserId = @UserId AND MemberId = @MemberId


V_VoteSummary
- Id (Primary Key)
- MemberId (Foreign Key referencing V_Constituency.Id)
- VotesSummary (JSON)
- MessageCountForMemberId (Int)

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
WHERE MemberId = @MemberId

trigger to update summary table
CREATE TRIGGER UpdateLocationSummary
ON V_Vote
AFTER INSERT
AS
BEGIN
    -- Get the MemberId and KPI for the newly inserted vote
    DECLARE @MemberId INT, @KPI INT;
    SELECT @MemberId = MemberId, @KPI = KPI FROM inserted;

    -- Update or insert into V_VoteSummary
    MERGE INTO V_VoteSummary AS target
    USING (
        SELECT
            MemberId,
            KPI,
            AVG(CAST(VoteValue AS DECIMAL(10, 2))) AS AggregateKPIVote,
            COUNT(*) AS Count
        FROM
            V_Vote
        WHERE
            MemberId = @MemberId
            AND KPI = @KPI
        GROUP BY
            MemberId,
            KPI
    ) AS source
    ON
        target.MemberId = source.MemberId
        AND target.KPI = source.KPI
    WHEN MATCHED THEN
        UPDATE SET
            target.AggregateKPIVote = source.AggregateKPIVote,
            target.Count = source.Count
    WHEN NOT MATCHED THEN
        INSERT (MemberId, KPI, AggregateKPIVote, Count)
        VALUES (source.MemberId, source.KPI, source.AggregateKPIVote, source.Count);
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

