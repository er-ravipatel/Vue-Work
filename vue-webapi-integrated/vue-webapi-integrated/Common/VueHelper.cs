using Microsoft.AspNetCore.SpaServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vue_webapi_integrated.Common
{
    public static class VueHelper
    {
        private static int Port { get; } = 8080;

        private static Uri DevelopmentServerEndPoint { get; } = new Uri($"http://localhost:{Port}");

        private static TimeSpan TimeOut { get; } = TimeSpan.FromSeconds(30);

        private static string DoneMessage { get; } = "Done Compiled successfully in";

        public static void UseVueDevelopmentServer(this ISpaBuilder spa)
        {
            spa.UseProxyToSpaDevelopmentServer(async () =>
            {
                var loggerFactory = spa.ApplicationBuilder.ApplicationServices.GetService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("Vue");

                if (IsRunning())
                {
                    return DevelopmentServerEndPoint;
                }


                var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                var processInfo = new ProcessStartInfo
                {
                    FileName = isWindows ? "cmd" : "npm",
                    Arguments = $"{(isWindows ? "/c npm " : "")}run serve",
                    WorkingDirectory = "client-app",
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                };
                var process = Process.Start(processInfo);
                var tcs = new TaskCompletionSource<int>();
                _ = Task.Run(() =>
                {
                    try
                    {
                        string line;
                        while ((line = process.StandardOutput.ReadLine()) != null)
                        {
                            logger.LogInformation(line);
                            if (!tcs.Task.IsCompleted && line.Contains(DoneMessage))
                            {
                                tcs.SetResult(1);
                            }
                        }
                    }
                    catch (EndOfStreamException ex)
                    {
                        logger.LogError(ex.ToString());
                        tcs.SetException(new InvalidOperationException("'npm run serve' failed.", ex));
                    }
                });
                _ = Task.Run(() =>
                {
                    try
                    {
                        string line;
                        while ((line = process.StandardError.ReadLine()) != null)
                        {
                            logger.LogError(line);
                        }
                    }
                    catch (EndOfStreamException ex)
                    {
                        logger.LogError(ex.ToString());
                        tcs.SetException(new InvalidOperationException("'npm run serve' failed.", ex));
                    }
                });

                var timeout = Task.Delay(TimeOut);
                if (await Task.WhenAny(timeout, tcs.Task) == timeout)
                {
                    throw new TimeoutException();
                }

                return DevelopmentServerEndPoint;
            });
        }

        private static bool IsRunning() => IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners().Select(x => x.Port).Contains(Port);
    }
}
