using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;


namespace Syntactical_analyzer
{
    class Program
    {
   
        static void Main(string[] args)
        {
            //MOV+
            //проверить файл на пустоту
            //функции вывода

            // название переменной+
            // хрень в мул поп+
            // запятая после поп pop lk,oo pop lk,+
            // левое название команды+-(pop mul)
            
            
            // рефакторинг
            // рефакторинг строк ошибок

            string path = @"E:\My documents\ОС\testProg.txt";
            string writePath = @"E:\My documents\ОС\resultText.log";
            Analyzer analyz = new Analyzer();

            analyz.ReadFileInList(path);
            //analyz.WriteReadText();

            analyz.DellExcessSpaceAndUpperCase();
            analyz.WriteReadText();
            analyz.CheckAlphabetError();

            if (analyz.CheckErrorList())
            {
                analyz.WriteErrorList();
            }
            else
            {
                analyz.StringAnalize();
                analyz.WriteInFile(writePath);
                analyz.WriteList();
            }
            //////////
            analyz.SynthAnalize();
            analyz.WriteErrorList();
        }
    }
}
