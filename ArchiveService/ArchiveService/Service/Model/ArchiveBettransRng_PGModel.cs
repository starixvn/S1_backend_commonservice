using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiveService.Service.Model
{
    public class ArchiveBettransRng_PGModel
    {
    }
    public class BettransRng_PGModel
    {
        public string TransactionId { set; get; }
        public string BetTransactionId { set; get; }
        public string ParentBetId { set; get; }
        public string BetId { set; get; }
        public string PlayerName { set; get; }
        public int GameId { set; get; }
        public string CurrencyCode { set; get; }
        public decimal TransferAmount { set; get; }
        public bool IsEndRound { set; get; }
        public DateTime UpdatedTime { set; get; }
        public DateTime CreateTime { set; get; }
        public int type { set; get; }
        public bool Flag { set; get; }
        public bool DoDelete { set; get; }

    }
}
