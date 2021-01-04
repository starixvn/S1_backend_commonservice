using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiveService.Service.Model
{
    public class ArchiveBettransRNG_CQ9Model
    {
    }
    public class BettransRNG_CQ9Model
    {
        public string mtcode { set; get; }
        public string roundid { set; get; }
        public string account { set; get; }
        public DateTime eventTime { set; get; }
        public string gamehall { set; get; }
        public string gamecode { set; get; }
        public decimal amount { set; get; }
        public decimal validbet { set; get; }
        public DateTime ?createTime { set; get; }
        public decimal Before { set; get; }
        public decimal Balance { set; get; }
        public string Actions { set; get; }
        public int TicketType { set; get; }
        public int TicketStatus { set; get; }
        public int SettleFlag { set; get; }
        public DateTime CreateTimes { set; get; }
        public DateTime UpdateTime { set; get; }
        public bool DoDelete { set; get; }

    }
}
