using System;
using Microsoft.Win32;
using System.IO;
namespace Auto_RUN
{
    internal class AutoRUN
    {
        public static void SetAutoRun(string fileName, bool isAutoRun)
        {
            RegistryKey reg = null;
            try
            {
                if (!File.Exists(fileName))
                {
                    throw new Exception(fileName + "该文件不存在!");
                }
                string name = fileName.Substring(fileName.LastIndexOf("\\") + 1);
                reg = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (reg == null)
                {
                    reg = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
                }
                if (isAutoRun)
                {
                    reg.SetValue(name, fileName);
                }
                else
                {
                    reg.SetValue(name, false);
                }
            }
            catch (Exception ex)
            {
                Time_Dates.TimeDates.WriteErroTOLog(ex.ToString());
            }
            finally
            {
                if (reg != null)
                {
                    reg.Close();
                }
            }
        }
        public static bool DeleteAutoRun(string fileName)
        {
            bool result;
            try
            {
                string keyName = fileName.Substring(fileName.LastIndexOf("\\") + 1);
                RegistryKey runKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (runKey == null)
                {
                    runKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\\\Microsoft\\\\Windows\\\\CurrentVersion\\\\Run");
                }
                runKey.DeleteValue(keyName);
                runKey.Close();
            }
            catch
            {
                result = false;
                return result;
            }
            result = true;
            return result;
        }
    }
}
