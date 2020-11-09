using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiveService.Service.Model
{
    public class ArchiveBettransRng_SGModel
    {
    }
    public class BettransRng_SGModel
    {
        public string transferId { set; get; }
        public string acctId { set; get; }
        public string currency { set; get; }
        public decimal amount { set; get; }
        public int type { set; get; }
        public string channel { set; get; }
        public string playerIp { set; get; }
        public DateTime transferTime { set; get; }
        public string gameCode { set; get; }
        public string ticketId { set; get; }
        public string referenceId { set; get; }
        public string specialGame { set; get; }
        public string refTicketIds { set; get; }
        public DateTime CreateTime { set; get; }
        public int TicketStatus { set; get; }
        public int? SettleFlag { set; get; }
        public bool DoDelete { set; get; }

    }
}
