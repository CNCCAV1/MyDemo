using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            uint u = 48;
            string x = Convert.ToString(u);
            char c = Convert.ToChar(u);
            Console.WriteLine("string = " + x);
            Console.WriteLine("char = " + c);
            Console.ReadKey();

        }
    }
}
