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
        public void ArchiveTP14(TpbetrecordModel data)
        {
            bool delete = configuration.GetValue<bool>("TPArchiveCFG:Delete");

            using MySqlConnection conn = new MySqlConnection(configuration.GetConnectionString("ConnectionString"));
            conn.Open();
            try
            {
                conn.Execute("Archive_TpBetRecord14_Ins", data, commandType: CommandType.StoredProcedure);
                if (delete)
                    conn.Execute("Archive_TpBetRecord_Del", data, commandType: CommandType.StoredProcedure);
            }
            catch (MySqlException ex)
            {
                logger.LogError(ex.Message);
                // Console.WriteLine("InsterToTp14 Exception ex.Number ::" + ex.Number);
                if (ex.Number == 1062) //匯入資料重複
                {
                    if (delete)
                        conn.Execute("Archive_TpBetRecord_Del", data, commandType: CommandType.StoredProcedure);
                }
            }
        }
        public void ArchiveDG14(Bettranslc_DGModel data)
        {
            bool delete = configuration.GetValue<bool>("DGArchiveCFG:Delete");

            using MySqlConnection conn = new MySqlConnection(configuration.GetConnectionString("ConnectionString"));
            conn.Open();
            try
            {
                conn.Execute("Archive_bettranslc_dg14_Ins", data, commandType: CommandType.StoredProcedure);
                if (delete)
                    conn.Execute("Archive_bettranslc_dg_Del", data, commandType: CommandType.StoredProcedure);
            }
            catch (MySqlException ex)
            {
                logger.LogError(ex.Message);
                if (ex.Number == 1062) //匯入資料重複
                {
                    if (delete)
                        conn.Execute("Archive_bettranslc_dg_Del", data, commandType: CommandType.StoredProcedure);
                }
            }
        }
        public void ArchivePP14(Bettransrng_PPModel data)
        {
            bool delete = configuration.GetValue<bool>("PPArchiveCFG:Delete");

            using MySqlConnection conn = new MySqlConnection(configuration.GetConnectionString("ConnectionString"));
            conn.Open();
            try
            {
                conn.Execute("Archive_bettransrng_pp14_Ins", data, commandType: CommandType.StoredProcedure);
                if (delete)
                    conn.Execute("Archive_bettransrng_pp_Del", data, commandType: CommandType.StoredProcedure);
            }
            catch (MySqlException ex)
            {
                logger.LogError(ex.Message);
                if (ex.Number == 1062) //匯入資料重複
                {
                    if (delete)
                        conn.Execute("Archive_bettransrng_pp_Del", data, commandType: CommandType.StoredProcedure);
                }
            }
        }
        public void ArchiveSG14(BettransRng_SGModel data)
        {
            bool delete = configuration.GetValue<bool>("SGArchiveCFG:Delete");

            using MySqlConnection conn = new MySqlConnection(configuration.GetConnectionString("ConnectionString"));
            conn.Open();
            try
            {
                conn.Execute("Archive_BettransRng_SG_Ins", data, commandType: CommandType.StoredProcedure);
                if (delete)
                    conn.Execute("Archive_BettransRng_SG_Del", data, commandType: CommandType.StoredProcedure);
            }
            catch (MySqlException ex)
            {
                logger.LogError(ex.Message);
                if (ex.Number == 1062) //匯入資料重複
                {
                    if (delete)
                        conn.Execute("Archive_BettransRng_SG_Del", data, commandType: CommandType.StoredProcedure);
                }
            }
        }
        public void ArchivePG14(BettransRng_PGModel data)
        {
            bool delete = configuration.GetValue<bool>("PGArchiveCFG:Delete");

            using MySqlConnection conn = new MySqlConnection(configuration.GetConnectionString("ConnectionString"));
            conn.Open();
            try
            {
                conn.Execute("Archive_BettransRng_PG14_Ins", data, commandType: CommandType.StoredProcedure);
                if (delete)
                    conn.Execute("Archive_BettransRng_PG_Del", data, commandType: CommandType.StoredProcedure);
            }
            catch (MySqlException ex)
            {
                logger.LogError(ex.Message);
                if (ex.Number == 1062) //匯入資料重複
                {
                    if (delete)
                        conn.Execute("Archive_BettransRng_PG_Del", data, commandType: CommandType.StoredProcedure);
                }
            }
        }
    }
}
