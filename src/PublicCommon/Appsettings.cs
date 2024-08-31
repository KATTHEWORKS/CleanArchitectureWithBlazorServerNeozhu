using AutoMapper;
using Microsoft.Extensions.Configuration;
using static PublicCommon.CONSTANTS;

namespace PublicCommon
    {
#pragma warning disable
    public class AppConfigurations
        {
        public AppConfigurations() : this(null)
            {

            }
        public AppConfigurations(EnvironmentEnum? nameSetForce = null)
            {
            if (nameSetForce == null)
                {
                if (System.Diagnostics.Debugger.IsAttached)
                    EnvironmentName = EnvironmentConsts.Name ?? EnvironmentConsts.Development;
                else
                    //above IfElse  is only to handle local deployment switch handling
                    EnvironmentName = EnvironmentConsts.Production;
                }
            else { EnvironmentName = Enum.GetName(typeof(EnvironmentEnum), nameSetForce) ?? EnvironmentConsts.Production; }

            Environment.SetEnvironmentVariable(EnvironmentConsts.ASPNETCORE_ENVIRONMENT, EnvironmentName);
            }



        public AppSettings? AppSettings { get; private set; }

        /// <summary>
        /// this should not have any sensitive personal information
        /// </summary>
        public AppSettingsForClient? AppSettingsForClient { get; private set; }
        public string EnvironmentName { get; private set; }

        public void Initialize(IConfiguration configuration, string environmentName, bool isDevelopment)
            {
            //locally Environment.IsDevelopment() for local//this checks for env varibale ASPNETCORE_ENVIRONMENT 
            //string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME"))) null for local,webapp has value


            //configuration.GetConnectionString loads as like below differently
            //in local fetches from only appsettings.json or appsettings.development.json
            //in azure webapp,it reads from appsettings.json but overrides with value of environment variable in webapp

            //so for local,to secure details if not stored in appsettings.development.json then store it in windows machine & feth on on isdebugging then read from environment variable

            //common settings loaded here
            AppSettings = configuration.Get<AppSettings>();
            //this copies exactly like appsettings.json but wont get anything from environment variable
            if (AppSettings != null)
                {
                //madhu continue here
                if (isDevelopment)
                    { //only for local
                    AppSettings.ConnectionStrings.StorageAccountConnectionString = Environment.GetEnvironmentVariable(EnvironmentConsts.StorageAccountConnectionString) ?? "its blank man";
                    }
                else
                    AppSettings.ConnectionStrings.StorageAccountConnectionString =
                        configuration.GetConnectionString(EnvironmentConsts.StorageAccountConnectionString) ?? "its blank mannnn";
                //}

                //configured using azcli command below & it gets filled at runtime
                //az webapp config appsettings set --name nextmp --resource-group rg-Next --settings BUILDNUMBER=$BUILD_BUILDID

                var mapper = new MapperConfiguration(cfg => cfg.AddProfile<AppSettingsToClientSettingsProfile>()).CreateMapper();
                AppSettingsForClient = mapper.Map<AppSettingsForClient>(AppSettings);
                if (!string.IsNullOrEmpty(configuration["BUILDNUMBER"]))
                    AppSettingsForClient.BuildNumber = configuration["BUILDNUMBER"]!;//this is fetched from az pipleine
                else
                    AppSettingsForClient.BuildNumber = "NoBuildNo:" + PublicCommon.DateTimeExtension.CurrentTimeInString;

                foreach (var item in AppSettings.Authentications)
                    {
                    item.ClientSecret = null;
                    }
                }
            }

        }

    public class AppSettingsForClient : AppSettingsBase
        {
        public string BuildNumber { get; set; } = PublicCommon.DateTimeExtension.CurrentTimeInString;
        public DateTime LoadedDate { get; set; } = PublicCommon.DateTimeExtension.CurrentTime;
        }
    public class AppSettingsToClientSettingsProfile : Profile
        {
        public AppSettingsToClientSettingsProfile()
            {
            CreateMap<AppSettings, AppSettingsForClient>();
            }
        }
    public class AppSettings : AppSettingsBase
        {
        // public string BuildNumber { get; set; } = PublicCommon.DateTimeExtension.CurrentTimeInString;
        //here environment specific settings will be added
        public required ConnectionStrings ConnectionStrings { get; set; }//db & storageaccountconnection string both here only

        public required JwtSettings JWTSettings { get; set; }
        }
    public class AppSettingsBase
        {
        public string EnvironmentName { get; set; }
        public string CompanyName { get; set; } = "Katthe Softwares & Solutions, India";
        public string CompanTagLine { get; set; } = "Software & Solutions with a Cause";
        public string CompanyUrl { get; set; } = "https://www.Katthe.com";

        public string ContactEmail { get; set; }


        public string PublicDomain { get; set; }//"next-mp.in"
        public string PublicDomainAbsoluteUrl { get; set; }//"https://www.next-mp.in" this is with https://www. so on display had to remove & use
        public string PublicDomainAbsoluteUrlSecond { get; set; }//"https://www.MP24.in" this is with https://www. so on display had to remove & use

        public string Title { get; set; } = "Katthe Softwares & Solutions with a Cause";//"MP DareDevil Transparent Secure Feedback Voting System & Survey"
        public string? FaviconImage { get; set; } = "_content/BaseBlazorComponentsRCL/images/favicon_KA_32_32.png";
        public string Description { get; set; }//"Know about your Member of Paliament MP and share current situation of your Constituency Problems Corruption Dictatorship Illegal Actions Steps Corrections"
        public string AppVideoUrl { get; set; }//"https://youtu.be/Ktc8GLW3QZo"
        public List<AuthenticationConfigurations> Authentications { get; set; }
        //public string BaseUri { get; set; }//this will not be used ,instead navigationmanager is taking 
        //public bool DetailedErrors { get; set; }
        public LoggingSettings Logging { get; set; }
        public string AllowedHosts { get; set; }
        public bool DetailedErrors { get; set; }
        public string IpAddressClientUser { get; set; }
        public VotingSystem VotingSystem { get; set; }
        }


    public class AuthenticationConfigurations
        {
        public string Type { get; set; }//google,facebook
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }


        public string Authority { get; set; }
        public string ValidIssuer { get; set; }

        public string PostLogoutRedirectUri { get; set; }
        public string RedirectUri { get; set; }

        public string ResponseType { get; set; }

        public string[] Claims { get; set; }

        //   "Authority": "https://accounts.google.com/",
        //"ValidIssuer": "https://accounts.google.com/",
        //"PostLogoutRedirectUri": "https://localhost:7195/authentication/logout-callback",
        //"RedirectUri": "https://localhost:7195/authentication/login-callback",
        //"ResponseType": "id_token"

        }


    //moved from CleanArchitecture.Infrastructure.Identity.Settings
    public class JwtSettings
        {
        /* "JWTSettings": {
    "Key": "C1CF4B7DC4C4175B6618DE4F55CA4AAA",
    "Issuer": "CoreIdentity",
    "Audience": "CoreIdentityUser",
    "DurationInMinutes": 15
  },*/
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public double DurationInMinutes { get; set; }
        }

    public class ConnectionStrings
        {
        public string DefaultConnection { get; set; }
        public string IdentityConnection { get; set; }

        public string FileManagerConnection { get; set; }

        public string StorageAccountConnectionString { get; set; } = Environment.GetEnvironmentVariable(EnvironmentConsts.StorageAccountConnectionString);
        public string ContainerName { get; set; }
        public string RootFolderName { get; set; }

        //"DefaultConnection": "Data Source=.\\sqlexpress;Initial Catalog=25June1;Integrated Security=True;MultipleActiveResultSets=True;TrustServerCertificate=True",
        //"IdentityConnection": "Data Source=.\\sqlexpress;Initial Catalog=CleanA17i;Integrated Security=True;MultipleActiveResultSets=True;TrustServerCertificate=True",
        //"FileManagerConnection": "Data Source=.\\sqlexpress;Initial Catalog=CleanAFile15;Integrated Security=True;MultipleActiveResultSets=True;TrustServerCertificate=True",
        //"StorageAccountConnectionString"
        }

    public class LoggingSettings
        {
        public Dictionary<string, string> LogLevel { get; set; }
        }
    public class VotingSystem
        {
        public string SystemType { get; set; }//MP
        public string CandidateType { get; set; }//MP
                                                 // public string PublicDomainUrl { get; set; }//Next-Mp.in //moved to top level

        }

    }
#pragma warning restore