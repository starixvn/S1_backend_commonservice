using ArchiveService.Service.Model;
using ArchiveService.Workers;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using static ArchiveService.Extensions.MethodExtension;

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

        public void ArchiveTP14(TpbetrecordModel data, bool delete)
        {
            using var conn = new MySqlConnection(configuration.GetConnectionString("ConnectionString"));
            var parameter = new DynamicParameters(data);
            try
            {
                conn.Execute("Archive_TpBetRecord14_Ins", parameter, commandType: CommandType.StoredProcedure);
                if (delete)
                    conn.Execute("Archive_TpBetRecord_Del", parameter, commandType: CommandType.StoredProcedure);
            }
            catch (MySqlException ex)
            {
                logger.LogError(ex, GetCurrentMethod());
                if (ex.Number == (int)MySqlErrorCode.DuplicateKeyEntry) //匯入資料重複
                {
                    if (delete)
                        conn.Execute("Archive_TpBetRecord_Del", parameter, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, GetCurrentMethod());
            }
        }
        // public void ArchiveDG14(Bettranslc_DGModel data)
        // {
        //     var delete = configuration.GetValue<bool>("DGArchiveCFG:Delete");
        //
        //     using var conn = new MySqlConnection(configuration.GetConnectionString("ConnectionString"));
        //     var parameter = new DynamicParameters(data);
        //     try
        //     {
        //         conn.Execute("Archive_bettranslc_dg14_Ins", parameter, commandType: CommandType.StoredProcedure);
        //         if (delete)
        //             conn.Execute("Archive_bettranslc_dg_Del", parameter, commandType: CommandType.StoredProcedure);
        //     }
        //     catch (MySqlException ex)
        //     {
        //         logger.LogError(ex, GetCurrentMethod());
        //         if (ex.Number == (int)MySqlErrorCode.DuplicateKeyEntry) //匯入資料重複
        //         {
        //             if (delete)
        //                 conn.Execute("Archive_bettranslc_dg_Del", parameter, commandType: CommandType.StoredProcedure);
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         logger.LogError(ex, GetCurrentMethod());
        //     }
        // }
        public void ArchivePP14(Bettransrng_PPModel data, bool delete)
        {
            using var conn = new MySqlConnection(configuration.GetConnectionString("ConnectionString"));
            var parameter = new DynamicParameters(data);
            try
            {
                conn.Execute("Archive_bettransrng_pp14_Ins", parameter, commandType: CommandType.StoredProcedure);
                if (delete)
                    conn.Execute("Archive_bettransrng_pp_Del", parameter, commandType: CommandType.StoredProcedure);
            }
            catch (MySqlException ex)
            {
                logger.LogError(ex, GetCurrentMethod());
                if (ex.Number == (int)MySqlErrorCode.DuplicateKeyEntry) //匯入資料重複
                {
                    if (delete)
                        conn.Execute("Archive_bettransrng_pp_Del", parameter, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, GetCurrentMethod());
            }
        }
        public void ArchiveSG14(BettransRng_SGModel data, bool delete)
        {
            using var conn = new MySqlConnection(configuration.GetConnectionString("ConnectionString"));
            var parameter = new DynamicParameters(data);
            try
            {
                conn.Execute("Archive_BettransRng_SG_Ins", parameter, commandType: CommandType.StoredProcedure);
                if (delete)
                    conn.Execute("Archive_BettransRng_SG_Del", parameter, commandType: CommandType.StoredProcedure);
            }
            catch (MySqlException ex)
            {
                logger.LogError(ex, GetCurrentMethod());
                if (ex.Number == (int)MySqlErrorCode.DuplicateKeyEntry) //匯入資料重複
                {
                    if (delete)
                        conn.Execute("Archive_BettransRng_SG_Del", parameter, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, GetCurrentMethod());
            }
        }
        public void ArchivePG14(BettransRng_PGModel data, bool delete)
        {
            using var conn = new MySqlConnection(configuration.GetConnectionString("ConnectionString"));
            var parameter = new DynamicParameters(data);
            try
            {
                conn.Execute("Archive_BettransRng_PG14_Ins", parameter, commandType: CommandType.StoredProcedure);
                if (delete)
                    conn.Execute("Archive_BettransRng_PG_Del", parameter, commandType: CommandType.StoredProcedure);
            }
            catch (MySqlException ex)
            {
                logger.LogError(ex, GetCurrentMethod());
                if (ex.Number == (int)MySqlErrorCode.DuplicateKeyEntry) //匯入資料重複
                {
                    if (delete)
                        conn.Execute("Archive_BettransRng_PG_Del", parameter, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, GetCurrentMethod());
            }
        }
        public void ArchiveGPI14(BettransRng_GPIModel data, bool delete)
        {
            using var conn = new MySqlConnection(configuration.GetConnectionString("ConnectionString"));
            var parameter = new DynamicParameters(data);
            try
            {
                conn.Execute("Archive_BettransRng_GPI14_Ins", parameter, commandType: CommandType.StoredProcedure);
                if (delete)
                    conn.Execute("Archive_BettransRng_GPI_Del", parameter, commandType: CommandType.StoredProcedure);
            }
            catch (MySqlException ex)
            {
                logger.LogError(ex, GetCurrentMethod());
                if (ex.Number == (int)MySqlErrorCode.DuplicateKeyEntry) //匯入資料重複
                {
                    if (delete)
                        conn.Execute("Archive_BettransRng_GPI_Del", parameter, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, GetCurrentMethod());
            }
        }
        public void ArchiveCP914(BettransRNG_CQ9Model data, bool delete)
        {                                                
            using var conn = new MySqlConnection(configuration.GetConnectionString("ConnectionString"));
            var parameter = new DynamicParameters(data);
            try
            {
                conn.Execute("Archive_BettransRNG_CQ914_Ins", parameter, commandType: CommandType.StoredProcedure);
                if (delete)
                    conn.Execute("Archive_BettransRNG_CQ9_Del", parameter, commandType: CommandType.StoredProcedure);
            }
            catch (MySqlException ex)
            {
                logger.LogError(ex, GetCurrentMethod());
                if (ex.Number == (int)MySqlErrorCode.DuplicateKeyEntry) //匯入資料重複
                {
                    if (delete)
                        conn.Execute("Archive_BettransRNG_CQ9_Del", parameter, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, GetCurrentMethod());
            }
        }
    }
}
