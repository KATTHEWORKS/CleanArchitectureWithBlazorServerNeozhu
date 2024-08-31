namespace PublicCommon
    {
    /// <summary>
    /// MyLogger.Log("Message to log file", LogLevel.Info, true);
    /*
     * // Development environment
Environment.SetEnvironmentVariable("ENVIRONMENT", "Development");

MyLogger.Log("Informational message"); // Will print (Info level)
MyLogger.Log("Debug message", LogLevel.Verbose); // Will print (Verbose level)
MyLogger.Log("Error message", LogLevel.Error); // Will print (Error level)

// Production environment (no verbose logging)
Environment.SetEnvironmentVariable("ENVIRONMENT", "Production");

MyLogger.Log("Informational message"); // Won't print
MyLogger.Log("Debug message", LogLevel.Verbose); // Won't print
MyLogger.Log("Error message", LogLevel.Error); // Will print

// Production environment with verbose logging
Environment.SetEnvironmentVariable("ENVIRONMENT", "Production");
Environment.SetEnvironmentVariable("VERBOSE", "true");

MyLogger.Log("Informational message"); // Will print
MyLogger.Log("Debug message", LogLevel.Verbose); // Will print

     */
    /// </summary>
    public static class MyLogger
        {
        private static readonly bool IsDevelopment = true;//Environment.GetEnvironmentVariable("ENVIRONMENT")?.ToLower(System.Globalization.CultureInfo.CurrentCulture) == "development";
        private static readonly bool IsVerbose = Environment.GetEnvironmentVariable("VERBOSE")?.ToLower() == "true";
        private static readonly string LogFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "myapp.log"); // Change "myapp" to your application name

        public static void Log(string message, LogLevel level = LogLevel.Info, bool writeToLogFile = false)
            {
            if ((IsDevelopment && level <= LogLevel.Info) || (IsVerbose && level <= LogLevel.Verbose))
                {
                Console.WriteLine($"{level}: {message}");
                }

            if (writeToLogFile)
                {
                WriteToFile($"{level}: {message}");
                }
            }

        private static void WriteToFile(string message)
            {
            using (StreamWriter writer = File.AppendText(LogFilePath))
                {
                writer.WriteLine(message);
                }
            }

        public enum LogLevel
            {
            Info,
            Verbose,
            Error
            }
        }

    }
