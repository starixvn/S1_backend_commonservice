using ArchiveService.Service.BLL;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ArchiveService.Workers
{
    public class HistoryWorker : BackgroundService
    {
        private readonly ILogger<HistoryWorker> logger;
        private readonly IHostEnvironment environment;
        private readonly QuaryTableBLL quaryTableBLL;

        public InsertTable14BLL insertTable14BLL { get; }

        private Timer timer;
        private readonly IConfiguration configuration;

        public HistoryWorker(
            ILogger<HistoryWorker> logger,
            IHostEnvironment environment,
            QuaryTableBLL quaryTableBLL,
            InsertTable14BLL insertTable14BLL,
            IConfiguration configuration
        )
        {
            this.logger = logger;
            this.environment = environment;
            this.quaryTableBLL = quaryTableBLL;
            this.insertTable14BLL = insertTable14BLL;
            this.configuration = configuration;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {

            timer = new Timer(DoTask, null, TimeSpan.Zero, TimeSpan.Zero);
            return Task.CompletedTask;
        }
        private void DoTask(object state)
        {

         
            var beginAt = new DateTimeOffset(DateTime.Now);
            var execTime = beginAt.ToOffset(new TimeSpan(8, 0, 0)).ToString("HHmm");
            
            if (execTime != configuration.GetValue<string>("StartTime"))
            {
                Console.WriteLine("execTime = "+ execTime+ " StartTime = "+ configuration.GetValue<string>("StartTime"));
                return;
            }
            Console.WriteLine("HERE");
            Console.WriteLine(execTime);

            try
            {
                int StartTimes = System.Environment.TickCount;
               
                var data = quaryTableBLL.GetTpData(configuration.GetValue<int>("LiminNum"));
                insertTable14BLL.InsterToTp14AndDelOld(data);


          
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
            finally
            {
                TimeSpan dueTimeSpan = TimeSpan.FromSeconds(5);
                timer.Change(dueTimeSpan, TimeSpan.Zero);
            }
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            //WriteToFile("WriteToFileHostedService: Process Stopped");
            timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}
