using ArchiveService.Service.BLL;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using static ArchiveService.Extensions.MethodExtension;

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
        private Timer timerSGArchive;
        private Timer timerPGArchive;
        private Timer timerGPIArchive;
        private Timer timerCQ9Archive;
        private Timer timerTpSBArchive;
        private Timer timerResetUsage;
        private readonly IConfiguration configuration;
        private readonly DelOldItemBLL delOldItemBLL;
        private string processTPDate = "";
        private string processDGDate = "";
        private string processPPDate = "";
        private string processSGDate = "";
        private string processPGDate = "";
        private string processGPIDate = "";
        private string processCQ9Date = "";
        private string processTpSBDate = "";
        private string processResetUsageDate = "";

        public HistoryWorker(IServiceProvider provider)
        {
            logger = provider.GetRequiredService<ILogger<HistoryWorker>>();
            queryTableBLL = provider.GetRequiredService<QueryTableBLL>();
            insertTable14BLL = provider.GetRequiredService<InsertTable14BLL>();
            resetUsagedBLL = provider.GetRequiredService<ResetUsagedBLL>();
            configuration = provider.GetRequiredService<IConfiguration>();
            delOldItemBLL = provider.GetRequiredService<DelOldItemBLL>();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            timerTPArchive = new Timer(DoTPArchiveTask, null, TimeSpan.Zero, TimeSpan.Zero);
            //timerDGArchive = new Timer(DoDGArchiveTask, null, TimeSpan.Zero, TimeSpan.Zero);
            timerPPArchive = new Timer(DoPPArchiveTaskAsync, null, TimeSpan.Zero, TimeSpan.Zero);
            //timerResetUsage = new Timer(DoResetUsageTask, null, TimeSpan.Zero, TimeSpan.Zero);
            timerSGArchive = new Timer(DoSGArchiveTaskAsync, null, TimeSpan.Zero, TimeSpan.Zero);
            timerPGArchive = new Timer(DoPGArchiveTask, null, TimeSpan.Zero, TimeSpan.Zero);
            timerGPIArchive = new Timer(DoGPIArchiveTaskAsync, null, TimeSpan.Zero, TimeSpan.Zero);
            timerTpSBArchive = new Timer(DoTpSBArchiveTaskAsync, null, TimeSpan.Zero, TimeSpan.Zero);
            timerCQ9Archive = new Timer(DoCQ9ArchiveTaskAsync, null, TimeSpan.Zero, TimeSpan.Zero);
            return Task.CompletedTask;
        }

        private async void DoTPArchiveTask(object state)
        {
            var beginAt = GetUTC8Time();
            var today = beginAt.Date.ToString();

            try
            {
                if (DateTime.TryParse($"{beginAt:yyyy/MM/dd} {configuration.GetValue<string>("TPArchiveCFG:StartTime")}", out var startTime) &&
                    DateTime.TryParse($"{beginAt:yyyy/MM/dd} {configuration.GetValue<string>("TPArchiveCFG:EndTime")}", out var endTime))
                {
                    var delete = configuration.GetValue<bool>("TPArchiveCFG:Delete");
                    var limit = configuration.GetValue<int>("TPArchiveCFG:LiminNum");

                    if (endTime < startTime)
                    {
                        endTime = endTime.AddDays(1);
                    }

                    if (today != processTPDate) //Don't do it again.
                    {
                        var tasks = new List<Task>();
                        while (GetUTC8Time() >= startTime && GetUTC8Time() <= endTime)
                        {
                            var sw = Stopwatch.StartNew();
                            var data = queryTableBLL.GetTpData(limit).ToList();
                            foreach (var item in data)
                            {
                                if (!item.DoDelete)
                                {
                                    processTPDate = today;
                                    break;
                                }

                                tasks.Add(Task.Run(() =>
                                {
                                    try
                                    {
                                        insertTable14BLL.ArchiveTP14(item, delete);
                                    }
                                    catch (Exception ex)
                                    {
                                        logger.LogError(ex, GetCurrentAsyncMethod());
                                    }
                                }));

                                if (tasks.Count >= 4)
                                {
                                    await Task.WhenAll(tasks);
                                    tasks.Clear();
                                }
                            }

                            if (tasks.Count > 0)
                            {
                                await Task.WhenAll(tasks);
                                tasks.Clear();
                            }

                            logger.LogInformation($"DoTPArchiveTask {sw.Elapsed:c}");
                            if (data.Count < limit)
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
                logger.LogError(ex, GetCurrentAsyncMethod());
            }
            finally
            {
                //   Console.WriteLine("finally in");
                var dueTimeSpan = TimeSpan.FromSeconds(configuration.GetValue<int>("TPArchiveCFG:CheckTime"));
                timerTPArchive.Change(dueTimeSpan, TimeSpan.Zero);
                //    Console.WriteLine("finish new timer");
            }
        }
        private void DoDGArchiveTask(object state)
        {
            // Console.WriteLine("DoTask start");
            var beginAt = GetUTC8Time();
            var today = beginAt.Date.ToString();

            try
            {
                if (DateTime.TryParse($"{beginAt:yyyy/MM/dd} {configuration.GetValue<string>("DGArchiveCFG:StartTime")}", out var startTime) &&
                    DateTime.TryParse($"{beginAt:yyyy/MM/dd} {configuration.GetValue<string>("DGArchiveCFG:EndTime")}", out var endTime))
                {

                    logger.LogInformation($"{startTime} - {endTime} {beginAt}");

                    if (endTime < startTime)
                    {
                        endTime = endTime.AddDays(1);
                    }

                    var nCount = 0;

                    if (today != processDGDate) //Don't do it again.
                    {
                        while (GetUTC8Time() >= startTime && GetUTC8Time() <= endTime)
                        {
                            //   Console.WriteLine("dtnNowTime: " + dtnNowTime + " ProcessDate:" + ProcessDate);
                            var data = queryTableBLL.GetDGData(configuration.GetValue<int>("DGArchiveCFG:LiminNum")).ToList();
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

                            nCount = data.Count;
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
                logger.LogError(ex, GetCurrentMethod());
            }
            finally
            {
                //   Console.WriteLine("finally in");
                var dueTimeSpan = TimeSpan.FromSeconds(configuration.GetValue<int>("DGArchiveCFG:CheckTime"));
                timerDGArchive.Change(dueTimeSpan, TimeSpan.Zero);
                //    Console.WriteLine("finish new timer");
            }
        }
        private async void DoPPArchiveTaskAsync(object state)
        {
            // Console.WriteLine("DoTask start");
            var beginAt = GetUTC8Time();
            var today = beginAt.Date.ToString();

            try
            {
                if (DateTime.TryParse($"{beginAt:yyyy/MM/dd} {configuration.GetValue<string>("PPArchiveCFG:StartTime")}", out var startTime) &&
                    DateTime.TryParse($"{beginAt:yyyy/MM/dd} {configuration.GetValue<string>("PPArchiveCFG:EndTime")}", out var endTime))
                {

                    logger.LogInformation($"{startTime} - {endTime} {beginAt}");

                    if (endTime < startTime)
                    {
                        endTime = endTime.AddDays(1);
                    }

                    var nCount = 0;

                    if (today != processPPDate) //Don't do it again.
                    {
                        var allTasks = new List<Task>();
                        while (GetUTC8Time() >= startTime && GetUTC8Time() <= endTime)
                        {
                            //   Console.WriteLine("dtnNowTime: " + dtnNowTime + " ProcessDate:" + ProcessDate);
                            var data = queryTableBLL.GetPPData(configuration.GetValue<int>("PPArchiveCFG:LiminNum"));
                            // Console.WriteLine("queryPP");
                            foreach (var item in data)
                            {
                                if (item.DoDelete)
                                {
                                    allTasks.Add(Task.Run(() => insertTable14BLL.ArchivePP14(item)));
                                    if (allTasks.Count >= 4)
                                    {
                                        await Task.WhenAll(allTasks);
                                        allTasks.Clear();
                                    }
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
                            //if (allTasks.Count > 0)
                            //{
                            //    await Task.WhenAll(allTasks);
                            //    allTasks.Clear();
                            //}
                            //Thread.Sleep(configuration.GetValue<int>("PPArchiveCFG:ColdDownTime") * 1000);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, GetCurrentAsyncMethod());
            }
            finally
            {
                //   Console.WriteLine("finally in");
                var dueTimeSpan = TimeSpan.FromSeconds(configuration.GetValue<int>("PPArchiveCFG:CheckTime"));
                timerPPArchive.Change(dueTimeSpan, TimeSpan.Zero);
                //    Console.WriteLine("finish new timer");
            }
        }
        private async void DoSGArchiveTaskAsync(object state)
        {
            // Console.WriteLine("DoTask start");
            var beginAt = GetUTC8Time();
            var today = beginAt.Date.ToString();

            try
            {
                if (DateTime.TryParse($"{beginAt:yyyy/MM/dd} {configuration.GetValue<string>("SGArchiveCFG:StartTime")}", out var startTime) &&
                    DateTime.TryParse($"{beginAt:yyyy/MM/dd} {configuration.GetValue<string>("SGArchiveCFG:EndTime")}", out var endTime))
                {

                    logger.LogInformation($"{startTime} - {endTime} {beginAt}");

                    if (endTime < startTime)
                    {
                        endTime = endTime.AddDays(1);
                    }

                    var nCount = 0;

                    if (today != processSGDate) //Don't do it again.
                    {
                        var allTasks = new List<Task>();

                        while (GetUTC8Time() >= startTime && GetUTC8Time() <= endTime)
                        {
                            //   Console.WriteLine("dtnNowTime: " + dtnNowTime + " ProcessDate:" + ProcessDate);
                            var data = queryTableBLL.GetSGData(configuration.GetValue<int>("SGArchiveCFG:LiminNum"));
                            // Console.WriteLine("querySG");
                            foreach (var item in data)
                            {
                                if (item.DoDelete)
                                {
                                    allTasks.Add(Task.Run(() => insertTable14BLL.ArchiveSG14(item)));
                                    if (allTasks.Count >= 4)
                                    {
                                        await Task.WhenAll(allTasks);
                                        allTasks.Clear();
                                    }
                                }
                                else
                                {
                                    processSGDate = today;
                                    break;
                                }
                            }

                            nCount = data.Count();
                            if (nCount < configuration.GetValue<int>("SGArchiveCFG:LiminNum"))
                            {
                                processSGDate = today;
                                break;
                            }

                            Thread.Sleep(configuration.GetValue<int>("SGArchiveCFG:ColdDownTime") * 1000);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, GetCurrentAsyncMethod());
            }
            finally
            {
                //   Console.WriteLine("finally in");
                var dueTimeSpan = TimeSpan.FromSeconds(configuration.GetValue<int>("SGArchiveCFG:CheckTime"));
                timerSGArchive.Change(dueTimeSpan, TimeSpan.Zero);
                //    Console.WriteLine("finish new timer");
            }
        }
        private async void DoPGArchiveTask(object state)
        {
            // Console.WriteLine("DoTask start");
            var beginAt = GetUTC8Time();
            var today = beginAt.Date.ToString();

            try
            {
                if (DateTime.TryParse($"{beginAt:yyyy/MM/dd} {configuration.GetValue<string>("PGArchiveCFG:StartTime")}", out var startTime) &&
                    DateTime.TryParse($"{beginAt:yyyy/MM/dd} {configuration.GetValue<string>("PGArchiveCFG:EndTime")}", out var endTime))
                {

                    logger.LogInformation($"{startTime} - {endTime} {beginAt}");

                    if (endTime < startTime)
                    {
                        endTime = endTime.AddDays(1);
                    }

                    var nCount = 0;

                    if (today != processPGDate) //Don't do it again.
                    {
                        var allTasks = new List<Task>();
                        while (GetUTC8Time() >= startTime && GetUTC8Time() <= endTime)
                        {
                            //   Console.WriteLine("dtnNowTime: " + dtnNowTime + " ProcessDate:" + ProcessDate);
                            var data = queryTableBLL.GetPGData(configuration.GetValue<int>("PGArchiveCFG:LiminNum"));
                            // Console.WriteLine("queryPG");
                            foreach (var item in data)
                            {
                                if (item.DoDelete)
                                {
                                    allTasks.Add(Task.Run(() => insertTable14BLL.ArchivePG14(item)));
                                    if (allTasks.Count >= 4)
                                    {
                                        await Task.WhenAll(allTasks);
                                        allTasks.Clear();
                                    }
                                }
                                else
                                {
                                    processPGDate = today;
                                    break;
                                }
                            }

                            nCount = data.Count();
                            if (nCount < configuration.GetValue<int>("PGArchiveCFG:LiminNum"))
                            {
                                processPGDate = today;
                                break;
                            }

                            Thread.Sleep(configuration.GetValue<int>("PGArchiveCFG:ColdDownTime") * 1000);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, GetCurrentAsyncMethod());
            }
            finally
            {
                //   Console.WriteLine("finally in");
                var dueTimeSpan = TimeSpan.FromSeconds(configuration.GetValue<int>("PGArchiveCFG:CheckTime"));
                timerPGArchive.Change(dueTimeSpan, TimeSpan.Zero);
                //    Console.WriteLine("finish new timer");
            }
        }
        private async void DoGPIArchiveTaskAsync(object state)
        {
            // Console.WriteLine("DoTask start");
            var beginAt = GetUTC8Time();
            var today = beginAt.Date.ToString();

            try
            {
                if (DateTime.TryParse($"{beginAt:yyyy/MM/dd} {configuration.GetValue<string>("GPIArchiveCFG:StartTime")}", out var startTime) &&
                    DateTime.TryParse($"{beginAt:yyyy/MM/dd} {configuration.GetValue<string>("GPIArchiveCFG:EndTime")}", out var endTime))
                {

                    logger.LogInformation($"{startTime} - {endTime} {beginAt}");

                    if (endTime < startTime)
                    {
                        endTime = endTime.AddDays(1);
                    }

                    var nCount = 0;

                    if (today != processGPIDate) //Don't do it again.
                    {
                        var allTasks = new List<Task>();
                        while (GetUTC8Time() >= startTime && GetUTC8Time() <= endTime)
                        {
                            //   Console.WriteLine("dtnNowTime: " + dtnNowTime + " ProcessDate:" + ProcessDate);
                            var data = queryTableBLL.GetGPIData(configuration.GetValue<int>("GPIArchiveCFG:LiminNum"));
                            // Console.WriteLine("queryGPI");
                            foreach (var item in data)
                            {
                                if (item.DoDelete)
                                {
                                    allTasks.Add(Task.Run(() => insertTable14BLL.ArchiveGPI14(item)));
                                    if (allTasks.Count >= 4)
                                    {
                                        await Task.WhenAll(allTasks);
                                        allTasks.Clear();
                                    }
                                }
                                else
                                {
                                    processGPIDate = today;
                                    break;
                                }
                            }

                            nCount = data.Count();
                            if (nCount < configuration.GetValue<int>("GPIArchiveCFG:LiminNum"))
                            {
                                processGPIDate = today;
                                break;
                            }
                            //if (allTasks.Count > 0)
                            //{
                            //    await Task.WhenAll(allTasks);
                            //    allTasks.Clear();
                            //}
                            //Thread.Sleep(configuration.GetValue<int>("GPIArchiveCFG:ColdDownTime") * 1000);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, GetCurrentAsyncMethod());
            }
            finally
            {
                //   Console.WriteLine("finally in");
                var dueTimeSpan = TimeSpan.FromSeconds(configuration.GetValue<int>("GPIArchiveCFG:CheckTime"));
                timerGPIArchive.Change(dueTimeSpan, TimeSpan.Zero);
                //    Console.WriteLine("finish new timer");
            }
        }

        private async void DoTpSBArchiveTaskAsync(object state)
        {
            // Console.WriteLine("DoTask start");
            var beginAt = GetUTC8Time();
            var today = beginAt.Date.ToString();

            try
            {
                if (DateTime.TryParse($"{beginAt:yyyy/MM/dd} {configuration.GetValue<string>("TpSBArchiveCFG:StartTime")}", out var startTime) &&
                    DateTime.TryParse($"{beginAt:yyyy/MM/dd} {configuration.GetValue<string>("TpSBArchiveCFG:EndTime")}", out var endTime))
                {

                    logger.LogInformation($"{startTime} - {endTime} {beginAt}");

                    if (endTime < startTime)
                    {
                        endTime = endTime.AddDays(1);
                    }

                    var nCount = 0;

                    if (today != processTpSBDate) //Don't do it again.
                    {
                        var allTasks = new List<Task>();
                        while (GetUTC8Time() >= startTime && GetUTC8Time() <= endTime)
                        {
                            //   Console.WriteLine("dtnNowTime: " + dtnNowTime + " ProcessDate:" + ProcessDate);
                            var data = queryTableBLL.GetTpSlotsBonusData(configuration.GetValue<int>("TpSBArchiveCFG:LiminNum"));
                            // Console.WriteLine("queryGPI");
                            foreach (var item in data)
                            {
                                if (item.DoDelete)
                                {
                                    allTasks.Add(Task.Run(() => delOldItemBLL.DelOldItemFromTpSlotsBonus(item)));
                                    if (allTasks.Count >= 4)
                                    {
                                        await Task.WhenAll(allTasks);
                                        allTasks.Clear();
                                    }
                                }
                                else
                                {
                                    processTpSBDate = today;
                                    break;
                                }
                            }

                            nCount = data.Count();
                            if (nCount < configuration.GetValue<int>("TpSBArchiveCFG:LiminNum"))
                            {
                                processTpSBDate = today;
                                break;
                            }
                            //if (allTasks.Count > 0)
                            //{
                            //    await Task.WhenAll(allTasks);
                            //    allTasks.Clear();
                            //}
                            //Thread.Sleep(configuration.GetValue<int>("GPIArchiveCFG:ColdDownTime") * 1000);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, GetCurrentAsyncMethod());
            }
            finally
            {
                //   Console.WriteLine("finally in");
                var dueTimeSpan = TimeSpan.FromSeconds(configuration.GetValue<int>("TpSBArchiveCFG:CheckTime"));
                timerTpSBArchive.Change(dueTimeSpan, TimeSpan.Zero);
                //    Console.WriteLine("finish new timer");
            }
        }
        private void DoResetUsageTask(object state)
        {
            var beginAt = GetUTC8Time();
            var today = beginAt.Date.ToString();

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
                logger.LogError(ex, GetCurrentMethod());
            }
            finally
            {
                //   Console.WriteLine("finally in");
                var dueTimeSpan = TimeSpan.FromSeconds(configuration.GetValue<int>("ResetUsaged:CheckTime"));
                timerResetUsage.Change(dueTimeSpan, TimeSpan.Zero);
                //    Console.WriteLine("finish new timer");
            }
        }
        private async void DoCQ9ArchiveTaskAsync(object state)
        {
            // Console.WriteLine("DoTask start");
            var beginAt = GetUTC8Time();
            var today = beginAt.Date.ToString();

            try
            {
                if (DateTime.TryParse($"{beginAt:yyyy/MM/dd} {configuration.GetValue<string>("CQ9ArchiveCFG:StartTime")}", out var startTime) &&
                    DateTime.TryParse($"{beginAt:yyyy/MM/dd} {configuration.GetValue<string>("CQ9ArchiveCFG:EndTime")}", out var endTime))
                {

                    logger.LogInformation($"{startTime} - {endTime} {beginAt}");

                    if (endTime < startTime)
                    {
                        endTime = endTime.AddDays(1);
                    }

                    var nCount = 0;

                    if (today != processCQ9Date) //Don't do it again.
                    {
                        var allTasks = new List<Task>();
                        while (GetUTC8Time() >= startTime && GetUTC8Time() <= endTime)
                        {
                            //   Console.WriteLine("dtnNowTime: " + dtnNowTime + " ProcessDate:" + ProcessDate);
                            var data = queryTableBLL.GetCQ9Data(configuration.GetValue<int>("CQ9ArchiveCFG:LiminNum"));
                            // Console.WriteLine("queryCQ9");
                            foreach (var item in data)
                            {
                                if (item.DoDelete)
                                {
                                    allTasks.Add(Task.Run(() => insertTable14BLL.ArchiveCP914(item)));
                                    if (allTasks.Count >= 4)
                                    {
                                        await Task.WhenAll(allTasks);
                                        allTasks.Clear();
                                    }
                                }
                                else
                                {
                                    processCQ9Date = today;
                                    break;
                                }
                            }

                            nCount = data.Count();
                            if (nCount < configuration.GetValue<int>("CQ9ArchiveCFG:LiminNum"))
                            {
                                processCQ9Date = today;
                                break;
                            }
                            //if (allTasks.Count > 0)
                            //{
                            //    await Task.WhenAll(allTasks);
                            //    allTasks.Clear();
                            //}
                            //Thread.Sleep(configuration.GetValue<int>("CQ9ArchiveCFG:ColdDownTime") * 1000);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, GetCurrentAsyncMethod());
            }
            finally
            {
                //   Console.WriteLine("finally in");
                var dueTimeSpan = TimeSpan.FromSeconds(configuration.GetValue<int>("CQ9ArchiveCFG:CheckTime"));
                timerCQ9Archive.Change(dueTimeSpan, TimeSpan.Zero);
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
            //timerResetUsage?.Change(Timeout.Infinite, 0);
            timerTPArchive?.Change(Timeout.Infinite, 0);
            timerPPArchive?.Change(Timeout.Infinite, 0);
            timerDGArchive?.Change(Timeout.Infinite, 0);
            timerSGArchive?.Change(Timeout.Infinite, 0);
            timerPGArchive?.Change(Timeout.Infinite, 0);
            timerGPIArchive?.Change(Timeout.Infinite, 0);
            timerTpSBArchive?.Change(Timeout.Infinite, 0);
            timerCQ9Archive?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}
