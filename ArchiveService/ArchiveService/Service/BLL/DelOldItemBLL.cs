using ArchiveService.Service.DAL;
using ArchiveService.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiveService.Service.BLL
{
    public class DelOldItemBLL
    {
        private readonly DelOldItemService delOldItemService;

        public DelOldItemBLL(DelOldItemService delOldItemService)
        {
            this.delOldItemService = delOldItemService;
        }
        public void DelOldItemFromTP(TpbetrecordModel data)
        {
                delOldItemService.DelOldItemFromTP(data);
        }
        public void DelOldItemFromDG(Bettranslc_DGModel data)
        {
            delOldItemService.DelOldItemFromDG(data);
        }
        public void DelOldItemFromPP(Bettransrng_PPModel data)
        {
            delOldItemService.DelOldItemFromPP(data);
        }
        public void DelOldItemFromPG(BettransRng_PGModel data)
        {
            delOldItemService.DelOldItemFromPG(data);
        }
        public void DelOldItemFromGPI(BettransRng_GPIModel data)
        {
            delOldItemService.DelOldItemFromGPI(data);
        }
    }
}
