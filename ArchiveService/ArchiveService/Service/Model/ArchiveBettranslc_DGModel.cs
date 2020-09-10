using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiveService.Service.Model
{
    public class ArchiveBettranslc_DGModel
    {
    }
    public class Bettranslc_DGModel
    {
        public string data { set; get; }
        public string  TransId { set; get; }
        public string  Username { set; get; }
        public decimal  Amount { set; get; }
        public string? Memo { set; get; }
        public int  TicketStatus { set; get; }
        public int  SettleFlag { set; get; }
        public int  TicketType { set; get; }
        public DateTime  CreateTime { set; get; }
        public DateTime? UpdateTime { set; get; }
        public bool DoDelete { set; get; }

    }
}
