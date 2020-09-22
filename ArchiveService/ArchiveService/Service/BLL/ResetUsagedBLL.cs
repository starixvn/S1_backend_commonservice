using ArchiveService.Service.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiveService.Service.BLL
{
    public class ResetUsagedBLL
    {
        private readonly ResetUsagedService resetUsagedService;

        public ResetUsagedBLL(ResetUsagedService resetUsagedService)
        {
            this.resetUsagedService = resetUsagedService;
        }
        public int ResetUsage()
        {
            return this.resetUsagedService.ResetUsage();
        }
    }
}
