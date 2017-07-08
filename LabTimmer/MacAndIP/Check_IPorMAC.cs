using Read_Xml;
using System;
using System.Collections.Generic;
namespace Check_IPorMAC
{
    internal class CheckIPorMAC
    {
        public static bool IsInLab(string ip, string mac, List<String> List_sever_ip)
        {
            List<string> LuYouMac = new List<string>();
            LuYouMac = ReadXML.readMcaMsg();
            bool result;
            for (int i = 0; i < LuYouMac.Count; i++)
            {
                if (mac.Equals(LuYouMac[i]))
                {
                    result = true;
                    return result;
                }
            }
            if (ip != null)
            {
                string[] s_arr = ip.Split(new char[]{ '.'});
                string[] server_arr = List_sever_ip[0].Split('.');

                string server_ip = server_arr[0];
                string Front = s_arr[0];
                for (int i = 1; i < s_arr.Length - 1; i++)
                {
                    Front = Front + "." + s_arr[i];
                    server_ip += "." + server_arr[i];
                }
                if (Front.Equals(server_ip))
                {
                    result = true;
                    return result;
                }
            }
            result = false;
            return result;
        }
    }
}
