using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    /// <summary>
    /// 公司名称：深圳华视电子读写设备有限公司
    /// 官方网址：http://www.chinaidcard.com/
    /// 产品名称：台式居民身份证阅读机具
    /// 产品型号：CVR-100U 
    /// </summary>
    public class ChinaVisionReader : IDCardReader
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
            for (int i = 1; i <= 16; i++)
            {
                if (CVR_InitComm(i) == 1)
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
            if (CVR_CloseComm() == 1) return 0; else return -1;
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
            if (CVR_Authenticate() == 1) return 0; else return -1;
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
            if (CVR_Read_FPContent() == 1) return 0; else return -1;
        }

        #region DLLIMPORT
        [DllImport("HS/termb.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern int CVR_InitComm(int Port);

        [DllImport("HS/termb.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern int CVR_Authenticate();

        [DllImport("HS/termb.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern int CVR_Read_FPContent();

        [DllImport("HS/termb.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern int CVR_CloseComm();
        #endregion
    }
}
