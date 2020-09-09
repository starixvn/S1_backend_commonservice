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

        // string connetStr = "server=127.0.0.1;port=3306;user=root;password=1234; database=Siebog;";
        //using var sqlConn = new MySqlConnection(configuration.);
        //sqlConn.Execute("David_Test_tpbetrecord14_Ins", data, commandType: CommandType.StoredProcedure);
    }
}
