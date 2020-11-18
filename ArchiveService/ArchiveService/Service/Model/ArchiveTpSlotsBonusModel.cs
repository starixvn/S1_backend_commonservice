using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiveService.Service.Model
{
    public class ArchiveTpSlotsBonusModel
    {
    }
    public class TpSlotsBonusModel
    {
        public int TpId { set; get; }
        public string TransId { set; get; }
        public int GameType { set; get; }
        public bool DoDelete { set; get; }

    }
}
