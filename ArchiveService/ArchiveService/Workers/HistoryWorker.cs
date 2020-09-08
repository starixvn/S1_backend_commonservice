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

        private readonly DelOldItemFromTPBLL delOldItemFromTPBLL;
        private Timer timer;
        private readonly IConfiguration configuration;
        private string processDate = "";

        public HistoryWorker(
            ILogger<HistoryWorker> logger,
            IHostEnvironment environment,
            QuaryTableBLL quaryTableBLL,
            InsertTable14BLL insertTable14BLL,
            DelOldItemFromTPBLL delOldItemFromTPBLL,
            IConfiguration configuration
        )
        {
            this.logger = logger;
            this.environment = environment;
            this.quaryTableBLL = quaryTableBLL;
            this.insertTable14BLL = insertTable14BLL;
            this.delOldItemFromTPBLL = delOldItemFromTPBLL;
            this.configuration = configuration;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {

            timer = new Timer(DoTask, null, TimeSpan.Zero, TimeSpan.Zero);
            return Task.CompletedTask;
        }
        private void DoTask(object state)
        {
           // Console.WriteLine("DoTask start");


            var beginAt = new DateTimeOffset(DateTime.Now);
            var execTime = beginAt.ToOffset(new TimeSpan(8, 0, 0)).ToString("HH:mm");
            var execData = beginAt.ToOffset(new TimeSpan(8, 0, 0)).ToString("MMdd");

            string today = execData.ToString();


        //    Console.WriteLine("on time");

            try
            {
                DateTime dtStartTime = Convert.ToDateTime(configuration.GetValue<string>("StartTime"));
                DateTime dtnEndTime = Convert.ToDateTime(configuration.GetValue<string>("EndTime"));
                DateTime dtnNowTime = Convert.ToDateTime(execTime.ToString()); ;

                int nCount = 0;

                if (today == processDate) //Don't do it twice a day
                {
                   // Console.WriteLine("return Today == ProcessDate ::" + Today);
                    return;
                }
                while (DateTime.Compare(dtnNowTime, dtStartTime) >= 0 && DateTime.Compare(dtnNowTime, dtnEndTime) <= 0)
                {
                    dtnNowTime = Convert.ToDateTime((new DateTimeOffset(DateTime.Now).ToOffset(new TimeSpan(8, 0, 0)).ToString("HH:mm")).ToString());

                    //   Console.WriteLine("dtnNowTime: " + dtnNowTime + " ProcessDate:" + ProcessDate);
                    processDate = today;
                    var data = quaryTableBLL.GetTpData(configuration.GetValue<int>("LiminNum"));
                    foreach (var item in data)
                    {
                        insertTable14BLL.Archive14(item);

                      //  if (insertTable14BLL.InsterToTp14(item))
                      //    delOldItemFromTPBLL.DelOldItemFromTP(item);
                    }

                    nCount = data.Count();
                    if (nCount < 1000)
                    {
                        break;
                    }
                    Thread.Sleep(configuration.GetValue<int>("ColdDownTime") * 1000);//
                }


            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
            finally
            {
             //   Console.WriteLine("finally in");
                TimeSpan dueTimeSpan = TimeSpan.FromSeconds(configuration.GetValue<int>("CheckTime"));
                timer.Change(dueTimeSpan, TimeSpan.Zero);
            //    Console.WriteLine("finish new timer");
            }
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
          //  Console.WriteLine("StopAsync(CancellationToken cancellationToken)");
            //WriteToFile("WriteToFileHostedService: Process Stopped");
            timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}
