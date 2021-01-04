using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    /// <summary>
    /// 读卡完成事件参数
    /// </summary>
    [DataContract]
    public class ReadCardCompletedEventArgs : EventArgs
    {
        /// <summary>
        /// 姓名，最大字符个数=15
        /// </summary>
        [DataMember]
        public string Name;

        /// <summary>
        /// 性别，1-男 2-女，最大字符个数=1
        /// </summary>
        [DataMember]
        public string Gender;

        /// <summary>
        /// 身份证号码，最大字符个数=18
        /// </summary>
        [DataMember]
        public string IDC;

        /// <summary>
        /// 民族编码，最大字符个数=2
        /// </summary>
        [DataMember]
        public string EthnicCode;

        /// <summary>
        /// 民族名称
        /// </summary>
        [DataMember]
        public string EthnicName;

        /// <summary>
        /// 出生日期（yyyy年MM月dd日），最大字符个数=11
        /// </summary>
        [DataMember]
        public string Birth;

        /// <summary>
        /// 住址，最大字符个数=35
        /// </summary>
        [DataMember]
        public string Address;

        /// <summary>
        /// 签发机关，最大字符个数=15
        /// </summary>
        [DataMember]
        public string IssuingAuthority;

        /// <summary>
        /// 有效期限开始日期（yyyy.MM.dd），最大字符个数=10
        /// </summary>
        [DataMember]
        public string ValidDateStart;

        /// <summary>
        /// 有效期限截止日期（yyyy.MM.dd），最大字符个数=10
        /// </summary>
        [DataMember]
        public string ValidDateEnd;

        /// <summary>
        /// 最新住址，最大字符个数=35
        /// </summary>
        [DataMember]
        public string NewAddress;

        /// <summary>
        /// 是否有指纹数据
        /// </summary>
        [DataMember]
        public bool HasFingerprint;

        /// <summary>
        /// 照片数据
        /// </summary>
        [DataMember]
        public byte[] Photo;

        /// <summary>
        /// 指纹数据
        /// </summary>
        [DataMember]
        public byte[] Fingerprint;

        /// <summary>
        /// 序列化，输出JSON格式字符串
        /// </summary>
        /// <returns>序列化后的JSON格式字符串</returns>
        public override string ToString()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ReadCardCompletedEventArgs));
                ser.WriteObject(ms, this);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="jsonString">序列化后的JSON格式字符串</param>
        /// <returns>反序列化后的ReadCardCompletedEventArgs对象实例</returns>
        public static ReadCardCompletedEventArgs CreateInstance(string jsonString)
        {
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ReadCardCompletedEventArgs));
                return (ReadCardCompletedEventArgs)ser.ReadObject(ms);
            }
        }
    }
}
