using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PublicCommon.MyVote
{
    public class ConfigurationsForWasmClient
    {
        //common for all apps
        public string PublicDomainAbsoluteUrl { get; set; }//Next-Mp.in
        public string PublicDomainAbsoluteUrlSecond { get; set; }//MP24.in
        public DateTime LoadedOn { get; set; } = DateTime.Now;
        public string? IpAddressClientUser { get; set; }
        public string ContactEmail { get; set; }
        public string SasToken { get; set; }


        //below are specific to app like vote
        public string? SystemType { get; set; }//MP
        public string? CandidateType { get; set; }//MP
    }
}
