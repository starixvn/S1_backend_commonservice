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
        private readonly ResetUsagedBLL resetUsagedBLL;
        private Timer timerTPArchive;
        private Timer timerPPArchive;
        private Timer timerDGArchive;
        private Timer timerResetUsage;
        private readonly IConfiguration configuration;
        private string processTPDate = "";
        private string processDGDate = "";
        private string processPPDate = "";
        private string processResetUsageDate = "";

        public HistoryWorker(
            ILogger<HistoryWorker> logger,
            QueryTableBLL queryTableBLL,
            InsertTable14BLL insertTable14BLL,
            ResetUsagedBLL resetUsagedBLL,
            IConfiguration configuration
        )
        {
            this.logger = logger;
            this.queryTableBLL = queryTableBLL;
            this.insertTable14BLL = insertTable14BLL;
            this.resetUsagedBLL = resetUsagedBLL;
            this.configuration = configuration;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {

            timerTPArchive = new Timer(DoTPArchiveTask, null, TimeSpan.Zero, TimeSpan.Zero);
            timerDGArchive = new Timer(DoDGArchiveTask, null, TimeSpan.Zero, TimeSpan.Zero);
            timerPPArchive = new Timer(DoPPArchiveTask, null, TimeSpan.Zero, TimeSpan.Zero);
            timerResetUsage = new Timer(DoResetUsageTask, null, TimeSpan.Zero, TimeSpan.Zero);


            return Task.CompletedTask;
        }
        private void DoTPArchiveTask(object state)
        {
            // Console.WriteLine("DoTask start");
            var beginAt = GetUTC8Time();
            string today = beginAt.Date.ToString();

            try
            {
                if (DateTime.TryParse($"{beginAt.ToString("yyyy/MM/dd")} {configuration.GetValue<string>("TPArchiveCFG:StartTime")}", out DateTime startTime) &&
                    DateTime.TryParse($"{beginAt.ToString("yyyy/MM/dd")} {configuration.GetValue<string>("TPArchiveCFG:EndTime")}", out DateTime endTime))
                {

                    logger.LogInformation($"{startTime} - {endTime} {beginAt}");

                    if (endTime < startTime)
                    {
                        endTime = endTime.AddDays(1);
                    }

                    int nCount = 0;

                    if (today != processTPDate) //Don't do it again.
                    {
                        while (GetUTC8Time() >= startTime && GetUTC8Time() <= endTime)
                        {
                            //   Console.WriteLine("dtnNowTime: " + dtnNowTime + " ProcessDate:" + ProcessDate);
                            var data = queryTableBLL.GetTpData(configuration.GetValue<int>("TPArchiveCFG:LiminNum"));
                          //  Console.WriteLine("queryTP");
                            foreach (var item in data)
                            {
                                if (item.DoDelete)
                                {
                                    insertTable14BLL.ArchiveTP14(item);
                                }
                                else
                                {
                                    processTPDate = today;
                                    break;
                                }
                            }

                            nCount = data.Count();
                            if (nCount < configuration.GetValue<int>("TPArchiveCFG:LiminNum"))
                            {
                                processTPDate = today;
                                break;
                            }

                            Thread.Sleep(configuration.GetValue<int>("TPArchiveCFG:ColdDownTime") * 1000);
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
                TimeSpan dueTimeSpan = TimeSpan.FromSeconds(configuration.GetValue<int>("TPArchiveCFG:CheckTime"));
                timerTPArchive.Change(dueTimeSpan, TimeSpan.Zero);
                //    Console.WriteLine("finish new timer");
            }
        }
        private void DoDGArchiveTask(object state)
        {
            // Console.WriteLine("DoTask start");
            var beginAt = GetUTC8Time();
            string today = beginAt.Date.ToString();

            try
            {
                if (DateTime.TryParse($"{beginAt.ToString("yyyy/MM/dd")} {configuration.GetValue<string>("DGArchiveCFG:StartTime")}", out DateTime startTime) &&
                    DateTime.TryParse($"{beginAt.ToString("yyyy/MM/dd")} {configuration.GetValue<string>("DGArchiveCFG:EndTime")}", out DateTime endTime))
                {

                    logger.LogInformation($"{startTime} - {endTime} {beginAt}");

                    if (endTime < startTime)
                    {
                        endTime = endTime.AddDays(1);
                    }

                    int nCount = 0;

                    if (today != processDGDate) //Don't do it again.
                    {
                        while (GetUTC8Time() >= startTime && GetUTC8Time() <= endTime)
                        {
                            //   Console.WriteLine("dtnNowTime: " + dtnNowTime + " ProcessDate:" + ProcessDate);
                            var data = queryTableBLL.GetDGData(configuration.GetValue<int>("DGArchiveCFG:LiminNum"));
                           // Console.WriteLine("queryDG");
                            foreach (var item in data)
                            {
                                if (item.DoDelete)
                                {
                                    insertTable14BLL.ArchiveDG14(item);
                                }
                                else
                                {
                                    processDGDate = today;
                                    break;
                                }
                            }

                            nCount = data.Count();
                            if (nCount < configuration.GetValue<int>("DGArchiveCFG:LiminNum"))
                            {
                                processDGDate = today;
                                break;
                            }

                            Thread.Sleep(configuration.GetValue<int>("DGArchiveCFG:ColdDownTime") * 1000);
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
                TimeSpan dueTimeSpan = TimeSpan.FromSeconds(configuration.GetValue<int>("DGArchiveCFG:CheckTime"));
                timerDGArchive.Change(dueTimeSpan, TimeSpan.Zero);
                //    Console.WriteLine("finish new timer");
            }
        }
        private void DoPPArchiveTask(object state)
        {
            // Console.WriteLine("DoTask start");
            var beginAt = GetUTC8Time();
            string today = beginAt.Date.ToString();

            try
            {
                if (DateTime.TryParse($"{beginAt.ToString("yyyy/MM/dd")} {configuration.GetValue<string>("PPArchiveCFG:StartTime")}", out DateTime startTime) &&
                    DateTime.TryParse($"{beginAt.ToString("yyyy/MM/dd")} {configuration.GetValue<string>("PPArchiveCFG:EndTime")}", out DateTime endTime))
                {

                    logger.LogInformation($"{startTime} - {endTime} {beginAt}");

                    if (endTime < startTime)
                    {
                        endTime = endTime.AddDays(1);
                    }

                    int nCount = 0;

                    if (today != processPPDate) //Don't do it again.
                    {
                        while (GetUTC8Time() >= startTime && GetUTC8Time() <= endTime)
                        {
                            //   Console.WriteLine("dtnNowTime: " + dtnNowTime + " ProcessDate:" + ProcessDate);
                            var data = queryTableBLL.GetPPData(configuration.GetValue<int>("PPArchiveCFG:LiminNum"));
                           // Console.WriteLine("queryPP");
                            foreach (var item in data)
                            {
                                if (item.DoDelete)
                                {
                                    insertTable14BLL.ArchivePP14(item);
                                }
                                else
                                {
                                    processPPDate = today;
                                    break;
                                }
                            }

                            nCount = data.Count();
                            if (nCount < configuration.GetValue<int>("PPArchiveCFG:LiminNum"))
                            {
                                processPPDate = today;
                                break;
                            }

                            Thread.Sleep(configuration.GetValue<int>("PPArchiveCFG:ColdDownTime") * 1000);
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
                TimeSpan dueTimeSpan = TimeSpan.FromSeconds(configuration.GetValue<int>("PPArchiveCFG:CheckTime"));
                timerPPArchive.Change(dueTimeSpan, TimeSpan.Zero);
                //    Console.WriteLine("finish new timer");
            }
        }
        private void DoResetUsageTask(object state)
        {
            var beginAt = GetUTC8Time();
            string today = beginAt.Date.ToString();

            try
            {
                if (today != processResetUsageDate) //Don't do it again.
                {
                    if (resetUsagedBLL.ResetUsage() == 1)
                        processResetUsageDate = today;
                }
                
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
            finally
            {
                //   Console.WriteLine("finally in");
                TimeSpan dueTimeSpan = TimeSpan.FromSeconds(configuration.GetValue<int>("ResetUsaged:CheckTime"));
                timerResetUsage.Change(dueTimeSpan, TimeSpan.Zero);
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
            timerPPArchive?.Change(Timeout.Infinite, 0);
            timerDGArchive?.Change(Timeout.Infinite, 0);
            timerResetUsage?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}
