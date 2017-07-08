using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
namespace HookTest
{
    public class GlobalHook
    {
        public delegate int HookProc(int nCode, int wParam, IntPtr lParam);
        public delegate int GlobalHookProc(int nCode, int wParam, IntPtr lParam);
        public const int WH_MOUSE_LL = 14;
        public const int WH_KEYBOARD_LL = 13;
        private const int WM_MOUSEMOVE = 512;
        private const int WM_LBUTTONDOWN = 513;
        private const int WM_RBUTTONDOWN = 516;
        private const int WM_MBUTTONDOWN = 519;
        private const int WM_LBUTTONUP = 514;
        private const int WM_RBUTTONUP = 517;
        private const int WM_MBUTTONUP = 520;
        private const int WM_LBUTTONDBLCLK = 515;
        private const int WM_RBUTTONDBLCLK = 518;
        private const int WM_MBUTTONDBLCLK = 521;
        private const int WM_KEYDOWN = 256;
        private const int WM_KEYUP = 257;
        private const int WM_SYSKEYDOWN = 260;
        private const int WM_SYSKEYUP = 261;
        private static int _hMouseHook = 0;
        private static int _hKeyboardHook = 0;
        private GlobalHook.GlobalHookProc MouseHookProcedure;
        private GlobalHook.GlobalHookProc KeyboardHookProcedure;
        public event MouseEventHandler OnMouseActivity;
        public event KeyEventHandler KeyDown;
        public event KeyPressEventHandler KeyPress;
        public event KeyEventHandler KeyUp;
        public int HMouseHook
        {
            get
            {
                return GlobalHook._hMouseHook;
            }
        }
        public int HKeyboardHook
        {
            get
            {
                return GlobalHook._hKeyboardHook;
            }
        }
        ~GlobalHook()
        {
            this.Stop();
        }
        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        public static extern int SetWindowsHookEx(int idHook, GlobalHook.GlobalHookProc lpfn, IntPtr hInstance, int threadId);
        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        public static extern bool UnhookWindowsHookEx(int idHook);
        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        public static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);
        public bool Start()
        {
            bool result;
            if (GlobalHook._hMouseHook == 0)
            {
                this.MouseHookProcedure = new GlobalHook.GlobalHookProc(this.MouseHookProc);
                try
                {
                    GlobalHook._hMouseHook = GlobalHook.SetWindowsHookEx(14, this.MouseHookProcedure, Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);
                }
                catch (Exception err_4B)
                {
                    Time_Dates.TimeDates.WriteErroTOLog(err_4B.ToString());
                }
                if (GlobalHook._hMouseHook == 0)
                {
                    this.Stop();
                    result = false;
                    return result;
                }
            }
            if (GlobalHook._hKeyboardHook == 0)
            {
                this.KeyboardHookProcedure = new GlobalHook.GlobalHookProc(this.KeyboardHookProc);
                try
                {
                    GlobalHook._hKeyboardHook = GlobalHook.SetWindowsHookEx(13, this.KeyboardHookProcedure, Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);
                }
                catch (Exception err2_B7)
                {
                    Time_Dates.TimeDates.WriteErroTOLog(err2_B7.ToString());
                }
                if (GlobalHook._hKeyboardHook == 0)
                {
                    this.Stop();
                    result = false;
                    return result;
                }
            }
            result = true;
            return result;
        }
        public void Stop()
        {
            bool retMouse = true;
            bool retKeyboard = true;
            if (GlobalHook._hMouseHook != 0)
            {
                retMouse = GlobalHook.UnhookWindowsHookEx(GlobalHook._hMouseHook);
                GlobalHook._hMouseHook = 0;
            }
            if (GlobalHook._hKeyboardHook != 0)
            {
                retKeyboard = GlobalHook.UnhookWindowsHookEx(GlobalHook._hKeyboardHook);
                GlobalHook._hKeyboardHook = 0;
            }
            if (!retMouse || !retKeyboard)
            {
            }
        }
        public void Stop(int hMouseHook, int hKeyboardHook)
        {
            if (hMouseHook != 0)
            {
                GlobalHook.UnhookWindowsHookEx(hMouseHook);
            }
            if (hKeyboardHook != 0)
            {
                GlobalHook.UnhookWindowsHookEx(hKeyboardHook);
            }
        }
        private int MouseHookProc(int nCode, int wParam, IntPtr lParam)
        {
            if (nCode >= 0 && this.OnMouseActivity != null)
            {
                MouseButtons button = MouseButtons.None;
                if (wParam != 513)
                {
                    if (wParam == 516)
                    {
                        button = MouseButtons.Right;
                    }
                }
                else
                {
                    button = MouseButtons.Left;
                }
                int clickCount = 0;
                if (button != MouseButtons.None)
                {
                    if (wParam == 515 || wParam == 518)
                    {
                        clickCount = 2;
                    }
                    else
                    {
                        clickCount = 1;
                    }
                }
                MouseHookStruct MyMouseHookStruct = (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));
                MouseEventArgs e = new MouseEventArgs(button, clickCount, MyMouseHookStruct.pt.x, MyMouseHookStruct.pt.y, 0);
                this.OnMouseActivity(this, e);
            }
            return GlobalHook.CallNextHookEx(GlobalHook._hMouseHook, nCode, wParam, lParam);
        }
        [DllImport("user32")]
        public static extern int ToAscii(int uVirtKey, int uScanCode, byte[] lpbKeyState, byte[] lpwTransKey, int fuState);
        [DllImport("user32")]
        public static extern int GetKeyboardState(byte[] pbKeyState);
        private int KeyboardHookProc(int nCode, int wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (this.KeyDown != null || this.KeyUp != null || this.KeyPress != null))
            {
                KeyboardHookStruct MyKeyboardHookStruct = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));
                if (this.KeyDown != null && (wParam == 256 || wParam == 260))
                {
                    Keys keyData = (Keys)MyKeyboardHookStruct.vkCode;
                    KeyEventArgs e = new KeyEventArgs(keyData);
                    this.KeyDown(this, e);
                }
                if (this.KeyPress != null && wParam == 256)
                {
                    byte[] keyState = new byte[256];
                    GlobalHook.GetKeyboardState(keyState);
                    byte[] inBuffer = new byte[2];
                    if (GlobalHook.ToAscii(MyKeyboardHookStruct.vkCode, MyKeyboardHookStruct.scanCode, keyState, inBuffer, MyKeyboardHookStruct.flags) == 1)
                    {
                        KeyPressEventArgs e2 = new KeyPressEventArgs((char)inBuffer[0]);
                        this.KeyPress(this, e2);
                    }
                }
                if (this.KeyUp != null && (wParam == 257 || wParam == 261))
                {
                    Keys keyData = (Keys)MyKeyboardHookStruct.vkCode;
                    KeyEventArgs e = new KeyEventArgs(keyData);
                    this.KeyUp(this, e);
                }
            }
            return GlobalHook.CallNextHookEx(GlobalHook._hKeyboardHook, nCode, wParam, lParam);
        }
    }
}
