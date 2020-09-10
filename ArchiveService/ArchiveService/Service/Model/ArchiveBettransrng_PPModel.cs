using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiveService.Service.Model
{
    public class ArchiveBettransrng_PPModel
    {
    }
    public class Bettransrng_PPModel
    {
        public string  reference { set; get; }
        public long  roundId { set; get; }
        public string  gameId { set; get; }
        public decimal  amount { set; get; }
        public string userId { set; get; }
        public string? roundDetails { set; get; }
        public long timestamp { set; get; }
        public string? bonusCode { set; get; }
        public long? jackpotId { set; get; }
        public decimal? jackpotContribution { set; get; }
        public DateTime CreateTime { set; get; }
        public int type { set; get; }
        public int TicketStatus { set; get; }
        public int Flag { set; get; }
        public decimal? promoWinAmount { set; get; }
        public string? promoWinReference { set; get; }
        public long? promoCampaignID { set; get; }
        public string? promoCampaignType { set; get; }
        public bool DoDelete { set; get; }
    }
}
