using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
namespace Read_Xml
{
    internal class ReadXML
    {
        public static List<string> readtext()
        {
            XmlDocument xmlDoc = new XmlDocument();
            List<string> lins = new List<string>();
            try
            {
                xmlDoc.Load(Path.Combine(new string[]
                {
                    Application.StartupPath + "\\IPandPort.xml"
                }));
                XmlNode xn = xmlDoc.SelectSingleNode("items");

                XmlNodeList xnl = xn.ChildNodes;
                foreach (XmlNode xnf in xnl)
                {
                    XmlElement xe = (XmlElement)xnf;
                    XmlNodeList xnf2 = xe.ChildNodes;
                    foreach (XmlNode xn2 in xnf2)
                    {
                        lins.Add(xn2.InnerText);
                    }
                }
            }
            catch(Exception ex)
            {
                Time_Dates.TimeDates.WriteErroTOLog(ex.Message);
                lins.Clear();
                lins.Add("222.196.33.254");
                lins.Add("80");
            }
            return lins;
        }
        public static List<string> readMcaMsg()
        {
            XmlDocument xmlDoc = new XmlDocument();
            List<string> lins = new List<string>();
            try
            {
                xmlDoc.Load(Path.Combine(new string[]
                {
                    Application.StartupPath + "\\MacMsg.xml"
                }));
                XmlNode xn = xmlDoc.SelectSingleNode("items");

                XmlNodeList xnl = xn.ChildNodes;
                foreach (XmlNode xnf in xnl)
                {
                    XmlElement xe = (XmlElement)xnf;
                    XmlNodeList xnf2 = xe.ChildNodes;
                    foreach (XmlNode xn2 in xnf2)
                    {
                        lins.Add(xn2.InnerText);
                    }
                }
            }
            catch(Exception ex)
            {
                Time_Dates.TimeDates.WriteErroTOLog(ex.Message);
                lins.Clear();
                lins.Add("14-e6-e4-83-f8-90");
                lins.Add("8c-21-0a-43-fe-dc");
                lins.Add("14-75-90-65-f1-21");
                lins.Add("ec-26-ca-e1-bb-39");
                lins.Add("c8-3a-35-98-55-86");
            }
            return lins;
        }
    }
}
