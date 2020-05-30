using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace iHealthCheck.Managers
{
    public class iQueueHealthCheckManager
    {
        private static PerformanceCounter _performanceCPU;
        private static PerformanceCounter _performanceRAM;

        static iQueueHealthCheckManager()
        {
            //try
            //{
            //    _performanceCPU = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            //    _performanceRAM = new PerformanceCounter("Memory", "Available MBytes");
            //}
            //catch (Exception e)
            //{

            //    throw;
            //}
        }

        private static  async Task<double> GetCpuUsageForProcess()
        {
            var startTime = DateTime.UtcNow;
            var startCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
            await Task.Delay(500);

            var endTime = DateTime.UtcNow;
            var endCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
            var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
            var totalMsPassed = (endTime - startTime).TotalMilliseconds;
            var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);
            return cpuUsageTotal * 100;
        }

        public static async Task<string> CheckMyHealth()
        {
            var cpuValue = await GetCpuUsageForProcess();

            var process = Process.GetCurrentProcess();
            var currentMemoryUsage = (process.WorkingSet64 / process.MaxWorkingSet.ToInt64()) * 100;

            return $"{cpuValue.ToString()} -- Memory : {currentMemoryUsage}";
        }
    }
}
