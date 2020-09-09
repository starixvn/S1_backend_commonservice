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
    }
}
