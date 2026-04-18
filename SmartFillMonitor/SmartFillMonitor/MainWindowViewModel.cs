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
        private object currentView;
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
        private void NavigateToDashBoard()
        {
            CurrentView = _serviceProvider.GetRequiredService<DashBoardViewModel>();
        }
        [RelayCommand]
        private void NavigateToDataQuery()
        {
            CurrentView = _serviceProvider.GetRequiredService<DashQueryViewModel>();
        }
        [RelayCommand]
        private void NavigateToLogs()
        {
            CurrentView = _serviceProvider.GetRequiredService<LogsViewModel>();
        }
        [RelayCommand]
        private void NavigateToAlarms()
        {
            CurrentView = _serviceProvider.GetRequiredService<AlarmsViewModel>();
        }
        [RelayCommand]
        private void NavigateToSetting()
        {
            CurrentView = _serviceProvider.GetRequiredService<SettingViewModel>();
        }

        #endregion
    }
}
