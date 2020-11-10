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
            using MySqlConnection conn = new MySqlConnection(configuration.GetConnectionString("ConnectionString"));
            return conn.Query<TpbetrecordModel>("Archive_TpBetRecord_Get", new { LIMINNUM = LinminNum }, commandType: CommandType.StoredProcedure);
        }
        public IEnumerable<Bettranslc_DGModel> GetDGData(int LinminNum)
        {
            using MySqlConnection conn = new MySqlConnection(configuration.GetConnectionString("ConnectionString"));
            return conn.Query<Bettranslc_DGModel>("Archive_bettranslc_dg_Get", new { LIMINNUM = LinminNum }, commandType: CommandType.StoredProcedure);
        }
        public IEnumerable<Bettransrng_PPModel> GetPPData(int LinminNum)
        {
            using MySqlConnection conn = new MySqlConnection(configuration.GetConnectionString("ConnectionString"));
            return conn.Query<Bettransrng_PPModel>("Archive_bettransrng_pp_Get", new { LIMINNUM = LinminNum }, commandType: CommandType.StoredProcedure);
        }
        public IEnumerable<BettransRng_SGModel> GetSGData(int LinminNum)
        {
            using MySqlConnection conn = new MySqlConnection(configuration.GetConnectionString("ConnectionString"));
            return conn.Query<BettransRng_SGModel>("Archive_BettransRng_SG_Get", new { LIMINNUM = LinminNum }, commandType: CommandType.StoredProcedure);
        }
        public IEnumerable<BettransRng_PGModel> GetPGData(int LinminNum)
        {
            using MySqlConnection conn = new MySqlConnection(configuration.GetConnectionString("ConnectionString"));
            return conn.Query<BettransRng_PGModel>("Archive_BettransRng_PG_Get", new { LIMINNUM = LinminNum }, commandType: CommandType.StoredProcedure);
        }


    }
}
