using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartFillMonitor.Models;
using SmartFillMonitor.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace SmartFillMonitor.ViewModels
{
    public partial class LogsViewModel : ObservableObject
    {
        [ObservableProperty]
        private DateTime _startDate = DateTime.Today;

        [ObservableProperty]
        private DateTime _endDate = DateTime.Today.AddDays(1).AddSeconds(-1);

        [ObservableProperty]
        private string _selectedLevel = "All";

        public ObservableCollection<string> LogLevels { get; } = new ObservableCollection<string>
        {
            "All",
            "Debug",
            "Information",
            "Warning",
            "Error",
        };

        [ObservableProperty]
        private ObservableCollection<SystemLog> _logs = new ObservableCollection<SystemLog>();

        [ObservableProperty]
        private bool _isBusy;//数据库操作是否忙碌

        [ObservableProperty]
        private int _totalCount;

        [ObservableProperty]
        private int _pageIndex = 1;

        [ObservableProperty]
        private string _searchText = "";

        [RelayCommand]
        private async Task SearchAsync()
        {

        }

        [RelayCommand]
        private async Task ExportAsync()
        {
            
        }

        [RelayCommand]
        private async Task PreviousPageAsync()
        {

        }

        [RelayCommand]
        private async Task NextPageAsync()
        {

        }

        private async Task LoadLogsAsync()
        {
            if (IsBusy) return;

            IsBusy = true;

            try
            {
                var query = BuildQuery();
                TotalCount = (int)await query.CountAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                IsBusy = false; 
            }
        }

        private FreeSql.ISelect<SystemLog> BuildQuery()
        {
            var query = DbProvider.Fsql.Select<SystemLog>();
            var start = StartDate.ToString("yyyy-MM-dd");
            var end = EndDate.ToString("yyyy-MM-dd");
            query = query.Where($"date(\"Timestamp\")>=date('{start}'AND date(\"Timestamp\")<=date('{end}')");

            if (!string.IsNullOrEmpty(SearchText))
            {
                query = query.Where(x => x.RenderedMessage.Contains(SearchText));
            }
            if (SelectedLevel!="All" && !string.IsNullOrEmpty(SelectedLevel))
            {
                query = query.Where(x => x.Level.Contains(SelectedLevel));
            }

            return query;
        }
    }
}
