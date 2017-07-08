using System;
using System.Windows.Forms;
using System.Diagnostics;

namespace NewLabTimmer
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
            {
                MessageBox.Show("您已经运行本计时程序，无需再次运行~");
            }
            else
            {
                Application.Run(new LabTimmer.Form1());
            }
        }
    }
}
