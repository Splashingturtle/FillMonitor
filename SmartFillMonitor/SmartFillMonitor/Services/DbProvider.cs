using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFillMonitor.Services
{
    public static class DbProvider
    {
        private static readonly object _lock = new object();

        public static IFreeSql Fsql { get; private set; }

        public static void Initialize(string connectionString, FreeSql.DataType data = FreeSql.DataType.Sqlite)
        {
            if (Fsql != null) return;//已经初始化过了
            lock (_lock)
            {
                if (Fsql != null) return;//双重检查锁定，确保线程安全
                Fsql = new FreeSql.FreeSqlBuilder()
                    .UseConnectionString(FreeSql.DataType.Sqlite, connectionString)
                    .UseAdoConnectionPool(true)
                    .UseMonitorCommand(
                    cmd =>
                    {

                    },
                    (cmd, traceLog) =>
                    {
                        Console.Write($"[SQL] {cmd.CommandText}\r\n ->{traceLog}");
                    }
                    )
                    .UseAutoSyncStructure(true)
                    .UseLazyLoading(true)
                    .Build();
            }
        }
    }
}
