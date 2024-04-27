using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.MyVote
{
    public class VoteSupportOppose(int constituencyId, int voteId, Guid supporterUserId, bool? support)
    {
        public int ConstituencyId { get; set; } = constituencyId;
        public int VoteId { get; set; } = voteId;
        public Guid SupporterUserId { get; set; } = supporterUserId;
        public bool? Support { get; set; } = support;
    }
}
