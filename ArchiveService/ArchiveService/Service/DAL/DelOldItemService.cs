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
            using var conn = new MySqlConnection(configuration.GetConnectionString("ConnectionString"));

            conn.Execute("Archive_TpBetRecord_Del", data, commandType: CommandType.StoredProcedure);
        }
        public void DelOldItemFromDG(Bettranslc_DGModel data)
        {
            using var conn = new MySqlConnection(configuration.GetConnectionString("ConnectionString"));

            conn.Execute("Archive_bettranslc_dg_Del", data, commandType: CommandType.StoredProcedure);
        }
        public void DelOldItemFromPP(Bettransrng_PPModel data)
        {
            using var conn = new MySqlConnection(configuration.GetConnectionString("ConnectionString"));

            conn.Execute("Archive_bettransrng_pp_Del", data, commandType: CommandType.StoredProcedure);
        }
        public void DelOldItemFromPG(BettransRng_PGModel data)
        {
            using var conn = new MySqlConnection(configuration.GetConnectionString("ConnectionString"));

            conn.Execute("Archive_BettransRng_PG_Del", data, commandType: CommandType.StoredProcedure);
        }
        public void DelOldItemFromGPI(BettransRng_GPIModel data)
        {
            using var conn = new MySqlConnection(configuration.GetConnectionString("ConnectionString"));

            conn.Execute("Archive_BettransRng_GPI_Del", data, commandType: CommandType.StoredProcedure);
        }
        public void DelOldItemFromTpSlotsBonus(TpSlotsBonusModel data)
        {
            using var conn = new MySqlConnection(configuration.GetConnectionString("ConnectionString"));

            conn.Execute("Archive_TpSlotsBonus_Del", data, commandType: CommandType.StoredProcedure);
        }
        public void DelOldItemFromCQ9(BettransRng_GPIModel data)
        {
            using var conn = new MySqlConnection(configuration.GetConnectionString("ConnectionString"));

            conn.Execute("Archive_BettransRNG_CQ9_Del", data, commandType: CommandType.StoredProcedure);
        }
    }
}
