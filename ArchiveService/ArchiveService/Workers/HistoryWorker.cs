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
        private readonly QueryTableBLL queryTableBLL;
        private readonly InsertTable14BLL insertTable14BLL;
        private Timer timerTPArchive;
        private readonly IConfiguration configuration;
        private string processDate = "";

        public HistoryWorker(
            ILogger<HistoryWorker> logger,
            QueryTableBLL queryTableBLL,
            InsertTable14BLL insertTable14BLL,
            IConfiguration configuration
        )
        {
            this.logger = logger;
            this.queryTableBLL = queryTableBLL;
            this.insertTable14BLL = insertTable14BLL;
            this.configuration = configuration;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {

            timerTPArchive = new Timer(DoTask, null, TimeSpan.Zero, TimeSpan.Zero);
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

                    logger.LogInformation($"{startTime} - {endTime} {beginAt}");

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
                            var data = queryTableBLL.GetTpData(configuration.GetValue<int>("LiminNum"));
                            foreach (var item in data)
                            {
                                if (item.DoDelete)
                                {
                                    insertTable14BLL.ArchiveTP14(item);
                                }
                                else
                                {
                                    processDate = today;
                                    break;
                                }
                            }

                            nCount = data.Count();
                            if (nCount < configuration.GetValue<int>("LiminNum"))
                            {
                                processDate = today;
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
                timerTPArchive.Change(dueTimeSpan, TimeSpan.Zero);
                //    Console.WriteLine("finish new timer");
            }
        }
        private DateTime GetUTC8Time()
        {
            return new DateTimeOffset(DateTime.Now).ToOffset(new TimeSpan(8, 0, 0)).DateTime;
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            //  Console.WriteLine("StopAsync(CancellationToken cancellationToken)");
            //WriteToFile("WriteToFileHostedService: Process Stopped");
            timerTPArchive?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}
