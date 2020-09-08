using ArchiveService.Service.Model;
using ArchiveService.Workers;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<HistoryWorker> logger;
        private readonly IConfiguration configuration;
        public InsertTable14Service(IConfiguration configuration,
            ILogger<HistoryWorker> logger)
        {
            this.logger = logger;
            this.configuration = configuration;
        }
        public void InsterToTp14(TpbetrecordModel data)
        {
            using MySqlConnection conn = new MySqlConnection(configuration.GetConnectionString("ConnectionString"));
            conn.Execute("Archive_TpBetRecord14_Ins", data, commandType: CommandType.StoredProcedure);
        }
        public void Archive14(TpbetrecordModel data)
        {
            bool delete = configuration.GetValue<bool>("Delete");

            using MySqlConnection conn = new MySqlConnection(configuration.GetConnectionString("ConnectionString"));
            conn.Open();
            try
            {
                conn.Execute("Archive_TpBetRecord14_Ins", data, commandType: CommandType.StoredProcedure);
                if (!delete)
                    conn.Execute("Archive_TpBetRecord_Del", data, commandType: CommandType.StoredProcedure);
            }
            catch (MySqlException ex)
            {
                logger.LogError(ex.Message);
                // Console.WriteLine("InsterToTp14 Exception ex.Number ::" + ex.Number);
                if (ex.Number == 1062) //匯入資料重複
                {
                    if (!delete)
                        conn.Execute("Archive_TpBetRecord_Del", data, commandType: CommandType.StoredProcedure);
                }
            }
        }
    }
}
