using SmartFillMonitor.Models;
using SmartFillMonitor.Services.Logs;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace SmartFillMonitor.Services
{
    //参数配置的管理：读取、保存配置
    public static class ConfigServices
    {
        private const string SettingsFileName = "device-settings.json";
        private static readonly SemaphoreSlim IoLock = new SemaphoreSlim(1, 1);
        public static string GetSettingsPath() => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SettingsFileName);

        public static async Task<DeviceSettings> LoadDeviceSettingAsync()
        {
            var path = GetSettingsPath();
            DeviceSettings? settings = null;
            await IoLock.WaitAsync();
            try
            {
                if (File.Exists(path))
                {
                    try
                    {
                        var json = await File.ReadAllTextAsync(path);
                        var opt = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };//设置属性名称不区分大小写
                        settings = JsonSerializer.Deserialize<DeviceSettings>(json, opt);
                        if (settings != null)
                        {
                            LogService.Info($"配置加载成功！{path}");
                            return settings;
                        }
                    }
                    catch (JsonException jsonEx)
                    {
                        LogService.Error($"配置文件格式错误，将其充值位默认值：{jsonEx.Message}"); 
                        BackCorrupFile(path);//备份损坏的文件
                    }
                    catch (Exception ex)
                    {
                        LogService.Error($"读取配置文件失败：{ex.Message}");
                    }
                }
                else
                {
                    //提示文件不存在，返回默认设置
                    LogService.Warn($"配置文件不存在，将创建新的默认配置：{path}");
                }
            }
            finally
            {
                IoLock.Release();//释放锁,允许其他线程访问
            }

            if (settings == null)
            {
                settings = new DeviceSettings();
                //保存方法
                await SaveDeviceSettingAsync(settings);
            }

            return settings;
        }

        public static async Task<bool> SaveDeviceSettingAsync(DeviceSettings settings)
        {
            if (settings == null)
            {
                MessageBox.Show("无法保存配置，设置对象为null！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            var path = GetSettingsPath();
            var tempPath = path + ".tmp";//临时文件路径
            await IoLock.WaitAsync();
            try
            {
                var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });//格式化输出
                await File.WriteAllTextAsync(tempPath, json);
                File.Move(tempPath, path, true);//覆盖写入
                //提示配置保存成功
                LogService.Info($"配置保存成功！{path}");
                MessageBox.Show("配置保存成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            }
            catch (Exception ex)
            {
                LogService.Error($"保存配置文件失败",ex);
                return false;
            }
            finally
            {
                IoLock.Release();//释放锁,允许其他线程访问
                if (File.Exists(tempPath))
                {
                    try
                    {
                        File.Delete(tempPath);
                    }
                    catch
                    {
                        //日志记录异常
                    }
                }
            }
        }

        public static void BackCorrupFile(string originalPath)
        {
            try
            {
                //生成备份路径
                var backupPath = originalPath + ".corrupt" + DateTime.Now.ToString("yyyyMMddHHmmss");
                File.Copy(originalPath, backupPath, true);//拷贝操作
                LogService.Warn($"已备份损坏的配置文件：{backupPath}");    
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
