using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    /// <summary>
    /// 公司名称：神思电子技术股份有限公司
    /// 官方网址：http://www.sdses.com/
    /// 产品名称：台式居民身份证阅读机具
    /// 产品型号：SS628(100) 
    /// </summary>
    public class SynthesisReader : IDCardReader
    {
        /// <summary>
        /// 自动检测身份证读卡器并初始化
        /// </summary>
        /// <returns>
        ///     true：成功
        ///     false：失败
        /// </returns>
        protected override bool Initialize()
        {
            byte CMD = 0x41;
            int para1 = 8811;
            int para2 = 9986;
            for (int i = 1001; i <= 1016; i++)
            {
                if (UCommand1(ref CMD, ref i, ref para1, ref para2) == 62171)
                {
                    InnerPort = i;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 关闭读卡器
        /// </summary>
        /// <returns>
        ///     0：成功
        ///     -1：失败
        /// </returns>
        protected override int CloseComm()
        {
            byte CMD = 0x42;
            int para1 = 8811;
            int para2 = 9986;
            UCommand1(ref CMD, ref InnerPort, ref para1, ref para2);

            return 0;
        }

        /// <summary>
        /// 验证卡
        /// </summary>
        /// <returns>
        ///     0：验卡成功
        ///     -1：验卡失败
        /// </returns>
        protected override int Authenticate()
        {
            byte CMD = 0x43;
            int para1 = 8811;
            int para2 = 9986;
            if (UCommand1(ref CMD, ref InnerPort, ref para1, ref para2) == 62171) return 0; else return -1;
        }

        /// <summary>
        /// 读卡基本信息
        /// </summary>
        /// <returns>
        ///     0：读卡成功，卡片不带指纹数据
        ///     1：读卡成功，卡片带指纹数据
        ///     -1：读卡失败
        /// </returns>
        protected override int ReadContent()
        {
            byte CMD = 0x44;
            int para1 = 8811;
            int para2 = 9986;
            int nRet = UCommand1(ref CMD, ref InnerPort, ref para1, ref para2);
            if (nRet == 62171) return 0; else if (nRet == 62172) return 1; else return -1;
        }

        #region DLLIMPORT
        [DllImport("SS/RdCard.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int UCommand1(ref byte pCmd, ref int parg0, ref int parg1, ref int parg2);
        #endregion
    }
}
