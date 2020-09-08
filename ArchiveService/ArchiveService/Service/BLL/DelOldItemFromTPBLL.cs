using ArchiveService.Service.DAL;
using ArchiveService.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiveService.Service.BLL
{
    public class DelOldItemFromTPBLL
    {
        private readonly DelOldItemFromTPService delOldItemFromTPService;

        public DelOldItemFromTPBLL(DelOldItemFromTPService delOldItemFromTPService)
        {
            this.delOldItemFromTPService = delOldItemFromTPService;
        }
        public void DelOldItemFromTP(TpbetrecordModel data)
        {
                delOldItemFromTPService.DelOldItemFromTP(data);
        }
    }
}
