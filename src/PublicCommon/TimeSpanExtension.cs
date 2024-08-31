namespace PublicCommon
    {
    public static class TimeSpanExtension
        {
        public static string TimeSpanConvertTo12Hr(this TimeSpan? timeSpan) => TimeSpanConvertTo12Hr(timeSpan?.ToString());

        public static string TimeSpanConvertTo12Hr(this string? timeSpanInString)
            {//ex: "15:35:20"=>3:35:20 PM ,  "00:35:20"=>12:35:20 AM

            if (string.IsNullOrEmpty(timeSpanInString)) return string.Empty;
            string result = string.Empty;
            // Get Hours
            int h1 = (int)timeSpanInString[0] - '0';
            int h2 = (int)timeSpanInString[1] - '0';

            int hh = h1 * 10 + h2;

            // Finding out the Meridien of time
            // ie. AM or PM
            string Meridien;
            if (hh < 12)
                {
                Meridien = "AM";
                }
            else
                Meridien = "PM";

            hh %= 12;

            // Handle 00 and 12 case separately
            if (hh == 0)
                {
                result = "12";
                Console.Write("12");

                // Printing minutes and seconds
                for (int i = 2; i < 8; ++i)
                    {
                    result += timeSpanInString[i];
                    Console.Write(timeSpanInString[i]);
                    }
                }
            else
                {
                result += hh.ToString();
                Console.Write(hh);
                // Printing minutes and seconds
                for (int i = 2; i < 8; ++i)
                    {
                    result += timeSpanInString[i];
                    Console.Write(timeSpanInString[i]);
                    }
                }

            // After time is printed
            // cout Meridien
            result += " " + Meridien;
            Console.WriteLine(" " + Meridien);
            return result;
            }
        }
    }
