using ArchiveService.Service.DAL;
using ArchiveService.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiveService.Service.BLL
{
    public class QueryTableBLL
    {
        private readonly QueryTableService queryTableService;

        public QueryTableBLL(QueryTableService queryTableService)
        {
            this.queryTableService = queryTableService;
        }
        public IEnumerable<TpbetrecordModel> GetTpData(int LinminNum)
        {
            return queryTableService.GetTpData(LinminNum);
        }
    }
}
