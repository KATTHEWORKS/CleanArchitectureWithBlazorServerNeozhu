namespace PublicCommon.Common;

public class QueItem
    {
    public int Index { get; set; }
    public required string Name { get; set; }
    public bool? IsDone { get; set; } = false; // Default to false (not done)
    public string? PhoneNumber { get; set; }
    public string? Description { get; set; }
    public decimal? AmountPaid { get; set; }
    public decimal? TotalFees { get; set; }
    public TimeSpan? SupposedTimeSlot { get; set; }
    }

public class Que
    {
    public Que()
        {
        Q = new();
        }
    public Que(List<QueItem> q)
        {
        Q = q;
        }
    public List<QueItem> Q { get; set; }//will be stored on db
    public int EachSlot = 10;//will be stored on db
    private int _currentToken = 1;

    public int CurrentToken => _currentToken;
    public int Count => Q.Count;

    public void SetQue(List<QueItem> q) => Q = q;

    public void Enqueue(string name, string phoneNumber, string description, decimal amountPaid, decimal totalFees)
        {
        Q ??= new();
        //var newIndex = Q.Count + 1;
        Q.Add(new QueItem
            {
            Index = Q.Count + 1,
            Name = name,
            PhoneNumber = phoneNumber,
            Description = description,
            AmountPaid = amountPaid,
            TotalFees = totalFees
            });
        }
    public void MarkFirstAsDone()
        {
        if (Q.Count == 0)
            {
            //throw new ArgumentOutOfRangeException("index");
            }
        else
            {
            var item = Q.First();
            item.IsDone = true;
            //return item;
            }
        }
    public void MarkAsDone(int index)
        {
        if (Q.Count == 0 || index < 0 || index >= Q.Count)
            {
            throw new ArgumentOutOfRangeException(nameof(index));
            }
        else
            {
            var item = Q[index];
            item.IsDone = true;
            //return item;
            }
        }

    public List<QueItem> GetPendingItems()
        {
        return Q.Where(item => item.IsDone != true).ToList();
        }

    public List<QueItem> GetCompletedItems()
        {
        return Q.Where(item => item.IsDone == true).ToList();
        }
    public string GetCurrentPosition()
        {
        var currentIndex = GetPendingItems().FindIndex(item => item.Index == _currentToken);
        return $"{currentIndex + 1}/{GetPendingItems().Count}";
        }
    public string GetSupposedTimeSlotByToken(int tokenNumber)
        {
        var targetItem = GetPendingItems().FirstOrDefault(item => item.Index == tokenNumber);

        if (targetItem == null)
            {
            return "Invalid token number";
            }

        var slotDuration = TimeSpan.FromMinutes(EachSlot); // Adjust as needed
        var supposedStartTime = (tokenNumber - 1) * slotDuration;
        var supposedEndTime = supposedStartTime + slotDuration;

        return $"{supposedStartTime.TotalHours:00}:{supposedStartTime.Minutes:00} - {supposedEndTime.TotalHours:00}:{supposedEndTime.Minutes:00}";
        }
    }






