using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            //uint u = 48;
            //string x = Convert.ToString(u);
            //char c = Convert.ToChar(u);
            //Console.WriteLine("string = " + x);
            //Console.WriteLine("char = " + c);
            //Console.ReadKey();
            Func<int, bool> f1 = Test;
            Console.WriteLine(f1(8));
            Func<int, bool> f2 = i => i > 5;
            Console.WriteLine(f2(2));
            Lazy<People> people = new Lazy<People>();
            people.Value.Name = "SB";
            int m = 0;
            Test(out m);
            Console.WriteLine("Test:" + m);
            Console.WriteLine("值m：" + m);
            Console.WriteLine("Test1:" + Test1(ref m));
            Console.WriteLine("值m：" + m);
            object sex = EnumTypeExtention.GetEnumByName<Sex>("男");
            Console.ReadKey();
        }
        public static int Test1(ref int i)
        {
            return i += 5;
        }
        public static void Test(out int i)
        {
            double j = 0;
            i = 10;
            i= i * 2 -3;
        }
        public static bool Test(int i)
        {
            return i > 5;
        }
    }
    public class People
    {
        public People()
        {
            Name = "XX";
            Sex = 1;
            Console.WriteLine("姓名：" + Name);
        }
        public string Name { get; set; }
        public int Sex { get; set; }
    }
    public class Demo : ISelectFunc<People>
    {
        public People Get(int num)
        {
            throw new NotImplementedException();
        }
    }
    public enum Sex
    {
        [Display(Name ="男")]
        Man,
        [Display(Name = "女")]
        WoMan
    }
}
