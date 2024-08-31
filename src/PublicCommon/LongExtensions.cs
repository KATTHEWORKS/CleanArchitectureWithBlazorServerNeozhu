namespace PublicCommon.Extensions
    {
    public static class LongExtensions
        {
        public static int ToIntSafe(this long value)
            {
            if (value < int.MinValue || value > int.MaxValue)
                {
                return (int)value; // Return long as int to avoid losing data
                }
            return Convert.ToInt32(value); // Safe conversion if within int range
            }
        }
    }
