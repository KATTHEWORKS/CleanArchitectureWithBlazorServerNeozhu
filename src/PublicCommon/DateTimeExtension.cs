namespace PublicCommon
    {
    public static class DateTimeExtension
        {
        //todo must in prod add to utc ,so better ,then can change to anything easily
        public static DateTime CurrentTime => DateTime.Now;
        public static string CurrentTimeInString => DateTime.Now.ToString();
        }
    }
