using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartFillMonitor.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFillMonitor.ViewModels
{
    public partial class AlarmsViewModel:ObservableObject
    {
        public ObservableCollection<AlarmUiModel> ActiveAlarms { get; set; }

        public ObservableCollection<AlarmUiModel> HistoryAlarms { get; set; }

        [ObservableProperty]
        public int activeAlarmCount;

        [ObservableProperty]
        private DateTime historyStartTime = DateTime.Today.AddDays(-1);

        [ObservableProperty]
        private DateTime historyEndTime = DateTime.Today;

        public AlarmsViewModel()
        {

        }

        [RelayCommand]
        private async Task RefreshAsync()
        {

        }

        [RelayCommand]
        private async Task LoadHistoryAlarmsAsync()
        {

        }
    }
}
