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
        private readonly QuaryTableBLL quaryTableBLL;
        private readonly InsertTable14BLL insertTable14BLL;
        private Timer timer;
        private readonly IConfiguration configuration;
        private string processDate = "";

        public HistoryWorker(
            ILogger<HistoryWorker> logger,
            QuaryTableBLL quaryTableBLL,
            InsertTable14BLL insertTable14BLL,
            IConfiguration configuration
        )
        {
            this.logger = logger;
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
            // Console.WriteLine("DoTask start");
            var beginAt = GetUTC8Time();
            string today = beginAt.Date.ToString();

            try
            {
                if (DateTime.TryParse($"{beginAt.ToString("yyyy/MM/dd")} {configuration.GetValue<string>("StartTime")}", out DateTime startTime) &&
                    DateTime.TryParse($"{beginAt.ToString("yyyy/MM/dd")} {configuration.GetValue<string>("EndTime")}", out DateTime endTime))
                {

                    if (endTime < startTime)
                    {
                        endTime = endTime.AddDays(1);
                    }

                    int nCount = 0;

                    if (today != processDate) //Don't do it again.
                    {
                        while (GetUTC8Time() >= startTime && GetUTC8Time() <= endTime)
                        {
                            //   Console.WriteLine("dtnNowTime: " + dtnNowTime + " ProcessDate:" + ProcessDate);
                            processDate = today;
                            var data = quaryTableBLL.GetTpData(configuration.GetValue<int>("LiminNum"));
                            foreach (var item in data)
                            {
                                insertTable14BLL.Archive14(item);
                            }

                            nCount = data.Count();
                            if (nCount < 1000)
                            {
                                break;
                            }

                            Thread.Sleep(configuration.GetValue<int>("ColdDownTime") * 1000);
                        }
                    }
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
        private DateTime GetUTC8Time()
        {
            return new DateTimeOffset(DateTime.Now).ToOffset(new TimeSpan(8, 0, 0)).LocalDateTime;
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
