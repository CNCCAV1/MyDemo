using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            string str = "sadasda-13245-哈拉家带口";
            string xx = str.Split('-')[0];
            Console.WriteLine(xx);
        }
    }
}
