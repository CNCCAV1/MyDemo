using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    /// <summary>
    /// 身份证生物特征属性
    /// </summary>
    [Flags]
    public enum IDCardBiometrics : uint
    {
        /// <summary>
        /// 不需要生物特征属性
        /// </summary>
        None = 0,

        /// <summary>
        /// 需要获取照片数据
        /// </summary>
        Photo = 1,

        /// <summary>
        /// 需要获取指纹数据
        /// </summary>
        Fingerprint = 2
    }
}
