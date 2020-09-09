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
    public class DelOldItemService
    {
        private readonly IConfiguration configuration;
        public DelOldItemService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public void DelOldItemFromTP(TpbetrecordModel data)
        {
            using MySqlConnection conn = new MySqlConnection(configuration.GetConnectionString("ConnectionString"));

            conn.Execute("Archive_TpBetRecord_Del", data, commandType: CommandType.StoredProcedure);
        }
    }
}
