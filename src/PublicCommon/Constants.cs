using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicCommon
    {
    //cachekey refreshinterval set inside VoteCacheKey class refreshInterval

    public enum ApplicationTypeEnum
        {
        Katthe = 0,
        MyVote = 1,
        MyTown = 2,
        MyProducts = 3
        }

    public enum EnvironmentEnum
        {
        Development,
        Test,
        Production
        }
    public static class Constants
        {
        public static class EnvironmentConsts
            {

            public const string ASPNETCORE_ENVIRONMENT = "ASPNETCORE_ENVIRONMENT";
            public static readonly string Name = Environment.GetEnvironmentVariable(ASPNETCORE_ENVIRONMENT);
            public const string Development = nameof(EnvironmentEnum.Development);
            public const string Production = nameof(EnvironmentEnum.Production);
            public const string Test = nameof(EnvironmentEnum.Test);
            public static readonly bool IsDevelopment = (Name == Development);
            public static readonly bool IsProduction = (Name == Production);
            }
        public static class Authentication
            {
            public static class ExternalProviders
                {
                public const string Google = "Google";
                public const string Facebook = "Facebook";
                public const string Twitter = "Twitter";
                public const string Microsoft = "Microsoft";
                }
            }

        }
    }
