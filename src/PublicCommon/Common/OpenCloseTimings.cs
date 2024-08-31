namespace PublicCommon.Common;

public class OpenCloseTiming
    {
    public OpenCloseTiming()
        {

        }
    public OpenCloseTiming(TimeSpan? open, TimeSpan? close)
        {
        OpenTime = open;
        CloseTime = close;
        }
    public TimeSpan? OpenTime { get; set; } //= TimeSpan.FromHours(10) + TimeSpan.FromMinutes(30);//10.30am
    public TimeSpan? CloseTime { get; set; } //= TimeSpan.FromHours(18) + TimeSpan.FromMinutes(30);//6.30pm

    public static List<OpenCloseTiming>? DeSerializeOpenCloseTimings(string? openCloseTimingsJson, List<OpenCloseTimingsOfDay>? timingsUsual = null)
        {
        // return !string.IsNullOrEmpty(timingsTodayJson)?JsonSerializer.Deserialize<List<Timing>>(timingsTodayJson):null;

        //if (!string.IsNullOrEmpty(timingsTodayJson))
        //return  JsonSerializer.Deserialize<List<Timing>>(timingsTodayJson);
        if (JsonExtensions.TryDeserialize<List<OpenCloseTiming>>(openCloseTimingsJson, out List<OpenCloseTiming> result))
            return result;
        else return timingsUsual?.FirstOrDefault(t => t.Day == DateTime.Today.DayOfWeek)?.Timings;
        }
    public static List<OpenCloseTiming>? DeSerializeTimings(string? timingsTodayJson, string? timingsRegularJson = null)
        {
        // return !string.IsNullOrEmpty(timingsTodayJson)?JsonSerializer.Deserialize<List<Timing>>(timingsTodayJson):null;

        //if (!string.IsNullOrEmpty(timingsTodayJson))
        //return  JsonSerializer.Deserialize<List<Timing>>(timingsTodayJson);
        if (JsonExtensions.TryDeserialize<List<OpenCloseTiming>>(timingsTodayJson, out List<OpenCloseTiming> result))
            return result;
        else return OpenCloseTimingsOfDay.DeserializeTimingsUsual(timingsRegularJson)?
                .FirstOrDefault(t => t.Day == DateTime.Today.DayOfWeek)?.Timings;
        }
    public static string? SerializeTimings(List<OpenCloseTiming>? timings) => JsonExtensions.Serialize(timings);
    }

public class OpenCloseTimingsOfDay
    {
    public DayOfWeek Day { get; set; }
    public List<OpenCloseTiming>? Timings { get; set; }
    public OpenCloseTimingsOfDay()
        {

        }
    public OpenCloseTimingsOfDay(DayOfWeek day)
        {
        Day = day;
        }
    public OpenCloseTimingsOfDay(DayOfWeek day, List<OpenCloseTiming> timings)
        {
        Day = day; Timings = timings;
        }

    public static List<OpenCloseTimingsOfDay>? DeserializeTimingsUsual(string? timingsRegularJson)
        {
        //return !string.IsNullOrEmpty(timingsUsualJson) ? JsonSerializer.Deserialize<List<WeeklyTiming>>(timingsUsualJson) : null;
        return JsonExtensions.TryDeserialize<List<OpenCloseTimingsOfDay>?>(timingsRegularJson, out List<OpenCloseTimingsOfDay>? result) ? result : null;
        }
    public static string? SerializeTimingsUsual(List<OpenCloseTimingsOfDay>? timings) => JsonExtensions.Serialize(timings);
    }

