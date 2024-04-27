using PublicCommon;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class UrlDomainBasedHandling
    {
    /*url => type of app
    =>if Vote then fix as next-mp...
    =>els if mytown then specific url
    =>else Katthe.com
    => heading,share button url & text
        */
    public static readonly List<string> DomainsHostedWebApp = ["nextmp.azurewebsites.net"];
    public static readonly List<string> DomainsKatthe = AddWwwPrefix(["katthe.com", "katthe.in", "nammadhu.in"]);
    public static readonly List<string> DomainsNextMp = AddWwwPrefix(["mp24.in", "next-mp.in", "samsad.in"]);
    public static readonly List<string> DomainsMyTown = AddWwwPrefix(["bdvt.in", "bhadravathi.com", "kadur.in", "birur.in", "arsikere.in"]);
    public static readonly List<string> DomainsMyProdcuts = AddWwwPrefix(["gombe.in", "holige.in", "hennu.in"]);

    public static List<string> AddWwwPrefix(List<string> domains)
        {
        List<string> result = [];

        foreach (string domain in domains)
            {
            if (!domain.StartsWith("www."))
                {
                result.Add(domain);
                result.Add("www." + domain);
                }
            else
                {
                result.Add(domain.Replace("www.", string.Empty));
                result.Add(domain);

                }
            }

        return result;
        }




    public static string GetLandingPageOfUrl(string url)
        {
        switch (GetApplicationTypeOfUrlDomain(url))
            {
            case ApplicationTypeEnum.MyVote: return "/constituency";//currently only one so default
            case ApplicationTypeEnum.MyTown: return "/mytown";
            case ApplicationTypeEnum.MyProducts:
                if (url.EndsWith("hennu.in/"))
                    return "/hennu";
                else if (url.EndsWith("holige.in/"))//|| Debugger.IsAttached)
                    return "/holige";
                else return "/";
            case ApplicationTypeEnum.Katthe:
            default: return "/";
            }
        }


    public static ApplicationTypeEnum GetApplicationTypeOfUrlDomain(string url)
        {
        switch (url)
            {
            case var _ when url.EndsWith("next-mp.in/"):
            case var _ when url.EndsWith("mp24.in/"):
                return ApplicationTypeEnum.MyVote;


            case var _ when url.EndsWith("bdvt.in/"):
            case var _ when url.EndsWith("bhadravathi.com/"):
            //to debug this os the only switch to change to any different app
            case var _ when Debugger.IsAttached://place this line wherever needed
                return ApplicationTypeEnum.MyTown;


            case var _ when url.EndsWith("hennu.in/"):

            case var _ when url.EndsWith("holige.in/"):

                return ApplicationTypeEnum.MyProducts;

            //case var _ when url.Contains("localhost")://place this line wherever needed
            case var _ when url.EndsWith("katthe.in/"):
            case var _ when url.EndsWith("katthe.com/"):
            default: return ApplicationTypeEnum.Katthe; //katthe.com
            }
        }

    }
