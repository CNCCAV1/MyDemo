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
        public static extern void Load();

    }
}
