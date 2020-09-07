using ArchiveService.Service.DAL;
using ArchiveService.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiveService.Service.BLL
{
    public class QuaryTableBLL
    {
        private readonly QuaryTableService quaryTableService;

        public QuaryTableBLL(QuaryTableService quaryTableService)
        {
            this.quaryTableService = quaryTableService;
        }
        public IEnumerable<TpbetrecordModel> GetTpData(int LinminNum)
        {
            return quaryTableService.GetTpData(LinminNum);
        }
    }
}
