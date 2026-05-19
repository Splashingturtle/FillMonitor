using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FreeSql.DataAnnotations;
using Microsoft.VisualBasic;
namespace SmartFillMonitor.Models
{
    #region 数据库实体类 - 报警记录

    [Table(Name = "AlarmRecord")]
    public class AlarmRecord
    {
        [Column(IsIdentity = true, IsPrimary = true)]
        public long Id { get; set; }

        //报警类型
        public AlarmCode AlarmCode { get; set; }
        //报警级别
        public AlarmServerity AlarmServerity { get; set; }
        //报警开始时间
        public DateTime StartTime { get; set; } = DateTime.Now;
        //报警恢复（设备接触故障）时间
        public DateTime EndTime { get; set; }

        //报警持续时间（秒）
        public double? DurationSeconds { get; set; }

        //是否为活动报警（true表示当前仍在持续的报警，false表示已经恢复的报警）
        public bool IsActive { get; set; } = true;

        //是否被人工确认
        public bool IsAcknowledged { get; set; } = false;

        public DateTime? AckTime { get; set; }

        //确认操作人（记录用户名）
        public string? AckUser { get; set; }

        //动态消息（温度过高，当前150摄氏度）
        public string? Message { get; set; }

        //处理建议（通常从枚举中获取）
        public string? Description { get; set; }
    }
    public enum AlarmServerity
    {
        [Description("所有")]
        All = 0,
        [Description("提示")]
        Info = 1,
        [Description("警告")]
        Warning = 2,
        [Description("错误")]
        Error = 3,
        [Description("致命")]
        Critical = 4,
    }

    public enum AlarmCode
    {
        [Description("无")]
        None = 0,
        [Description("原料桶液位过低")]
        LowLiquidLevel = 1001,
        [Description("压缩空气压力偏低")]
        LowAirPresure = 2001,
        [Description("加热温度过高")]
        HighTemperature = 3001,
        [Description("PLC通讯故障")]
        CommunicationError = 4001,
        [Description("系统内部错误")]
        SystemError = 5001,
    }
    #endregion

    #region

    public class AlarmUiModel : INotifyPropertyChanged
    {
        private long _id;
        private string _code;
        private string _title;
        private string _timeStr;
        private string _description;

        public long Id
        {
            get => _id;
            set
            {
                if (value == _id) return;
                _id = value;
                OnPropertyChanged();
            }
        }

        public string Code
        {
            get => _code;
            set
            {
                if (value == _code) return;
                _code = value;
                OnPropertyChanged();
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                if (value == _title) return;
                _title = value;
                OnPropertyChanged();
            }
        }

        public string TimeStr
        {
            get => _timeStr;
            set
            {
                if (value == _timeStr) return;
                _timeStr = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                if (value == _description) return;
                _description = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static AlarmUiModel FromRecord(AlarmRecord record)
        {
            var title = record.AlarmCode.GetDescription();
            return new AlarmUiModel
            {
                Id = record.Id,
                Code = $"E{(int)record.AlarmCode}",//加上前缀显得更像故障码 
                Title = title,
                TimeStr = record.StartTime.ToString("MM-dd HH:mm:ss"),
                Description = record.Description
            };
        }
    }
    #endregion
    
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value) 
        { 
            var field = value.GetType().GetField(value.ToString());
            var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            return attribute?.Description ?? value.ToString();
        }
    }
}
