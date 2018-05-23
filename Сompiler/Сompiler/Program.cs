using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Сompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"E:\My documents\ОС\ProgTest.txt";

            Compiler compiler = new Compiler(path);

            compiler.ReadFileInList();
            compiler.DellExcessSpaceAddUpCaseAndCommaSpace();


            compiler.StringAnalize();
            compiler.SynthAnalize();

            //compiler.WriteList();
            compiler.WriteTable();

            //compiler.WriteList();
            //string s = "1111011000100110";
            //int code = Convert.ToInt32(s, 2);
            //Console.WriteLine(code);
            //Console.WriteLine(code.ToString("x"));

            //    string hhh = "jhgjhbkjfbH";
            //    hhh = hhh.Remove(hhh.Length-1);
            //    Console.WriteLine(hhh);

            //string code = "101";
            //Console.WriteLine(Convert.ToUInt32(code).ToString("x"));
            //Int16 hex = 0x0000;
            //hex += 0x1a;

            //Console.WriteLine(hex);
        }
    }
}
