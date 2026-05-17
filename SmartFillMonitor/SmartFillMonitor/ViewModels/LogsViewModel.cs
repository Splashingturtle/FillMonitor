using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartFillMonitor.Models;
using SmartFillMonitor.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Formats.Asn1;
using System.IO;
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
        
        private const int PageSize = 50;//每页显示50条数据

        public LogsViewModel()
        {
            _ = LoadLogsAsync();
        }

        [RelayCommand]
        private async Task SearchAsync()
        {
            PageIndex = 1;//重置页码
            await LoadLogsAsync();
        }

        [RelayCommand]
        private async Task ExportAsync()
        {
            var query = BuildQuery();   
            var allData = await query.OrderByDescending(x => x.Timestamp).ToListAsync();
            if (allData.Count == 0)
            {
                MessageBox.Show("没有数据可以导出");
                return;
            }

            var lines = new List<string> { "时间,等级,内容,异常" };
            lines.AddRange(allData.Select(x => $"{x.Timestamp:yyyy-MM-dd HH:mm:ss},{x.Level},\"{x.RenderedMessage.Replace("\"", "\"\"")}\",\"{x.Exception?.Replace("\n","")}\""));
            var path = $"Logs_Export_{DateTime.Now:yyyyMMddHHmmss}.csv";
            await File.WriteAllLinesAsync(path, lines, Encoding.UTF8);
            MessageBox.Show($"日志已导出到 {path}");
        }

        [RelayCommand]
        private async Task PreviousPageAsync()
        {
            if (PageIndex <= 1) return;
            PageIndex--;
            await LoadLogsAsync();

        }

        [RelayCommand]
        private async Task NextPageAsync()
        {
            if (Logs.Count < PageSize) return;  //已经是最后一页了
            PageIndex++;
            await LoadLogsAsync();
        }

        private async Task LoadLogsAsync()
        {
            if (IsBusy) return;

            IsBusy = true;

            try
            {
                var query = BuildQuery();
                TotalCount = (int)await query.CountAsync();
                var data = await query.OrderByDescending(x => x.Timestamp)
                    .Page(PageIndex, PageSize)
                    .ToListAsync();
                Logs = new ObservableCollection<SystemLog>(data);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载日志失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
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
            query = query.Where($"date(\"Timestamp\")>=date('{start}')AND date(\"Timestamp\")<=date('{end}')");

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
