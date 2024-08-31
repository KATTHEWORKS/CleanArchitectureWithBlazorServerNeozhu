using System.Text.Json;
using System.Text.Json.Serialization;
using PublicCommon.Common;

namespace PublicCommon
    {
    //cachekey refreshinterval set inside VoteCacheKey class refreshInterval

    public class ApplicationTypes
        {
        public static readonly List<ApplicationType> Apps = [new ApplicationType( Katthe,"Katthe.in"),
            new(MyVote),
            new(MyTown,new List<string>(){"SmartTown.in" }),
            new(MyProducts)
        ];

        public const string Katthe = "Katthe";
        public const string MyVote = "MyVote";
        public const string MyTown = "SmartTown";
        public const string MyProducts = "MyProducts";
        }

    public class ApplicationType
        {
        public ApplicationType(string name)
            {
            Name = name;
            }
        public ApplicationType(string name, string url) : this(name)
            {
            Urls = [url];
            }
        public ApplicationType(string name, List<string> urls) : this(name)
            {
            Urls = urls;
            }
        public byte Index { get; set; }
        public string Name { get; set; }
        public List<string>? Urls { get; set; }
        }


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
    public static class CONSTANTS
        {
        public static readonly TimeSpan Default_MaxCacheTimeSpan = TimeSpan.FromDays(15);
        public static readonly TimeSpan Default_MinCacheTimeSpan = TimeSpan.FromHours(6);

        public static readonly List<OpenCloseTiming> OpenCloseTimingsDefaultSingle = [
       new OpenCloseTiming(TimeSpan.FromHours(10),TimeSpan.FromHours(18))];
        public static readonly List<OpenCloseTiming> OpenCloseTimingsDouble = [
       new OpenCloseTiming(TimeSpan.FromHours(10),TimeSpan.FromHours(13)),
        new OpenCloseTiming(TimeSpan.FromHours(16),TimeSpan.FromHours(20))];


        public static readonly List<OpenCloseTimingsOfDay> TimingsUsualWeekDaysDefault = new List<OpenCloseTimingsOfDay>
        {
            new OpenCloseTimingsOfDay(DayOfWeek.Monday,OpenCloseTimingsDefaultSingle),
            new OpenCloseTimingsOfDay(DayOfWeek.Tuesday,OpenCloseTimingsDefaultSingle),
            new OpenCloseTimingsOfDay(DayOfWeek.Wednesday,OpenCloseTimingsDefaultSingle),
            new OpenCloseTimingsOfDay(DayOfWeek.Thursday,OpenCloseTimingsDefaultSingle),
            new OpenCloseTimingsOfDay(DayOfWeek.Friday,OpenCloseTimingsDefaultSingle),
            new OpenCloseTimingsOfDay(DayOfWeek.Saturday,[new OpenCloseTiming(TimeSpan.FromHours(10),TimeSpan.FromHours(14))]),
            new OpenCloseTimingsOfDay(DayOfWeek.Sunday)//holiday
        };


        public static readonly JsonSerializerOptions DefaultSerializationJsonOptions = new JsonSerializerOptions()
            {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault | JsonIgnoreCondition.WhenWritingNull,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            WriteIndented = true,
            //PropertyNamingPolicy = JsonNamingPolicy.CamelCase//if necessary
            };
        public const string Base64ImagePrefix = "data:image/";//"data:image/png";


        public const string AuthenticationLoginPath = "authentication/login";
        public const string AuthenticationLogOutPath = "authentication/logout";
        public const string ReturnUrl = "?returnurl=";//"authentication/login?returnUrl=/Town/CardTypes"

        public const string DefaultImageExtension = "JPG";

        public const int DefaultApprovalRequired = 3;
        public const string ClientAnonymous = "AnonymousClient";
        public const string ClientAuthorized = "AuthClient";


        public const string ClientConfigurations = "ClientConfigurations";
        public const string MyTownApp = "MyTown";
        public const string MyTownAppKey = "mYtOWN";
        public const string MyTownAppKeyAuth = "mYtOWNsECURE";
        public const string AppKeyName = "X-Encrypted-Content";

        public const string Bearer = "Bearer";
        public const string Authorization = "Authorization";
        public const string ApplicationJson = "application/json";

        public const string Email = "Email";
        public const string UserId = "UserId";

        public const string LocalApiIssuedJwtKey = "ApiIssuedJwtKey";
        public const string LocalConfigurationKey = "Configuration";

        public static class EnvironmentConsts
            {

            public const string ASPNETCORE_ENVIRONMENT = "ASPNETCORE_ENVIRONMENT";
            public const string StorageAccountConnectionString = "StorageAccountConnectionString";// nameof(StorageAccountConnectionString);
            public static readonly string Name = Environment.GetEnvironmentVariable(ASPNETCORE_ENVIRONMENT) ?? "Development";
            public const string Development = nameof(EnvironmentEnum.Development);
            public const string Production = nameof(EnvironmentEnum.Production);
            public const string Test = nameof(EnvironmentEnum.Test);
            public static readonly bool IsDevelopment = (Name == Development);
            public static readonly bool IsProduction = (Name == Production);
            }
        public static class Auth
            {
            public const string Role_Admin = "Admin";
            public const string Role_InternalAdmin = "InternalAdmin";
            public const string Role_InternalViewer = "InternalViewer";

            public const string Role_TownAdmin = "TownAdmin";
            public const string Role_TownReviewer = "TownReviewer";

            /// <summary>
            /// once card created then he becomes Creator but then he can transfer
            /// </summary>
            public const string Role_CardCreator = "CardCreator";

            /// <summary>
            /// CardOwner by default when he created, otherwise on transferring of card
            /// </summary>
            public const string Role_CardOwner = "CardOwner";

            /// <summary>
            /// when any card verified he gets this (VerifiedCard table has OwnerId)
            /// </summary>
            public const string Role_CardVerifiedOwner = "CardVerifiedOwner";

            /// <summary>
            /// when someone added as reviewer or reviewed by himself
            /// </summary>
            public const string Role_CardVerifiedReviewer = "CardReviewer";


            //Town main page,option works with Owner role only,if owner then only he can edit,not with creator

            //Any AuthenticatedUser //no separate role required
            //Anonymous //no separate role required

            public const string Role_Blocked = "Blocked";

            public static readonly List<string> AdminWriters = [Role_Admin, Role_InternalAdmin];
            public static readonly List<string> AdminViewers = [Role_Admin, Role_InternalAdmin, Role_InternalViewer];
            public static readonly List<string> Approver = [Role_Admin, Role_InternalAdmin, Role_InternalViewer,
                Role_CardCreator,Role_CardOwner,Role_CardVerifiedOwner,Role_CardVerifiedReviewer];


            public static class ExternalProviders
                {
                public const string Google = "Google";
                public const string Facebook = "Facebook";
                public const string Twitter = "Twitter";
                public const string Microsoft = "Microsoft";
                }

            public const string UserIDPData = "UserIDPData";
            }

        }
    }
