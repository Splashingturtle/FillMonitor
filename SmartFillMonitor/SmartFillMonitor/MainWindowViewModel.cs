using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using SmartFillMonitor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;


namespace SmartFillMonitor
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private object mainContent;
        private readonly IServiceProvider _serviceProvider;

        private readonly DispatcherTimer _timer;

        [ObservableProperty]
        private string currentTime;

        public MainWindowViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        #region Navigation Commands
        [RelayCommand]  
        private void Navigate(string? destination)
        {
            if (string.IsNullOrEmpty(destination)) return;

            switch (destination)
            {
                case "Dashboard":
                    MainContent = _serviceProvider.GetRequiredService<DashBoardViewModel>();
                    break;
                case "DataQuery":
                    MainContent = _serviceProvider.GetRequiredService<DashQueryViewModel>();
                    break;
                case "Logs":
                    MainContent = _serviceProvider.GetRequiredService<DashBoardViewModel>();
                    break;
                case "Alarms":
                    MainContent = _serviceProvider.GetRequiredService<AlarmsViewModel>();
                    break;
                case "Settings":
                    MainContent = _serviceProvider.GetRequiredService<SettingViewModel>();
                    break;
                default:
                    break;
            }
        }
        #endregion
    }
}
