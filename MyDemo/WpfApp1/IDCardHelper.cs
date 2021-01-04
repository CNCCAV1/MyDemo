using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class IDCardHelper
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="Port">Com端口号</param>
        /// <returns></returns>
        [DllImport("Termb.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int CVR_InitComm(int Port);
        /// <summary>
        /// 卡验证
        /// </summary>
        /// <returns></returns>
        [DllImport("Termb.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int CVR_Authenticate();
        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <returns></returns>
        [DllImport("Termb.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int CVR_CloseComm();
        /// <summary>
        /// 获取姓名
        /// </summary>
        [DllImport("Termb.dll", EntryPoint = "GetPeopleName", CharSet = CharSet.Ansi, SetLastError = false)]
        public static extern int GetPeopleName(ref byte strTmp, ref int strLen);
        /// <summary>
        /// 获取民族
        /// </summary>
        [DllImport("Termb.dll", EntryPoint = "GetPeopleNation", CharSet = CharSet.Ansi, SetLastError = false)]
        public static extern int GetPeopleNation(ref byte strTmp, ref int strLen);
        /// <summary>
        /// 获取出生日期
        /// </summary>
        [DllImport("Termb.dll", EntryPoint = "GetPeopleBirthday", CharSet = CharSet.Ansi, SetLastError = false, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetPeopleBirthday(ref byte strTmp, ref int strLen);
        /// <summary>
        /// 获取地址
        /// </summary>
        [DllImport("Termb.dll", EntryPoint = "GetPeopleAddress", CharSet = CharSet.Ansi, SetLastError = false, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetPeopleAddress(ref byte strTmp, ref int strLen);
        /// <summary>
        /// 获取身份证号
        /// </summary>
        [DllImport("Termb.dll", EntryPoint = "GetPeopleIDCodeU", CharSet = CharSet.Ansi, SetLastError = false, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetPeopleIDCodeU(ref byte strTmp, ref int strLen);
        /// <summary>
        /// 获取发证机关
        /// </summary>
        [DllImport("Termb.dll", EntryPoint = "GetDepartment", CharSet = CharSet.Ansi, SetLastError = false, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetDepartment(ref byte strTmp, ref int strLen);
        /// <summary>
        /// 获取有效开始日期
        /// </summary>
        [DllImport("Termb.dll", EntryPoint = "GetStartDate", CharSet = CharSet.Ansi, SetLastError = false, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetStartDate(ref byte strTmp, ref int strLen);
        /// <summary>
        /// 获取有效截止日期
        /// </summary>
        [DllImport("Termb.dll", EntryPoint = "GetEndDate", CharSet = CharSet.Ansi, SetLastError = false, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetEndDate(ref byte strTmp, ref int strLen);
        /// <summary>
        /// 获取性别
        /// </summary>
        [DllImport("Termb.dll", EntryPoint = "GetPeopleSex", CharSet = CharSet.Ansi, SetLastError = false, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetPeopleSex(ref byte strTmp, ref int strLen);
        /// <summary>
        /// 获取安全模块号码
        /// </summary>
        [DllImport("Termb.dll", EntryPoint = "CVR_GetSAMID", CharSet = CharSet.Ansi, SetLastError = false, CallingConvention = CallingConvention.StdCall)]
        public static extern int CVR_GetSAMID(ref byte strTmp);
        /// <summary>
        /// 读取身份证  运行目录下生成wz.txt（文字信息）和zp.bmp（照片信息）
        /// </summary>
        /// <returns></returns>
        [DllImport("Termb.dll", EntryPoint = "CVR_Read_FPContent", CharSet = CharSet.Ansi, SetLastError = false, CallingConvention = CallingConvention.StdCall)]
        public static extern int CVR_Read_FPContent();
        public static CardInfo ReadCardInfo()
        {
            CardInfo card = new CardInfo();
            byte[] name = new byte[30];
            byte[] sex = new byte[30];
            byte[] nation = new byte[30];
            byte[] birthday = new byte[30];
            byte[] address = new byte[300];
            byte[] idcode = new byte[30];
            byte[] department = new byte[300];
            int length = 300;
            IDCardHelper.GetPeopleName(ref name[0], ref length);
            IDCardHelper.GetPeopleSex(ref sex[0], ref length);
            IDCardHelper.GetPeopleNation(ref nation[0], ref length);
            IDCardHelper.GetPeopleBirthday(ref birthday[0], ref length);
            IDCardHelper.GetPeopleAddress(ref address[0], ref length);
            IDCardHelper.GetPeopleIDCodeU(ref idcode[0], ref length);
            IDCardHelper.GetDepartment(ref department[0], ref length);
            card.Name = System.Text.Encoding.GetEncoding("GB2312").GetString(name).Replace("\0", "").Trim();
            card.Sex = System.Text.Encoding.GetEncoding("GB2312").GetString(sex).Replace("\0", "").Trim();
            card.Nation = System.Text.Encoding.GetEncoding("GB2312").GetString(nation).Replace("\0", "").Trim();
            card.Birthday = System.Text.Encoding.GetEncoding("GB2312").GetString(birthday).Replace("\0", "").Trim();
            card.Address = System.Text.Encoding.GetEncoding("GB2312").GetString(address).Replace("\0", "").Trim();
            card.Office = System.Text.Encoding.GetEncoding("GB2312").GetString(department).Replace("\0", "").Trim();

            card.ID = System.Text.Encoding.GetEncoding("GB2312").GetString(idcode).Replace("\0", "").Trim();
            CVR_CloseComm();
            return card;
        }
    }
}
