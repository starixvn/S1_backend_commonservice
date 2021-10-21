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
    public class QueryTableService
    {
        private readonly IConfiguration configuration;

        public QueryTableService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IEnumerable<TpbetrecordModel> GetTpData(int LinminNum)
        {
            using var conn = new MySqlConnection(configuration.GetConnectionString("ConnectionStringReport"));
            return conn.Query<TpbetrecordModel>("Archive_TpBetRecord_Get", new { LIMINNUM = LinminNum }, commandType: CommandType.StoredProcedure);
        }
        public IEnumerable<Bettranslc_DGModel> GetDGData(int LinminNum)
        {
            using var conn = new MySqlConnection(configuration.GetConnectionString("ConnectionStringReport"));
            return conn.Query<Bettranslc_DGModel>("Archive_bettranslc_dg_Get", new { LIMINNUM = LinminNum }, commandType: CommandType.StoredProcedure);
        }
        public IEnumerable<Bettransrng_PPModel> GetPPData(int LinminNum)
        {
            using var conn = new MySqlConnection(configuration.GetConnectionString("ConnectionStringReport"));
            return conn.Query<Bettransrng_PPModel>("Archive_bettransrng_pp_Get", new { LIMINNUM = LinminNum }, commandType: CommandType.StoredProcedure);
        }
        public IEnumerable<BettransRng_SGModel> GetSGData(int LinminNum)
        {
            using var conn = new MySqlConnection(configuration.GetConnectionString("ConnectionStringReport"));
            return conn.Query<BettransRng_SGModel>("Archive_BettransRng_SG_Get", new { LIMINNUM = LinminNum }, commandType: CommandType.StoredProcedure);
        }
        public IEnumerable<BettransRng_PGModel> GetPGData(int LinminNum)
        {
            using var conn = new MySqlConnection(configuration.GetConnectionString("ConnectionStringReport"));
            return conn.Query<BettransRng_PGModel>("Archive_BettransRng_PG_Get", new { LIMINNUM = LinminNum }, commandType: CommandType.StoredProcedure);
        }
        public IEnumerable<BettransRng_GPIModel> GetGPIData(int LinminNum)
        {
            using var conn = new MySqlConnection(configuration.GetConnectionString("ConnectionStringReport"));
            return conn.Query<BettransRng_GPIModel>("Archive_BettransRng_GPI_Get", new { LIMINNUM = LinminNum }, commandType: CommandType.StoredProcedure);
        }
        public IEnumerable<TpSlotsBonusModel> GetTpSlotsBonusData(int LinminNum)
        {
            using var conn = new MySqlConnection(configuration.GetConnectionString("ConnectionStringReport"));
            return conn.Query<TpSlotsBonusModel>("Archive_TpSlotsBonus_Get", new { LIMINNUM = LinminNum }, commandType: CommandType.StoredProcedure);
        }
        public IEnumerable<BettransRNG_CQ9Model> GetCQ9Data(int LinminNum)
        {
            using var conn = new MySqlConnection(configuration.GetConnectionString("ConnectionStringReport"));
            return conn.Query<BettransRNG_CQ9Model>("Archive_BettransRNG_CQ9_Get", new { LIMINNUM = LinminNum }, commandType: CommandType.StoredProcedure);
        }


    }
}
