using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp4
{
    public class Class1
    {

        private readonly BardCodeHooK _barCode = new BardCodeHooK();

        private void InitBarcode()
        {
            _barCode.BarCodeEvent += OnBarcode;
            _barCode.Start();
        }

        private void OnBarcode(BardCodeHooK.BarCodes barCode)
        {

            if (!barCode.IsValid) return;
            var str = barCode.BarCode.Replace("\0", "");

            Console.WriteLine($"{DateTime.Now:MM:dd HH:mm:ss.fff} [{str}]");

        }
        ///键盘钩子类

        /// <summary>
            /// 键盘钩子
            /// </summary>
        public class BardCodeHooK
        {

            #region 定义

            //获取键盘输入或者USB扫描枪数据 可以是没有焦点 应为使用的是全局钩子USB扫描枪 是模拟键盘按下这里主要处理扫描枪的值，手动输入的值不太好处理

            public delegate void BardCodeDeletegate(BarCodes barCode);
            public event BardCodeDeletegate BarCodeEvent;

            //定义成静态，这样不会抛出回收异常
            private static HookProc _hookproc;

            public struct BarCodes
            {
                public int VirtKey;//虚拟吗
                public int ScanCode;//扫描码
                public string KeyName;//键名
                public uint Ascll;//Ascll
                public char Chr;//字符

                public string BarCode;//条码信息 保存最终的条码
                public bool IsValid;//条码是否有效
                public DateTime Time;//扫描时间,
            }

            private struct EventMsg
            {
                public int message;
                public int paramL;
                public int paramH;
                public int Time;
                public int hwnd;
            }

            private delegate int HookProc(int nCode, int wParam, IntPtr lParam);

            private BarCodes _barCode;
            private int _hKeyboardHook;
            private string _strBarCode = "";

            #endregion

            public bool Start()
            {
                if (_hKeyboardHook != 0) return _hKeyboardHook != 0;

                _hookproc = KeyboardHookProc;
                var modulePtr = GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName);

                _hKeyboardHook = SetWindowsHookEx(13, _hookproc, modulePtr, 0);
                return (_hKeyboardHook != 0);
            }

            public bool Stop()
            {
                return _hKeyboardHook == 0 || UnhookWindowsHookEx(_hKeyboardHook);
            }

            private int KeyboardHookProc(int nCode, int wParam, IntPtr lParam)
            {
                if (nCode != 0) return CallNextHookEx(_hKeyboardHook, nCode, wParam, lParam);
                var msg = (EventMsg)Marshal.PtrToStructure(lParam, typeof(EventMsg));
                if (wParam != 0x100) return CallNextHookEx(_hKeyboardHook, nCode, wParam, lParam);
                _barCode.VirtKey = msg.message & 0xff;//虚拟吗
                _barCode.ScanCode = msg.paramL & 0xff;//扫描码
                var strKeyName = new StringBuilder(225);
                _barCode.KeyName = GetKeyNameText(_barCode.ScanCode * 65536, strKeyName, 255) > 0 ? strKeyName.ToString().Trim(' ', '\0') : "";
                var kbArray = new byte[256];
                uint uKey = 0;
                GetKeyboardState(kbArray);


                if (ToAscii(_barCode.VirtKey, _barCode.ScanCode, kbArray, ref uKey, 0))
                {
                    _barCode.Ascll = uKey;
                    _barCode.Chr = Convert.ToChar(uKey);
                }

                var ts = DateTime.Now.Subtract(_barCode.Time);

                if (ts.TotalMilliseconds > 50)
                {//时间戳，大于50 毫秒表示手动输入
                    _strBarCode = _barCode.Chr.ToString();
                }
                else
                {
                    if ((msg.message & 0xff) == 13 && _strBarCode.Length > 3)
                    {//回车
                        _barCode.BarCode = _strBarCode;
                        _barCode.IsValid = true;
                    }
                    _strBarCode += _barCode.Chr.ToString();
                }
                _barCode.Time = DateTime.Now;
                BarCodeEvent?.Invoke(_barCode);//触发事件
                _barCode.IsValid = false;
                return CallNextHookEx(_hKeyboardHook, nCode, wParam, lParam);
            }

            #region DllImport

            [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
            private static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

            [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
            private static extern bool UnhookWindowsHookEx(int idHook);

            [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
            private static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);

            [DllImport("user32", EntryPoint = "GetKeyNameText")]
            private static extern int GetKeyNameText(int param, StringBuilder lpBuffer, int nSize);

            [DllImport("user32", EntryPoint = "GetKeyboardState")]
            private static extern int GetKeyboardState(byte[] pbKeyState);

            [DllImport("user32", EntryPoint = "ToAscii")]
            private static extern bool ToAscii(int virtualKey, int scanCode, byte[] lpKeySate, ref uint lpChar, int uFlags);

            [DllImport("kernel32.dll")]
            public static extern IntPtr GetModuleHandle(string name);

            #endregion

        }
    }
}