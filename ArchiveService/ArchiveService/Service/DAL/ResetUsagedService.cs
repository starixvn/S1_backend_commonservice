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
    public class ResetUsagedService
    {
        private readonly IConfiguration configuration;

        public ResetUsagedService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public int ResetUsage()
        {
            var returncode = 0;
            using var conn = new MySqlConnection(configuration.GetConnectionString("ConnectionString"));
            var BonusCode = configuration.GetSection("ResetUsaged:BonusCode").Get<string[]>();
            conn.Open();
            foreach (var item in BonusCode)
            {
                var p = new DynamicParameters(new { BonusCode = item });
                p.Add("returncode", returncode, dbType: DbType.Int32, direction: ParameterDirection.Output);
                conn.Execute("Service_PromotionSpecific_ResetUsage_Set", p, commandType: CommandType.StoredProcedure);
                returncode = p.Get<int?>("returncode") ?? 0;
            }
         //   Console.WriteLine("ResetUsage:" + returncode);
            return returncode;

        }
    }
}
