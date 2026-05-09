using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace SmartFillMonitor.Services.Logs
{
    public static class LogService                                          
    {
       public static void Info(string message)=> Log.Information(message);
       public static void Warn(string message)=> Log.Warning(message);
       public static void Debug(string message)=> Log.Debug(message);
       public static void Verbose(string message)=> Log.Verbose(message);
       public static void Fatal(string message)=> Log.Fatal(message);
       public static void Fatal(string message,Exception ex = null)=> Log.Fatal(ex,message);
       public static void Error(string message,Exception ex = null) => Log.Error(ex,message);
    }
}
