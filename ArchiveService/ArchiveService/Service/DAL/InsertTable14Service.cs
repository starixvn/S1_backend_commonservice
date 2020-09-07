using ArchiveService.Service.Model;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiveService.Service.DAL
{
    public class InsertTable14Service
    {
        private readonly IConfiguration configuration;
        public InsertTable14Service(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public void InsterToTp14AndDelOld(IEnumerable<TpbetrecordModel> data)
        {
            using MySqlConnection conn = new MySqlConnection(configuration.GetConnectionString("ConnectionString"));

            conn.Execute("David_Test_tpbetrecord14_Ins", data, commandType: CommandType.StoredProcedure);
        }
    }
}
