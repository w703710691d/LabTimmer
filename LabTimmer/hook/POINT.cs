using System;
using System.Runtime.InteropServices;
namespace HookTest
{
    [StructLayout(LayoutKind.Sequential)]
    public class POINT
    {
        public int x;
        public int y;
    }
}
