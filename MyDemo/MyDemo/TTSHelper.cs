using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MyDemo
{
    public class TTSHelper
    {
        const string dllpath = "DLL\\jTTS_ML.dll";
        [DllImport(dllpath)]
        public static extern int jTTS_Play(string str,int intpar);
        [DllImport(dllpath)]
        public static extern void jTTS_End();
        [DllImport(dllpath)]
        public static extern void jTTS_Set();
        [DllImport(dllpath)]
        public static extern int jTTS_Init(string a,string b);
        [DllImport(dllpath)]
        public static extern void jTTS_GetVoiceCount();

    }
}
