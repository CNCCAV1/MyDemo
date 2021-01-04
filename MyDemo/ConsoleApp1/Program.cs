//using System;

//namespace ConsoleApp1
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {
//            //string str = "sadasda-13245-哈拉家带口";
//            //string xx = str.Split('-')[0];
//            //Console.WriteLine(xx);
//            uint u = 48;
//            string x = Convert.ToString(u);
//            char c = Convert.ToChar(u);
//            Console.WriteLine("string = " + x);
//            Console.WriteLine("char = " + c);
//            Console.ReadKey();
//        }
//    }
//}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;
using System.Collections;
using System.IO;

namespace ResExport
{
    class Program
    {
        static void Main(string[] args)
        {
            ResourceReader res = new ResourceReader("Resources.resources");//该文件放到bin
            IDictionaryEnumerator dics = res.GetEnumerator();
            while (dics.MoveNext())
            {
                Stream s = (Stream)dics.Value;
                int fileSize = (int)s.Length;
                byte[] fileContent = new byte[fileSize];
                s.Read(fileContent, 0, fileSize);
                FileStream fs;
                string filepath = dics.Key.ToString();
                filepath = Path.Combine("C://", filepath); //保存到指定目录
                filepath = Path.GetFullPath(filepath);
                var p = Path.GetDirectoryName(filepath);//要创建的目录
                if (!Directory.Exists(p))
                {
                    Directory.CreateDirectory(p);
                }

                FileInfo fi = new System.IO.FileInfo(filepath);
                fs = fi.OpenWrite();
                fs.Write(fileContent, 0, fileSize);
                fs.Close();
            }

            res.Close();

        }
    }
}
