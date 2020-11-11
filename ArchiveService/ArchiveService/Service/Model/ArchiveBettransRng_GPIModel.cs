using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiveService.Service.Model
{
    public class ArchiveBettransRng_GPIModel
    {
    }
    public class BettransRng_GPIModel
    {
        public string trx_id { set; get; }
        public string cust_id { set; get; }
        public string currency { set; get; }
        public decimal amount { set; get; }
        public string? Operator { set; get; }
        public string game_id { set; get; }
        public string round_id { set; get; }
        public string? bundle_id { set; get; }
        public decimal? jackpot_contribution { set; get; }
        public decimal? jackpot_win { set; get; }
        public string platform { set; get; }
        public DateTime CreateTime { set; get; }
        public int? type { set; get; }
        public int? SettleFlag { set; get; }
        public DateTime? UpdateTime { set; get; }
        public bool DoDelete { set; get; }

    }
}
