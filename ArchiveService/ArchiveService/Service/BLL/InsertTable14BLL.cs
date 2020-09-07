using ArchiveService.Service.DAL;
using ArchiveService.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiveService.Service.BLL
{
    public class InsertTable14BLL
    {
        private readonly InsertTable14Service insertTable14Service;

        public InsertTable14BLL(InsertTable14Service insertTable14Service)
        {
            this.insertTable14Service = insertTable14Service;
        }
        public void InsterToTp14AndDelOld(IEnumerable<TpbetrecordModel> data)
        {
            insertTable14Service.InsterToTp14AndDelOld(data);
        }

    }
}
