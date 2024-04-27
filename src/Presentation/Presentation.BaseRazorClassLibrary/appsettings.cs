using Microsoft.Extensions.Configuration;
using PublicCommon;
using System.Net;
using static PublicCommon.Constants;

namespace Presentation.BaseRazorClassLibrary
{
    public static class AppConfigurations
        {
        public static AppSettings AppSettings { get; private set; }
        public static string EnvironmentName { get; private set; }
        public static void SetEnvironment(EnvironmentEnum? nameSetForce = null)
            {
            if (nameSetForce == null)
                {
                if (System.Diagnostics.Debugger.IsAttached)
                    EnvironmentName = EnvironmentConsts.Name ?? EnvironmentConsts.Development;
                else
                    //above IfElse  is only to handle local deployment switch handling
                    EnvironmentName = EnvironmentConsts.Production;
                }
            else { EnvironmentName = Enum.GetName(typeof(EnvironmentEnum), nameSetForce); }
            Environment.SetEnvironmentVariable(EnvironmentConsts.ASPNETCORE_ENVIRONMENT, EnvironmentName);
            }
        public static void Initialize(IConfiguration configuration)
            {
            //common settings loaded here
            AppSettings = configuration.Get<AppSettings>();
            if (AppSettings != null)
                {
                if (!string.IsNullOrEmpty(configuration["BUILDNUMBER"]))
                    AppSettings.BuildNumber = configuration["BUILDNUMBER"]!;//this is fetched from az pipleine
                else
                    AppSettings.BuildNumber = "NoBuildNo:" + DateTime.Now.ToString();
                //configured using azcli command below & it gets filled at runtime
                //az webapp config appsettings set --name nextmp --resource-group rg-Next --settings BUILDNUMBER=$BUILD_BUILDID
                }

            //var connectionString = _configuration.GetConnectionString("BlobStorage");
            }

        }
    public class AppSettings : AppSettingsBase
        {
        public string BuildNumber { get; set; } = DateTime.Now.ToString();
        //here environment specific settings will be added
        public required ConnectionStrings ConnectionStrings { get; set; }
        //if needed add authentication also

        }
    public class AppSettingsBase
        {
        public string CompanyName { get; set; } = "Katthe Softwares & Solutions, India";
        public string CompanTagLine { get; set; } = "Software & Solutions with a Cause";
        public string CompanyUrl { get; set; } = "https://www.Katthe.com";

        public string ContactEmail { get; set; }


        public string PublicDomain { get; set; }//"next-mp.in"
        public string PublicDomainAbsoluteUrl { get; set; }//"https://www.next-mp.in" this is with https://www. so on display had to remove & use
        public string PublicDomainAbsoluteUrlSecond { get; set; }//"https://www.MP24.in" this is with https://www. so on display had to remove & use

        public string Title { get; set; } = "Katthe Softwares & Solutions with a Cause";//"MP DareDevil Transparent Secure Feedback Voting System & Survey"
        public string? FaviconImage { get; set; } = "_content/BaseBlazorComponentsRCL/images/faviconKATTHELogo.png";
        public string Description { get; set; }//"Know about your Member of Paliament MP and share current situation of your Constituency Problems Corruption Dictatorship Illegal Actions Steps Corrections"
        public string AppVideoUrl { get; set; }//"https://youtu.be/Ktc8GLW3QZo"
        public List<Authentication> Authentications { get; set; }
        //public string BaseUri { get; set; }//this will not be used ,instead navigationmanager is taking 
        //public bool DetailedErrors { get; set; }
        public LoggingSettings Logging { get; set; }
        public string AllowedHosts { get; set; }
        public bool DetailedErrors { get; set; }
        public VotingSystem VotingSystem { get; set; }
        }


    public class Authentication
        {
        public string Type { get; set; }//google,facebook
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        }
    public class ConnectionStrings
        {
        public string DatabaseConnection { get; set; }
        public string BlobStorage { get; set; }
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
        public IPAddress IpAddressOfUser { get; set; }
        }

    }
