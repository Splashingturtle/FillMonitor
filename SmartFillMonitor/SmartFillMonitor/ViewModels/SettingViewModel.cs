using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartFillMonitor.Models;
using SmartFillMonitor.Services;
using SmartFillMonitor.Services.Logs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFillMonitor.ViewModels
{
    public partial class SettingViewModel : ObservableObject
    {
        public ObservableCollection<string> PortNames { get; } = new ObservableCollection<string>();
        public ObservableCollection<int> BaudRates { get; } = new ObservableCollection<int>()
        {
            9600,
            19200,
            38400,
            57600,
            115200
        };
        public ObservableCollection<int> DataBitsOptions { get; } = new ObservableCollection<int>()
        {
           7,8
        };
        public ObservableCollection<string> ParityOptions { get; } = new ObservableCollection<string>()
        {
            "None",
            "Odd",
            "Even"
        };
        public ObservableCollection<string> StopBitsOptions { get; } = new()
        {
            "None", "One", "Two"
        };

        [ObservableProperty]
        public string portName = "COM3";

        [ObservableProperty]
        public int selectedBaud = 115200;

        [ObservableProperty]
        public int selectedDataBits = 8;

        [ObservableProperty]
        public string selectedParity = "None";

        [ObservableProperty]
        public string selectedStopBits = "One";

        [ObservableProperty]
        public bool autoConnect = true;

        [ObservableProperty]
        public bool alarmSound = true;

        [ObservableProperty]
        public bool debugLogMode = false;

        public SettingViewModel()
        {
            RefreshPortList();
            try
            {
                LoadSettings();
            }
            catch (Exception ex)
            {
                //加载失败
                LogService.Error($"加载配置失败,使用默认值，原因：{ex.Message}");
            }
        }

        private void RefreshPortList()
        {
            PortNames.Clear();
            try
            {
                var ports = PlcService.GetAvailablePorts() ?? SerialPort.GetPortNames();
                foreach ( var item in ports)
                {
                    PortNames.Add(item);
                }
                if (!string.IsNullOrEmpty(PortName) && !PortNames.Contains(PortName))
                {
                    PortName = PortNames.Count > 0 ? PortNames[0] : PortName;

                }
            }
            catch (Exception ex)
            {
                LogService.Error($"获取窗口列表失败：{ex.Message}");
                PortNames.Clear();
                PortNames.Add("COM1");
                PortNames.Add("COM2");
            }
        }
        private async void LoadSettings()
        {
            var ds = await ConfigServices.LoadDeviceSettingAsync();
            PortName = ds.PortName;
            SelectedBaud = ds.BaudRate;
            SelectedDataBits = ds.DataBits;
            SelectedParity = ds.Parity;
            SelectedStopBits = string.IsNullOrEmpty(ds.StopBits) ? "One" : ds.StopBits;
            AutoConnect = ds.AutoConnect;
            AlarmSound = ds.AlarmSound;
            DebugLogMode = ds.DebugLogMode;
        }

        [RelayCommand]
        public async Task SaveAsync()
        {
            try
            {
                var model = new DeviceSettings
                {
                    PortName = PortName,
                    BaudRate = SelectedBaud,
                    DataBits = SelectedDataBits,
                    Parity = SelectedParity,
                    StopBits = SelectedStopBits,
                    AutoConnect = AutoConnect,
                    AlarmSound = AlarmSound,
                    DebugLogMode = DebugLogMode
                };
                await ConfigServices.SaveDeviceSettingAsync(model);
            }
            catch (Exception ex)
            {
                //保存失败
                LogService.Error($"保存配置失败:{ex}");
            }
        }
    }
}
