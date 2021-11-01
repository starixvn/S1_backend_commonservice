using ArchiveService.Service.BLL;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
            timerPPArchive = new Timer(DoPPArchiveTask, null, TimeSpan.Zero, TimeSpan.Zero);
            //timerResetUsage = new Timer(DoResetUsageTask, null, TimeSpan.Zero, TimeSpan.Zero);
            timerSGArchive = new Timer(DoSGArchiveTask, null, TimeSpan.Zero, TimeSpan.Zero);
            timerPGArchive = new Timer(DoPGArchiveTask, null, TimeSpan.Zero, TimeSpan.Zero);
            timerGPIArchive = new Timer(DoGPIArchiveTask, null, TimeSpan.Zero, TimeSpan.Zero);
            timerTpSBArchive = new Timer(DoTpSBArchiveTask, null, TimeSpan.Zero, TimeSpan.Zero);
            timerCQ9Archive = new Timer(DoCQ9ArchiveTask, null, TimeSpan.Zero, TimeSpan.Zero);
            return Task.CompletedTask;
        }

        private void DoTPArchiveTask(object state)
        {
            var beginAt = GetUTC8Time();
            var today = beginAt.Date.ToString(CultureInfo.InvariantCulture);

            try
            {
                if (DateTime.TryParse($"{beginAt:yyyy/MM/dd} {configuration.GetValue<string>("TPArchiveCFG:StartTime")}", out var startTime) &&
                    DateTime.TryParse($"{beginAt:yyyy/MM/dd} {configuration.GetValue<string>("TPArchiveCFG:EndTime")}", out var endTime))
                {
                    logger.LogInformation($"TP - {startTime} - {endTime} {beginAt}");
                    
                    var delete = configuration.GetValue<bool>("TPArchiveCFG:Delete");
                    var limit = configuration.GetValue<int>("TPArchiveCFG:LiminNum");
                    var taskCount = configuration.GetValue<int>("TPArchiveCFG:TaskCount");

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
                                if (item.DoDelete)
                                {
                                    tasks.Add(Task.Run(() =>
                                    {
                                        try
                                        {
                                            insertTable14BLL.ArchiveTP14(item, delete);
                                        }
                                        catch (Exception ex)
                                        {
                                            logger.LogError(ex, GetCurrentMethod());
                                        }
                                    }));

                                    if (tasks.Count >= taskCount)
                                    {
                                        Task.WhenAll(tasks).Wait();
                                        tasks.Clear();
                                    }
                                }
                                else
                                {
                                    processPPDate = today;
                                    break;
                                }
                            }

                            if (tasks.Count > 0)
                            {
                                Task.WhenAll(tasks).Wait();
                                tasks.Clear();
                            }

                            logger.LogInformation($"{GetCurrentMethod()} {sw.Elapsed:c}");
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
                logger.LogError(ex, GetCurrentMethod());
            }
            finally
            {
                var dueTimeSpan = TimeSpan.FromSeconds(configuration.GetValue<int>("TPArchiveCFG:CheckTime"));
                timerTPArchive.Change(dueTimeSpan, TimeSpan.Zero);
            }
        }
        // private void DoDGArchiveTask(object state)
        // {
        //     var beginAt = GetUTC8Time();
        //     var today = beginAt.Date.ToString(CultureInfo.InvariantCulture);
        //
        //     try
        //     {
        //         if (DateTime.TryParse($"{beginAt:yyyy/MM/dd} {configuration.GetValue<string>("DGArchiveCFG:StartTime")}", out var startTime) &&
        //             DateTime.TryParse($"{beginAt:yyyy/MM/dd} {configuration.GetValue<string>("DGArchiveCFG:EndTime")}", out var endTime))
        //         {
        //
        //             logger.LogInformation($"{startTime} - {endTime} {beginAt}");
        //
        //             if (endTime < startTime)
        //             {
        //                 endTime = endTime.AddDays(1);
        //             }
        //
        //             var nCount = 0;
        //
        //             if (today != processDGDate) //Don't do it again.
        //             {
        //                 while (GetUTC8Time() >= startTime && GetUTC8Time() <= endTime)
        //                 {
        //                     var data = queryTableBLL.GetDGData(configuration.GetValue<int>("DGArchiveCFG:LiminNum")).ToList();
        //                     foreach (var item in data)
        //                     {
        //                         if (item.DoDelete)
        //                         {
        //                             insertTable14BLL.ArchiveDG14(item);
        //                         }
        //                         else
        //                         {
        //                             processDGDate = today;
        //                             break;
        //                         }
        //                     }
        //
        //                     nCount = data.Count;
        //                     if (nCount < configuration.GetValue<int>("DGArchiveCFG:LiminNum"))
        //                     {
        //                         processDGDate = today;
        //                         break;
        //                     }
        //
        //                     Thread.Sleep(configuration.GetValue<int>("DGArchiveCFG:ColdDownTime") * 1000);
        //                 }
        //             }
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         logger.LogError(ex, GetCurrentMethod());
        //     }
        //     finally
        //     {
        //         var dueTimeSpan = TimeSpan.FromSeconds(configuration.GetValue<int>("DGArchiveCFG:CheckTime"));
        //         timerDGArchive.Change(dueTimeSpan, TimeSpan.Zero);
        //     }
        // }
        private void DoPPArchiveTask(object state)
        {
            var beginAt = GetUTC8Time();
            var today = beginAt.Date.ToString(CultureInfo.InvariantCulture);

            try
            {
                if (DateTime.TryParse($"{beginAt:yyyy/MM/dd} {configuration.GetValue<string>("PPArchiveCFG:StartTime")}", out var startTime) &&
                    DateTime.TryParse($"{beginAt:yyyy/MM/dd} {configuration.GetValue<string>("PPArchiveCFG:EndTime")}", out var endTime))
                {
                    logger.LogInformation($"PP - {startTime} - {endTime} {beginAt}");
                    
                    var delete = configuration.GetValue<bool>("PPArchiveCFG:Delete");
                    var limit = configuration.GetValue<int>("PPArchiveCFG:LiminNum");
                    var taskCount = configuration.GetValue<int>("PPArchiveCFG:TaskCount");

                    if (endTime < startTime)
                    {
                        endTime = endTime.AddDays(1);
                    }

                    if (today != processPPDate) //Don't do it again.
                    {
                        var allTasks = new List<Task>();
                        while (GetUTC8Time() >= startTime && GetUTC8Time() <= endTime)
                        {
                            var sw = Stopwatch.StartNew();
                            var data = queryTableBLL.GetPPData(limit).ToList();
                            foreach (var item in data)
                            {
                                if (item.DoDelete)
                                {
                                    allTasks.Add(Task.Run(() => insertTable14BLL.ArchivePP14(item, delete)));
                                    if (allTasks.Count >= taskCount)
                                    {
                                        Task.WhenAll(allTasks).Wait();
                                        allTasks.Clear();
                                    }
                                }
                                else
                                {
                                    processPPDate = today;
                                    break;
                                }
                            }

                            if (allTasks.Count > 0)
                            {
                                Task.WhenAll(allTasks).Wait();
                                allTasks.Clear();
                            }

                            logger.LogInformation($"{GetCurrentMethod()} {sw.Elapsed:c}");
                            if (data.Count < limit)
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
                logger.LogError(ex, GetCurrentMethod());
            }
            finally
            {
                var dueTimeSpan = TimeSpan.FromSeconds(configuration.GetValue<int>("PPArchiveCFG:CheckTime"));
                timerPPArchive.Change(dueTimeSpan, TimeSpan.Zero);
            }
        }
        private void DoSGArchiveTask(object state)
        {
            var beginAt = GetUTC8Time();
            var today = beginAt.Date.ToString(CultureInfo.InvariantCulture);

            try
            {
                if (DateTime.TryParse($"{beginAt:yyyy/MM/dd} {configuration.GetValue<string>("SGArchiveCFG:StartTime")}", out var startTime) &&
                    DateTime.TryParse($"{beginAt:yyyy/MM/dd} {configuration.GetValue<string>("SGArchiveCFG:EndTime")}", out var endTime))
                {
                    logger.LogInformation($"SG - {startTime} - {endTime} {beginAt}");
                    
                    var delete = configuration.GetValue<bool>("SGArchiveCFG:Delete");
                    var limit = configuration.GetValue<int>("SGArchiveCFG:LiminNum");
                    var taskCount = configuration.GetValue<int>("SGArchiveCFG:TaskCount");

                    if (endTime < startTime)
                    {
                        endTime = endTime.AddDays(1);
                    }

                    if (today != processSGDate) //Don't do it again.
                    {
                        var allTasks = new List<Task>();

                        while (GetUTC8Time() >= startTime && GetUTC8Time() <= endTime)
                        {
                            var sw = Stopwatch.StartNew();
                            var data = queryTableBLL.GetSGData(limit).ToList();
                            foreach (var item in data)
                            {
                                if (item.DoDelete)
                                {
                                    allTasks.Add(Task.Run(() => insertTable14BLL.ArchiveSG14(item, delete)));
                                    if (allTasks.Count >= taskCount)
                                    {
                                        Task.WhenAll(allTasks).Wait();
                                        allTasks.Clear();
                                    }
                                }
                                else
                                {
                                    processSGDate = today;
                                    break;
                                }
                            }

                            if (allTasks.Count > 0)
                            {
                                Task.WhenAll(allTasks).Wait();
                                allTasks.Clear();
                            }

                            logger.LogInformation($"{GetCurrentMethod()} {sw.Elapsed:c}");
                            if (data.Count < limit)
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
                logger.LogError(ex, GetCurrentMethod());
            }
            finally
            {
                var dueTimeSpan = TimeSpan.FromSeconds(configuration.GetValue<int>("SGArchiveCFG:CheckTime"));
                timerSGArchive.Change(dueTimeSpan, TimeSpan.Zero);
            }
        }
        private void DoPGArchiveTask(object state)
        {
            var beginAt = GetUTC8Time();
            var today = beginAt.Date.ToString(CultureInfo.InvariantCulture);

            try
            {
                if (DateTime.TryParse($"{beginAt:yyyy/MM/dd} {configuration.GetValue<string>("PGArchiveCFG:StartTime")}", out var startTime) &&
                    DateTime.TryParse($"{beginAt:yyyy/MM/dd} {configuration.GetValue<string>("PGArchiveCFG:EndTime")}", out var endTime))
                {
                    logger.LogInformation($"PG - {startTime} - {endTime} {beginAt}");
                    
                    var delete = configuration.GetValue<bool>("PGArchiveCFG:Delete");
                    var limit = configuration.GetValue<int>("PGArchiveCFG:LiminNum");
                    var taskCount = configuration.GetValue<int>("PGArchiveCFG:TaskCount");

                    if (endTime < startTime)
                    {
                        endTime = endTime.AddDays(1);
                    }

                    if (today != processPGDate) //Don't do it again.
                    {
                        
                        while (GetUTC8Time() >= startTime && GetUTC8Time() <= endTime)
                        {
                            var sw = Stopwatch.StartNew();
                            var data = queryTableBLL.GetPGData(limit).ToList();

                            Parallel.ForEach(data, item =>
                            { 
                               insertTable14BLL.ArchivePG14(item, delete);
                            });

                            logger.LogInformation($"{GetCurrentMethod()} {sw.Elapsed:c}");
                            if (data.Count < limit)
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
                logger.LogError(ex, GetCurrentMethod());
            }
            finally
            {
                var dueTimeSpan = TimeSpan.FromSeconds(configuration.GetValue<int>("PGArchiveCFG:CheckTime"));
                timerPGArchive.Change(dueTimeSpan, TimeSpan.Zero);
            }
        }
        private void DoGPIArchiveTask(object state)
        {
            var beginAt = GetUTC8Time();
            var today = beginAt.Date.ToString(CultureInfo.InvariantCulture);

            try
            {
                if (DateTime.TryParse($"{beginAt:yyyy/MM/dd} {configuration.GetValue<string>("GPIArchiveCFG:StartTime")}", out var startTime) &&
                    DateTime.TryParse($"{beginAt:yyyy/MM/dd} {configuration.GetValue<string>("GPIArchiveCFG:EndTime")}", out var endTime))
                {
                    logger.LogInformation($"GPI - {startTime} - {endTime} {beginAt}");
                    
                    var delete = configuration.GetValue<bool>("GPIArchiveCFG:Delete");
                    var limit = configuration.GetValue<int>("GPIArchiveCFG:LiminNum");
                    var taskCount = configuration.GetValue<int>("GPIArchiveCFG:TaskCount");

                    if (endTime < startTime)
                    {
                        endTime = endTime.AddDays(1);
                    }

                    if (today != processGPIDate) //Don't do it again.
                    {
                        var allTasks = new List<Task>();
                        while (GetUTC8Time() >= startTime && GetUTC8Time() <= endTime)
                        {
                            var sw = Stopwatch.StartNew();
                            var data = queryTableBLL.GetGPIData(limit).ToList();
                            foreach (var item in data)
                            {
                                if (item.DoDelete)
                                {
                                    allTasks.Add(Task.Run(() => insertTable14BLL.ArchiveGPI14(item, delete)));
                                    if (allTasks.Count >= taskCount)
                                    {
                                        Task.WhenAll(allTasks).Wait();
                                        allTasks.Clear();
                                    }
                                }
                                else
                                {
                                    processGPIDate = today;
                                    break;
                                }
                            }

                            if (allTasks.Count > 0)
                            {
                                Task.WhenAll(allTasks).Wait();
                                allTasks.Clear();
                            }

                            logger.LogInformation($"{GetCurrentMethod()} {sw.Elapsed:c}");
                            if (data.Count < limit)
                            {
                                processGPIDate = today;
                                break;
                            }
                            
                            Thread.Sleep(configuration.GetValue<int>("GPIArchiveCFG:ColdDownTime") * 1000);
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
                var dueTimeSpan = TimeSpan.FromSeconds(configuration.GetValue<int>("GPIArchiveCFG:CheckTime"));
                timerGPIArchive.Change(dueTimeSpan, TimeSpan.Zero);
            }
        }

        private void DoTpSBArchiveTask(object state)
        {
            var beginAt = GetUTC8Time();
            var today = beginAt.Date.ToString(CultureInfo.InvariantCulture);

            try
            {
                if (DateTime.TryParse($"{beginAt:yyyy/MM/dd} {configuration.GetValue<string>("TpSBArchiveCFG:StartTime")}", out var startTime) &&
                    DateTime.TryParse($"{beginAt:yyyy/MM/dd} {configuration.GetValue<string>("TpSBArchiveCFG:EndTime")}", out var endTime))
                {
                    logger.LogInformation($"SB - {startTime} - {endTime} {beginAt}");
                    
                    var limit = configuration.GetValue<int>("TpSBArchiveCFG:LiminNum");
                    var taskCount = configuration.GetValue<int>("TpSBArchiveCFG:TaskCount");

                    if (endTime < startTime)
                    {
                        endTime = endTime.AddDays(1);
                    }

                    if (today != processTpSBDate) //Don't do it again.
                    {
                        var allTasks = new List<Task>();
                        while (GetUTC8Time() >= startTime && GetUTC8Time() <= endTime)
                        {
                            var sw = Stopwatch.StartNew();
                            var data = queryTableBLL.GetTpSlotsBonusData(limit).ToList();
                            foreach (var item in data)
                            {
                                if (item.DoDelete)
                                {
                                    allTasks.Add(Task.Run(() => delOldItemBLL.DelOldItemFromTpSlotsBonus(item)));
                                    if (allTasks.Count >= taskCount)
                                    {
                                        Task.WhenAll(allTasks).Wait();
                                        allTasks.Clear();
                                    }
                                }
                                else
                                {
                                    processTpSBDate = today;
                                    break;
                                }
                            }
                            
                            if (allTasks.Count > 0)
                            {
                                Task.WhenAll(allTasks).Wait();
                                allTasks.Clear();
                            }

                            logger.LogInformation($"{GetCurrentMethod()} {sw.Elapsed:c}");
                            if (data.Count < limit)
                            {
                                processTpSBDate = today;
                                break;
                            }

                            Thread.Sleep(configuration.GetValue<int>("TpSBArchiveCFG:ColdDownTime") * 1000);
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
                var dueTimeSpan = TimeSpan.FromSeconds(configuration.GetValue<int>("TpSBArchiveCFG:CheckTime"));
                timerTpSBArchive.Change(dueTimeSpan, TimeSpan.Zero);
            }
        }
        // private void DoResetUsageTask(object state)
        // {
        //     var beginAt = GetUTC8Time();
        //     var today = beginAt.Date.ToString(CultureInfo.InvariantCulture);
        //
        //     try
        //     {
        //         if (today != processResetUsageDate) //Don't do it again.
        //         {
        //             if (resetUsagedBLL.ResetUsage() == 1)
        //                 processResetUsageDate = today;
        //         }
        //
        //     }
        //     catch (Exception ex)
        //     {
        //         logger.LogError(ex, GetCurrentMethod());
        //     }
        //     finally
        //     {
        //         var dueTimeSpan = TimeSpan.FromSeconds(configuration.GetValue<int>("ResetUsaged:CheckTime"));
        //         timerResetUsage.Change(dueTimeSpan, TimeSpan.Zero);
        //     }
        // }
        private void DoCQ9ArchiveTask(object state)
        {
            var beginAt = GetUTC8Time();
            var today = beginAt.Date.ToString(CultureInfo.InvariantCulture);

            try
            {
                if (DateTime.TryParse($"{beginAt:yyyy/MM/dd} {configuration.GetValue<string>("CQ9ArchiveCFG:StartTime")}", out var startTime) &&
                    DateTime.TryParse($"{beginAt:yyyy/MM/dd} {configuration.GetValue<string>("CQ9ArchiveCFG:EndTime")}", out var endTime))
                {
                    logger.LogInformation($"CQ9 - {startTime} - {endTime} {beginAt}");
                    
                    var delete = configuration.GetValue<bool>("CQ9ArchiveCFG:Delete");
                    var limit = configuration.GetValue<int>("CQ9ArchiveCFG:LiminNum");
                    var taskCount = configuration.GetValue<int>("CQ9ArchiveCFG:TaskCount");

                    if (endTime < startTime)
                    {
                        endTime = endTime.AddDays(1);
                    }

                    if (today != processCQ9Date) //Don't do it again.
                    {
                        var allTasks = new List<Task>();
                        while (GetUTC8Time() >= startTime && GetUTC8Time() <= endTime)
                        {
                            var sw = Stopwatch.StartNew();
                            var data = queryTableBLL.GetCQ9Data(limit).ToList();
                            foreach (var item in data)
                            {
                                if (item.DoDelete)
                                {
                                    allTasks.Add(Task.Run(() => insertTable14BLL.ArchiveCP914(item, delete)));
                                    if (allTasks.Count >= taskCount)
                                    {
                                        Task.WhenAll(allTasks).Wait();
                                        allTasks.Clear();
                                    }
                                }
                                else
                                {
                                    processCQ9Date = today;
                                    break;
                                }
                            }

                            if (allTasks.Count > 0)
                            {
                                Task.WhenAll(allTasks).Wait();
                                allTasks.Clear();
                            }

                            logger.LogInformation($"{GetCurrentMethod()} {sw.Elapsed:c}");
                            if (data.Count < limit)
                            {
                                processCQ9Date = today;
                                break;
                            }
                            
                            Thread.Sleep(configuration.GetValue<int>("CQ9ArchiveCFG:ColdDownTime") * 1000);
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
                var dueTimeSpan = TimeSpan.FromSeconds(configuration.GetValue<int>("CQ9ArchiveCFG:CheckTime"));
                timerCQ9Archive.Change(dueTimeSpan, TimeSpan.Zero);
            }
        }
        private DateTime GetUTC8Time()
        {
            return new DateTimeOffset(DateTime.Now).ToOffset(new TimeSpan(8, 0, 0)).DateTime;
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            //timerResetUsage?.Change(Timeout.Infinite, 0);
            timerTPArchive?.Change(Timeout.Infinite, 0);
            timerPPArchive?.Change(Timeout.Infinite, 0);
            // timerDGArchive?.Change(Timeout.Infinite, 0);
            timerSGArchive?.Change(Timeout.Infinite, 0);
            timerPGArchive?.Change(Timeout.Infinite, 0);
            timerGPIArchive?.Change(Timeout.Infinite, 0);
            timerTpSBArchive?.Change(Timeout.Infinite, 0);
            timerCQ9Archive?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}
