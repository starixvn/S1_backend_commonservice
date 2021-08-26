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

        public void ArchiveTP14(TpbetrecordModel data, bool delete)
        {
            insertTable14Service.ArchiveTP14(data, delete);
        }
        // public void ArchiveDG14(Bettranslc_DGModel data)
        // {
        //     insertTable14Service.ArchiveDG14(data);
        // }
        public void ArchivePP14(Bettransrng_PPModel data, bool delete)
        {
            insertTable14Service.ArchivePP14(data, delete);
        }
        public void ArchiveSG14(BettransRng_SGModel data, bool delete)
        {
            insertTable14Service.ArchiveSG14(data, delete);
        }
        public void ArchivePG14(BettransRng_PGModel data, bool delete)
        {
            insertTable14Service.ArchivePG14(data, delete);
        }
        public void ArchiveGPI14(BettransRng_GPIModel data, bool delete)
        {
            insertTable14Service.ArchiveGPI14(data, delete);
        }
        public void ArchiveCP914(BettransRNG_CQ9Model data, bool delete)
        {
            insertTable14Service.ArchiveCP914(data, delete);
        }
    }
}
