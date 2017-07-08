using httpclient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
namespace Time_Dates
{
    internal class TimeDates
    {
        private object obj1 = new object();
        private object obj2 = new object();
        private HttpPostToServer _httpPost;
        public void WriteDateTOLog(string start, string stop)
        {
            string FILE_NAME = Environment.GetEnvironmentVariable("windir") + "\\UserDateLog.log";
            StreamWriter strw;
            if (File.Exists(FILE_NAME))
            {
                strw = File.AppendText(FILE_NAME);
            }
            else
            {
                strw = File.CreateText(FILE_NAME);
            }
            strw.WriteLine(start + "#" + stop);
            strw.Flush();
            strw.Close();
            GC.Collect();
        }
        static public void WriteErroTOLog(string erre)
        {
            string FILE_NAME = Application.StartupPath + "\\erro.txt";
            StreamWriter strw;
            if (File.Exists(FILE_NAME))
            {
                strw = File.AppendText(FILE_NAME);
            }
            else
            {
                strw = File.CreateText(FILE_NAME);
            }
            strw.WriteLine(DateTime.Now.ToString() + " : " + erre);
            strw.Flush();
            strw.Close();
        }
        public void SendDateToSever(string Mac, List<string> List_sever_ip)
        {
            List<string> lines = new List<string>(File.ReadAllLines(Environment.GetEnvironmentVariable("windir") + "\\UserDateLog.log"));
            List<string> line_delet = new List<string>();
            for (int i = 0; i < lines.Count; i++)
            {
                this._httpPost = new HttpPostToServer();
                if (lines[i].Equals(string.Empty))
                {
                    line_delet.Add(lines[i]);
                }
                else
                {
                    if (lines[i].ToString() != null)
                    {
                        string[] timestring = lines[i].Split(new char[]
                        {
                            '#'
                        });
                        string start = timestring[0];
                        string stop = timestring[1];
                        if (DateTime.Compare(DateTime.Parse(start), DateTime.Parse(stop)) > 0)
                        {
                            WriteErroTOLog("时间格式错误：开始时间不能大于结束时间");
                            line_delet.Add(lines[i]);
                        }
                        else
                        {
                            string res;
                            try
                            {
                                res = this._httpPost.SendToServer(Mac, start, stop, List_sever_ip);
                            }
                            catch (Exception ex)
                            {
                                WriteErroTOLog(ex.ToString());
                                this._httpPost = new HttpPostToServer();
                                goto IL_208;
                            }
                            if (res.Equals("NO1!"))
                            {
                                WriteErroTOLog(res + " 请求超时");
                                this._httpPost = new HttpPostToServer();
                                break;
                            }
                            if (res.Equals("NO2!"))
                            {
                                WriteErroTOLog(res + " 网络无连接");
                                this._httpPost = new HttpPostToServer();
                                break;
                            }
                            if (res.Equals("NO3!"))
                            {
                                WriteErroTOLog(res + "服务器无响应");
                                this._httpPost = new HttpPostToServer();
                                break;
                            }
                            WriteErroTOLog(res);
                            JObject ob = JObject.Parse(res);
                            if (ob["result"]["result"].ToString().Equals("OK"))
                            {
                                line_delet.Add(lines[i]);
                            }
                        }
                    }
                }
            IL_208:;
            }
            for (int j = 0; j < line_delet.Count; j++)
            {
                lines.Remove(line_delet[j]);
            }
            File.WriteAllLines(Environment.GetEnvironmentVariable("windir") + "\\UserDateLog.log", lines.ToArray());
            lines.Clear();
        }
    }
}
