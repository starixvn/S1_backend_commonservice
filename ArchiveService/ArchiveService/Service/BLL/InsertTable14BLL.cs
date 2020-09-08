using ArchiveService.Service.DAL;
using ArchiveService.Service.Model;
using MySql.Data.MySqlClient;
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
        public bool InsterToTp14(TpbetrecordModel data)
        {
            try
            {
                insertTable14Service.InsterToTp14(data);
                return true;
            }
            catch (MySqlException ex)
            {
              //  Console.WriteLine("InsterToTp14 Exception ex.Number ::" + ex.Number);
                if (ex.Number == 1062) //匯入資料重複
                {
                    return true;
                }
            }

            return false;
        }
        public void Archive14(TpbetrecordModel data)
        {
                insertTable14Service.Archive14(data);
        }

    }
}
