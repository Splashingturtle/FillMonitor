using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFillMonitor.Models
{
    public class DeviceSettings
    {
        public string PortName { get; set; } = "COM3";
        public int BaudRate { get; set; } = 115200;
        public int DataBits { get; set; } = 8;
        public string Parity { get; set; } = "None";
        public string StopBits { get; set; } = "One";
        public bool AutoConnect { get; set; } = true;
        public bool AlarmSound { get; set; } = true;
        public bool DebugLogMode { get; set; } = false;
    }
}
