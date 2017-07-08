using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net.Sockets;
namespace Get_LuYouMac
{
    internal class GetLuYouMac
    {
        public static string GetMac(List<String> mac_list)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "arp.exe";
            cmd.StartInfo.Arguments = "-a";
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.Start();
            string info = cmd.StandardOutput.ReadToEnd();
            cmd.WaitForExit();
            cmd.Close();
            string mac = "";
            string[] ips = info.Split(new string[]
            {
                "\r\n"
            }, StringSplitOptions.RemoveEmptyEntries);
            if (ips.Length >= 2)
            {
                for (int i = 0; i < ips.Length; i++)
                {
                    string[] subs = ips[i].Split(new char[]
                    {
                        ' '
                    }, StringSplitOptions.RemoveEmptyEntries);
                    if (mac_list.Contains(subs[1]))
                    {
                        mac = subs[1];
                        Time_Dates.TimeDates.WriteErroTOLog("Your router mac is " + mac);
                        break;
                    }
                }
            }
            return mac;
        }
        public static string GettrueIpV4()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            NetworkInterface[] array = nics;
            string result;
            for (int i = 0; i < array.Length; i++)
            {
                NetworkInterface adapter = array[i];
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet && adapter.OperationalStatus == OperationalStatus.Up && !adapter.Name.Contains("tooth"))
                {
                    IPInterfaceProperties ip = adapter.GetIPProperties();
                    UnicastIPAddressInformationCollection ipCollection = ip.UnicastAddresses;
                    foreach (UnicastIPAddressInformation ipadd in ipCollection)
                    {
                        if (ipadd.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            result = ipadd.Address.ToString();
                            Time_Dates.TimeDates.WriteErroTOLog("Your IP address is " + result);
                            return result;
                        }
                    }
                }
            }
            result = null;
            return result;
        }
    }
}
