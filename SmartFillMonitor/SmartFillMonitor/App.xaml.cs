using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SmartFillMonitor.Services;
using SmartFillMonitor.ViewModels;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SmartFillMonitor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //全局唯一日志框
        public static RichTextBox LogView = new RichTextBox
        {
            IsReadOnly = true,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            Background = Brushes.Black,
            Foreground = Brushes.White,
            FontFamily = new FontFamily("Consolas")
        };

        private const string LogTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss fff} [{Level}] ({ThreadId)} {Message} {NewLine} {NewLine} {Exception}";
        private const string LogPath = "Logs\\log-.txt";
        private const string DbFilePaht = "SmartFillMonitor.db";
        private const string DbConnectionString = "Data Source=SmartFillMonitor.db";//给Freesql使用的连接字符串，SQLite不需要用户名密码
        public IServiceProvider ServiceProvider { get; private set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ConfigLogging();//配置日志
            InitialCoreServiceAsync();//调用数据库初始化方法，确保数据库文件和表存在
            var services = new ServiceCollection();//创建DI集合
            ConfigureServices(services);//把所有ViewModel放进这个集合
            ServiceProvider = services.BuildServiceProvider();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

        }

        private async Task InitialCoreServiceAsync()
        {
            Log.Debug("Initialzie Database");
            DbProvider.Initialize(DbConnectionString);
        }
        private void ConfigLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.WithThreadId()
                .WriteTo.RichTextBox(LogView, outputTemplate: LogTemplate)
                .WriteTo.Console(outputTemplate: LogTemplate)
                .WriteTo.File(LogPath, rollingInterval: RollingInterval.Day, outputTemplate : LogTemplate, shared : true)
                .WriteTo.SQLite(DbFilePaht, tableName : "SystemLog", storeTimestampInUtc : false)
                .CreateLogger();
        }

        #region DI
        private void ConfigureServices(IServiceCollection service)
        {
            service.AddSingleton<AlarmsViewModel>();
            service.AddSingleton<DashBoardViewModel>();
            service.AddSingleton<DashQueryViewModel>();
            service.AddSingleton<LogsViewModel>();
            service.AddSingleton<SettingViewModel>();
            service.AddSingleton<MainWindowViewModel>();
        }
        #endregion

    }
}
