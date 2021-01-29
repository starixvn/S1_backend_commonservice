using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiveService.Service.Model
{
    public class ArchiveTpbetrecordModel
    {
    }
    public class TpbetrecordModel
    {
        public int TpId { set; get; }
        public string TransId { set; get; }
        public int GameType { set; get; }
        public string CustId { set; get; }
        public string UserName { set; get; }
        public int CurrencyId { set; get; }
        public int SiteId { set; get; }
        public string TpUserName { set; get; }
        public DateTime TransTime { set; get; }
        public DateTime SettledTime { set; get; }
        public decimal Stake { set; get; }
        public decimal? ValidBet { set; get; }
        public decimal? Revenue { set; get; }
        public decimal WinlostAmount { set; get; }
        public int SettlementType { set; get; }
        public int TicketStatus { set; get; }
        public string? OriginalState { set; get; }
        public long TicketKey { set; get; }
        public int BonusId { set; get; }
        public DateTime CreateTime { set; get; }
        public bool DoDelete { set; get; }
        
    }
}
