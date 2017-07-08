using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
namespace httpclient
{
    internal class HttpPostToServer
    {
        public string SendToServer(string mac, string starttime, string endtime, List<string> List_sever_ip)
        {
            string Sever_IP = string.Concat(new string[]
            {
                "http://",
                List_sever_ip[0],
                ":",
                List_sever_ip[1],
                "/addmsg/"
            });
            string url = string.Concat(new string[]
            {
                Sever_IP,
                mac,
                "/",
                starttime,
                "/",
                endtime,
                "/"
            });
            string body = "";
            string contentType = "application/x-www-form-urlencoded";
            return HttpPostToServer.GetHttp(url, body, contentType);
        }
        private static string GetHttp(string url, string body, string contentType)
        {
            GC.Collect();
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.KeepAlive = false;
            httpWebRequest.Method = "GET";
            httpWebRequest.ContentType = contentType;
            httpWebRequest.Timeout = 2000;
            httpWebRequest.ServicePoint.Expect100Continue = false;
            ServicePointManager.DefaultConnectionLimit = 200;
            HttpWebResponse response = null;
            string result;
            try
            {
                response = (HttpWebResponse)httpWebRequest.GetResponse();
                if (response.StatusCode == HttpStatusCode.RequestTimeout)
                {
                    if (response != null)
                    {
                        response.Close();
                        response = null;
                    }
                    if (httpWebRequest != null)
                    {
                        httpWebRequest.Abort();
                        httpWebRequest = null;
                    }
                    result = "NO1!";
                }
                else
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        if (response != null)
                        {
                            response.Close();
                            response = null;
                        }
                        if (httpWebRequest != null)
                        {
                            httpWebRequest.Abort();
                            httpWebRequest = null;
                        }
                        result = "NO3!";
                    }
                    else
                    {
                        Stream myResponseStream = response.GetResponseStream();
                        StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                        string retString = myStreamReader.ReadToEnd();
                        if (response != null)
                        {
                            response.Close();
                            response = null;
                        }
                        if (httpWebRequest != null)
                        {
                            httpWebRequest.Abort();
                            httpWebRequest = null;
                        }
                        httpWebRequest = null;
                        myStreamReader.Close();
                        myResponseStream.Close();
                        result = retString;
                    }
                }
            }
            catch (WebException wex_159)
            {
                Time_Dates.TimeDates.WriteErroTOLog(wex_159.ToString());
                if (httpWebRequest != null)
                {
                    httpWebRequest.Abort();
                    httpWebRequest = null;
                }
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
                result = "NO2!";
            }
            return result;
        }
    }
}
