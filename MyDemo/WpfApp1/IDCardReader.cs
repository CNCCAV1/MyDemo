using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WpfApp1
{
    /// <summary>
    /// 二代身份证阅读器抽象类
    /// </summary>
    public abstract class IDCardReader : IDisposable
    {
        // 存储文件名
        protected const string CHMsgFileName = "wz.txt";   // 文字信息文件
        protected const string PHMsgFileName = "zp.wlt";   // 加密照片信息   
        protected const string PhotoFileName = "zp.bmp";   // 照片信息文件

        /// <summary>
        /// 委托声明
        /// </summary>
        /// <param name="sender">事件发送者，为 IDCardReader 对象</param>
        /// <param name="e">事件参数</param>
        public delegate void ReadCardCompleted(object sender, ReadCardCompletedEventArgs e);

        /// <summary>
        /// 读卡结束事件
        /// </summary>
        public event ReadCardCompleted OnReadCardCompleted;

        /// <summary>
        /// 同步控制身份证读卡器
        /// </summary>
        public ManualResetEvent ExternalSyncEvent = null;

        /// <summary>
        /// 是否处于扫描状态
        /// </summary>
        protected volatile bool InnerIsAlive = false;
        public bool IsAlive { get { return InnerIsAlive; } }

        /// <summary>
        /// 端口号，有效的端口范围为[1001,1016]
        /// </summary>
        protected int InnerPort = -1;
        public int Port { get { return InnerPort; } }

        /// <summary>
        /// 是否停止读取身份证
        /// </summary>
        private volatile bool shouldStop = false;

        /// <summary>
        /// 追踪Dispose是否已被调用
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        public IDCardReader()
        {
            if (!Initialize()) throw new Exception("找不到身份证读卡器！");
        }

        /// <summary>
        /// 自动检测身份证读卡器并初始化
        /// </summary>
        /// <returns>
        ///     true：成功
        ///     false：失败
        /// </returns>
        protected abstract bool Initialize();

        /// <summary>
        /// 释放托管资源和非托管资源，等同于Dispose()
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        /// <summary>
        /// 释放托管资源和非托管资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 如果disposing为true，则释放托管资源和非托管资源，否则只释放非托管资源
        /// </summary>
        /// <param name="disposing">是否释放托管资源</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // 释放托管资源
                }

                // 停止身份证扫描服务
                if (IsAlive) Stop();

                // 关闭端口
                if (InnerPort != -1)
                {
                    CloseComm();
                }

                disposed = true;
            }
        }

        /// <summary>
        /// 关闭阅读器
        /// </summary>
        /// <returns>
        ///     0：成功
        ///     -1：失败
        /// </returns>
        protected abstract int CloseComm();

        /// <summary>
        /// 开启身份证扫描
        /// </summary>
        /// <param name="wanted">需要的生物特征数据</param>
        /// <returns>
        ///     0：读卡服务启动成功
        ///     1：读卡服务已经启动
        ///     -1：读卡服务启动失败
        /// </returns>
        public int Start(IDCardBiometrics wanted = IDCardBiometrics.None)
        {
            if (InnerIsAlive)
            {   // 服务已经启动
                return 1;
            }
            else
            {
                shouldStop = false;
                bool isOK = ThreadPool.QueueUserWorkItem(CardReaderCallback, wanted);
                if (isOK) return 0; else return -1;
            }
        }

        /// <summary>
        /// 停止读卡
        /// </summary>
        public void Stop()
        {
            shouldStop = true;
            if (ExternalSyncEvent != null) ExternalSyncEvent.Set();
        }

        /// <summary>
        /// 线程池回调方法
        /// </summary>
        /// <param name="state">回调方法要使用的信息对象</param>
        private void CardReaderCallback(Object state)
        {
            InnerIsAlive = true;
            try
            {
                // 读取身份证
                while (true)
                {
                    if (shouldStop) break;

                    //  等待外部同步信号
                    if (ExternalSyncEvent != null)
                    {
                        ExternalSyncEvent.WaitOne();
                        if (shouldStop) break;
                    }

                    // 验证卡
                    if (Authenticate() >= 0)
                    {   // 读基本信息
                        if (ReadContent() >= 0)
                        {   // 存储身份证信息
                            ReadCardCompletedEventArgs Args = new ReadCardCompletedEventArgs();

                            if (File.Exists(CHMsgFileName))
                            {   // 文字信息  
                                using (StreamReader sr = new StreamReader(CHMsgFileName, Encoding.Unicode, true))
                                {   // 提取数据
                                    Retrieve(sr.ReadToEnd(), Args);
                                }
                            }

                            // 照片信息
                            if (((IDCardBiometrics)state & IDCardBiometrics.Photo) == IDCardBiometrics.Photo)
                            {
                                if (File.Exists(PhotoFileName))
                                {
                                    using (FileStream fs = new FileStream(PhotoFileName, FileMode.Open, FileAccess.Read))
                                    {
                                        Args.Photo = new byte[fs.Length];
                                        fs.Read(Args.Photo, 0, (int)fs.Length);
                                    }
                                }
                            }

                            // 关闭读卡操作，等待下次信号
                            if (ExternalSyncEvent != null)
                            {
                                ExternalSyncEvent.Reset();
                                if (shouldStop) break;
                            }

                            // 处理读卡结束事件
                            if (OnReadCardCompleted != null)
                            {
                                OnReadCardCompleted(this, Args);
                            }
                        }
                    }
                } // End While    
            }
            catch (Exception exception)
            {
                // 阻止异常抛出
            }
            finally
            {
                InnerIsAlive = false;
            }
        }

        /// <summary>
        /// 验证卡
        /// </summary>
        /// <returns>
        ///     >=0：验卡成功
        /// </returns>
        protected abstract int Authenticate();

        /// <summary>
        /// 读卡基本信息
        /// </summary>
        /// <returns>
        ///     >=0：成功
        /// </returns>
        protected abstract int ReadContent();

        private void Retrieve(string content, ReadCardCompletedEventArgs e)
        {
            // 提取姓名
            e.Name = content.Substring(0, 15).Trim();

            // 提取性别
            e.Gender = (content[15] == '1') ? "男" : "女";

            // 提取民族编码和民族名称
            e.EthnicCode = content.Substring(16, 2);
            e.EthnicName = Ethnics.Contains(e.EthnicCode) ? (string)Ethnics[e.EthnicCode] : string.Empty;

            // 获取出生日期
            e.Birth = DateTime.ParseExact(content.Substring(18, 8), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

            // 获取住址
            e.Address = content.Substring(26, 35).Trim();

            // 获取身份证编码
            e.IDC = content.Substring(61, 18).Trim();

            // 获取签发机关
            e.IssuingAuthority = content.Substring(79, 15).Trim();

            // 获取有效期限开始日期
            e.ValidDateStart = DateTime.ParseExact(content.Substring(94, 8), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy.MM.dd");

            // 获取有限期限截止日期
            if (content.Substring(102, 2).Equals("长期"))
            {
                e.ValidDateEnd = "长期";
            }
            else
            {
                e.ValidDateEnd = DateTime.ParseExact(content.Substring(102, 8), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy.MM.dd");
            }
        }

        /// <summary>
        /// 验证身份证号码
        /// </summary>
        /// <param name="IDC">要验证的身份证号码</param>
        /// <param name="Gender">性别 0：不校验 1：男性 2：女性</param>
        /// <returns>
        ///     true：号码校验通过
        ///     false：号码校验失败
        /// </returns>
        /// <remarks>
        /// 参考资料：http://baike.baidu.com/view/5112521.htm
        /// 身份证最后一位是根据前面十七位数字码，按照ISO 7064:1983.MOD 11-2校验码计算出来的
        /// 第17位，奇数表示男性，偶数表示女性
        /// </remarks>
        public static bool VerifyIdCardNumber(string IDC, int Gender = 0)
        {
            const int LENGTH = 18;    // 身份证号码长度18位
            if (string.IsNullOrEmpty(IDC) || IDC.Length != LENGTH) return false;

            try
            {
                // 检测月份 1～12
                int Month = Convert.ToInt32(IDC.Substring(10, 2));
                if (Month < 1 || Month > 12) return false;

                // 检测日期 1～31
                int Day = Convert.ToInt32(IDC.Substring(12, 2));
                if (Day < 1 || Day > 31) return false;

                // 判断性别
                if (Gender == 1)
                {   // 男性，奇数为男
                    if (Convert.ToInt32(IDC.Substring(16, 1)) % 2 == 0) return false;
                }
                else if (Gender == 2)
                {   // 女性，偶数为女
                    if (Convert.ToInt32(IDC.Substring(16, 1)) % 2 != 0) return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            // 验证末位的校验码
            byte[] Weight = { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
            int Sum = 0;
            for (int i = 0; i < LENGTH - 1; i++)
            {
                char ch = IDC[i];
                if (!Char.IsDigit(ch)) return false;    // 是否为数字字符
                Sum += (ch - '0') * Weight[i];
            }

            int Remainder = Sum % 11;
            if (Remainder == 2)
            {
                return Char.ToLower(IDC[LENGTH - 1]).Equals('x');
            }
            else
            {
                return (IDC[LENGTH - 1] - '0') == ((12 - Remainder) % 11);
            }
        }

        /// <summary>
        /// 对身份证号码最后一位进行纠错，并返回正确的身份证号码
        /// </summary>
        /// <param name="IDC">要纠错的身份证号码</param>
        /// <returns>纠错后的身份证号码</returns>
        public static string CorrectIDC(string IDC)
        {
            const int LENGTH = 18;    // 身份证号码长度18位
            if (string.IsNullOrEmpty(IDC) || IDC.Length != LENGTH) return null;

            // 验证末位的校验码
            byte[] Weight = { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
            int Sum = 0;
            for (int i = 0; i < LENGTH - 1; i++)
            {
                char ch = IDC[i];
                if (!Char.IsDigit(ch)) return null;    // 是否为数字字符
                Sum += (ch - '0') * Weight[i];
            }

            int Remainder = Sum % 11;
            if (Remainder == 2)
            {
                return IDC.Substring(0, 17) + "X";
            }
            else
            {
                return IDC.Substring(0, 17) + ((12 - Remainder) % 11).ToString();
            }
        }

        #region Ethnic
        /// <summary>
        /// 民族编号对照表
        /// </summary>
        public static readonly SortedList Ethnics = new SortedList() {
            {"01", "汉族"},
            {"02", "蒙古族"},
            {"03", "回族"},
            {"04", "藏族"},
            {"05", "维吾尔族"},
            {"06", "苗族"},
            {"07", "彝族"},
            {"08", "壮族"},
            {"09", "布依族"},
            {"10", "朝鲜族"},
            {"11", "满族"},
            {"12", "侗族"},
            {"13", "瑶族"},
            {"14", "白族"},
            {"15", "土家族"},
            {"16", "哈尼族"},
            {"17", "哈萨克族"},
            {"18", "傣族"},
            {"19", "黎族"},
            {"20", "傈僳族"},
            {"21", "佤族"},
            {"22", "畲族"},
            {"23", "高山族"},
            {"24", "拉祜族"},
            {"25", "水族"},
            {"26", "东乡族"},
            {"27", "纳西族"},
            {"28", "景颇族"},
            {"29", "柯尔克孜族"},
            {"30", "土族"},
            {"31", "达翰尔族"},
            {"32", "仫佬族"},
            {"33", "羌族"},
            {"34", "布朗族"},
            {"35", "撒拉族"},
            {"36", "毛南族"},
            {"37", "仡佬族"},
            {"38", "锡伯族"},
            {"39", "阿昌族"},
            {"40", "普米族"},
            {"41", "塔吉克族"},
            {"42", "怒族"},
            {"43", "乌孜别克族"},
            {"44", "俄罗斯族"},
            {"45", "鄂温克族"},
            {"46", "德昂族"},
            {"47", "保安族"},
            {"48", "裕固族"},
            {"49", "京族"},
            {"50", "塔塔尔族"},
            {"51", "独龙族"},
            {"52", "鄂伦春族"},
            {"53", "赫哲族"},
            {"54", "门巴族"},
            {"55", "珞巴族"},
            {"56", "基诺族"},
            {"81", "穿青人"},
            {"97", "其他"},
            {"98", "外国血统中国籍人士"}
        };
        #endregion
    }
}
