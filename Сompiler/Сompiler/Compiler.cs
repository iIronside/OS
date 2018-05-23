using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Сompiler
{
    class Compiler
    {
        int indexStr = 1;

        string path;

        List<string> strList = new List<string>();
        List<string> resultText = new List<string>();


        Dictionary<string, string> tokenDict = new Dictionary<string, string>()
        {
            {",", "1"},
            {"DB", "2"},
            {"DW", "3"},
            {"MOV", "4"},
            {"MUL", "5"},
            {"POP", "6"},
            {"AL", "7"},
            {"AH", "7"},
            {"BL", "7"},
            {"BH", "7"},
            {"CL", "7"},
            {"CH", "7"},
            {"DL", "7"},
            {"DH", "7"},
            {"AX", "8"},
            {"BX", "8"},
            {"CX", "8"},
            {"DX", "8"},
            {"DS", "9"},
            {"SS", "9"},
            {"ES", "9"},
            {"CS", "10"}
        };

        Dictionary<string, string> registerСodes = new Dictionary<string, string>()
        {
            {"AL", "000"},
            {"CL", "001"},
            {"DL", "010"},
            {"BL", "011"},
            {"AH", "100"},
            {"CH", "101"},
            {"DH", "110"},
            {"BH", "111"},
            {"AX", "000"},
            {"CX", "001"},
            {"DX", "010"},
            {"BX", "011"},
            {"SP", "100"},
            {"BP", "101"},
            {"SI", "110"},
            {"DI", "111"},
            {"ES", "00"},
            {"CS", "01"},
            {"SS", "10"},
            {"DS", "11"}
        };
        //////////////////////////////
        private int address;
        List<string> resultTable = new List<string>();

        private static string mulInBinCode = "1111011";

        string strTab = " ";

        ///////////////////////
        public Compiler(string filePath)
        {
            path = filePath;

            address = 0;
        }

        public void ReadFileInList()
        {
            StreamReader readFile = null;
            try
            {
                readFile = new StreamReader(path);

                while (!readFile.EndOfStream)
                {
                    strList.Add(readFile.ReadLine());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
            finally
            {
                if (readFile != null)
                    readFile.Close();
            }
        }

        public void StringAnalize()
        {
            string bufer;
            string[] arrStrBuf;
            foreach (string s in strList)
            {
                arrStrBuf = s.Split(' ');

                foreach (string substring in arrStrBuf)
                {
                    bufer = WordAnalize(substring); // вызов фун. анализа слова
                    resultText.Add(indexStr + " " + bufer + " " + substring);
                }
                ++indexStr;
            }
        }

        public string WordAnalize(string str)
        {
            if (tokenDict.TryGetValue(str, out string value))
            {
                return value;
            }
            else if (int.TryParse(str, out int number)) // целое число
            {
                return "11";
            }
            else if (Char.IsNumber(str[0]) && str[str.Length - 1] == 'H') // целое число для 16-сис
            {
                return "11";
            }
            else // Идентификатор 
            {
                return "12";
            }
        }

        public void WriteList()
        {
            if (resultText.Any())
            {
                foreach (string s in resultText)
                {
                    Console.WriteLine(s);
                }
            }
            else
            {
                Console.WriteLine("Список пуст.");
            }
        }

        public void WriteTable()
        {
            if (resultTable.Any())
            {
                foreach (string s in resultTable)
                {
                    Console.WriteLine(s);
                }
            }
            else
            {
                Console.WriteLine("Список пуст.");
            }
        }

        public void DellExcessSpaceAddUpCaseAndCommaSpace()
        {
            for (int i = 0; i < strList.Count; ++i)
            {
                if (String.IsNullOrWhiteSpace(strList[i])) //Удаление пустых строк
                {
                    strList.RemoveAt(i);
                    --i;
                }
                else
                {
                    strList[i] = strList[i].Replace("\t", " ");
                    strList[i] = strList[i].Replace(",", " , ");
                    strList[i] = strList[i].ToUpper();
                    strList[i] = System.Text.RegularExpressions.Regex.Replace(strList[i], @"\s+", " ");
                    strList[i] = strList[i].Trim();
                }
            }
        }

        public void ProcessInteger(int i)
        {
            string tmp;

            tmp = resultText[i].Split(' ')[0] + strTab; // номер строки
            tmp += address.ToString("X").PadLeft(4,'0') + strTab;

            string integerValue = resultText[i + 2].Split(' ')[2];
            if (integerValue.EndsWith("H"))
            {
                tmp += integerValue.Remove(integerValue.Length - 1) + strTab;
            }
            else // конвертировать десятичные
            {
                tmp += Convert.ToUInt32(integerValue).ToString("x") + strTab;
            }

            tmp += FindString(i);

            resultTable.Add(tmp);

            if (CheckAlreadyDW(i))
            {
                address += 2;
            }
            else
            {
                address += 1;
            }
        }

        public int CommandSize(int integer)
        {
            int size = 0;
            string checkStr = resultText[integer].Split(' ')[0];
            for (int i = ++integer; i < resultText.Count; ++i)
            {
                if (resultText[i].StartsWith(checkStr))
                {
                    ++size;
                }
                else
                {
                    return size;
                }
            }
            return size;
        }

        public void ProcessPOP(int i)
        {
            string tmp;
            string popCommand = "";

            tmp = resultText[i].Split(' ')[0] + strTab; // номер строки
            tmp += address.ToString("X").PadLeft(4, '0') + strTab;

             

            if (resultText[i + 1].Contains(" 8 "))
            {
                ++address;

                popCommand = "01011";

                registerСodes.TryGetValue(resultText[i + 1].Split(' ')[2], out string value);
                popCommand += value; // reg

                tmp += Convert.ToInt32(popCommand, 2).ToString("x") + strTab;
            }
            else if(resultText[i + 1].Contains(" 9 ") || resultText[i + 1].Contains(" 10 "))
            {
                ++address;

                popCommand = "000";

                registerСodes.TryGetValue(resultText[i + 1].Split(' ')[2], out string value);
                popCommand += value;
                popCommand += "111";

                tmp += Convert.ToInt32(popCommand, 2).ToString("x") + strTab;
            }
            else // if (resultText[i + 1].Contains(" 12 ")) память
            {
                address += 4;

                popCommand = "1000111100000110"; // абсолютный адрес

                tmp += Convert.ToInt32(popCommand, 2).ToString("x") + FindIntegerAdress(resultText[i + 1].Split(' ')[2]) + strTab;
            }
            tmp += FindString(i);

            resultTable.Add(tmp);
        }

        private void ProcessMOV(int i)
        {
            string tmp;
            string movCommand = "";

            tmp = resultText[i].Split(' ')[0] + strTab; // номер строки
            tmp += address.ToString("X").PadLeft(4, '0') + strTab;

            if (resultText[i + 3].Contains(" 9 ") || resultText[i + 3].Contains(" 10 ")) // Сегментный регистр в регистр/память
            {
                movCommand = "10001100";

                registerСodes.TryGetValue(resultText[i + 3].Split(' ')[2], out string value);
                value += "110"; // reg
                if (resultText[i + 1].Contains(" 12 ")) // в память
                {
                    movCommand += "000";
                    movCommand += value;
                    movCommand = Convert.ToInt32(movCommand, 2).ToString("x");
                    movCommand += FindIntegerAdress(resultText[i+1].Split(' ')[2]);
                }
                else // в регистр
                {
                    movCommand += "110";
                    movCommand += value;
                    movCommand = Convert.ToInt32(movCommand, 2).ToString("x");
                }
                tmp += movCommand + strTab;
                tmp += FindString(i);

                address += 4;
                resultTable.Add(tmp);
            }
            else if (resultText[i + 1].Contains(" 9 ") || resultText[i + 1].Contains(" 10 ")) // Регистр/память в сегментный регистр:
            {
                movCommand = "10001110";

                registerСodes.TryGetValue(resultText[i + 1].Split(' ')[2], out string value);
                value += "110"; // reg
                if (resultText[i + 3].Contains(" 12 ")) // из память
                {
                    movCommand += "000";
                    movCommand += value;
                    movCommand = Convert.ToInt32(movCommand, 2).ToString("x");
                    movCommand += FindIntegerAdress(resultText[i + 3].Split(' ')[2]);
                }
                else // из регистра
                {
                    movCommand += "110";
                    movCommand += value;
                    movCommand = Convert.ToInt32(movCommand, 2).ToString("x");
                }
                tmp += movCommand + strTab;
                tmp += FindString(i);

                address += 4;
                resultTable.Add(tmp);
            }
            else if (resultText[i+3].Split(' ')[2] == "AX" || resultText[i + 3].Split(' ')[2] == "AL" && resultText[i + 1].Contains(" 12 ")) // Регистр AX (AL) в память
            {
                movCommand = "1010001";

                if (resultText[i + 3].Contains(" 8 ")) // AX
                {
                    movCommand += "1";
                }
                else // AL
                {
                    movCommand += "0";                   
                }
                movCommand = Convert.ToInt32(movCommand, 2).ToString("x");
                movCommand += FindIntegerAdress(resultText[i + 1].Split(' ')[2]);

                tmp += movCommand + strTab;
                tmp += FindString(i);

                address += 3;
                resultTable.Add(tmp);
            }
            else if (resultText[i + 1].Split(' ')[2] == "AX" || resultText[i + 1].Split(' ')[2] == "AL" && resultText[i + 3].Contains(" 12 ")) // Память в регистр AX (AL)
            {
                movCommand = "1010000";

                if (resultText[i + 1].Contains(" 8 ")) // AX
                {
                    movCommand += "1";
                }
                else // AL
                {
                    movCommand += "0";
                }
                movCommand = Convert.ToInt32(movCommand, 2).ToString("x");
                movCommand += FindIntegerAdress(resultText[i + 3].Split(' ')[2]);

                tmp += movCommand + strTab;
                tmp += FindString(i);

                address += 3;
                resultTable.Add(tmp);
            }
            //else if () // Непосредственное значение в регистр
            //{

            //}
            //else if () // Непосредственное значение в регистр/память
            //{

            //}
            //else // Регистр/память в/из регистр
            //{
            //    movCommand = "1000101";
            //    if (resultText[i + 1].Contains(" 12 "))
            //    {
            //        if (CheckAlreadyDW(i + 1))
            //        {
            //            movCommand += "1";
            //        }
            //        else
            //        {
            //            movCommand += "0";
            //        }
            //        movCommand += "00";
            //    }|100010dw|modregr/m| адресс переменной вместо кода регистра

            //}

        }

        public void ProcessMUL(int i)
        {
            string tmp;

            tmp = resultText[i].Split(' ')[0] + strTab; // номер строки
            tmp += address.ToString("X").PadLeft(4, '0') + strTab;

            string comandCode = mulInBinCode;
            if ((resultText[i + 1].Contains(" 12 ")))
            {
                address += 4;

                if (CheckAlreadyDW(i + 1))
                {
                    comandCode += 1;
                }
                else
                {
                    comandCode += 0;
                }

                tmp += Convert.ToInt32(comandCode, 2).ToString("x") + "26 "; // абсолютный адрес без смещения 26(100110)
                tmp += FindIntegerAdress(resultText[i+1].Split(' ')[2]) + strTab;
                tmp += FindString(i);

                resultTable.Add(tmp);
            }
            else // если регистр
            {
                address += 2;

                if ((resultText[i + 1].Contains(" 8 ")))
                {
                    comandCode += 1;
                }
                else // для однобайтовых регистров
                {
                    comandCode += 0;
                }
                tmp += Convert.ToInt32(comandCode, 2).ToString("x") + " ";

                comandCode = "11100"; // mod
                registerСodes.TryGetValue(resultText[i + 1].Split(' ')[2], out string value);
                comandCode += value;

                tmp += Convert.ToInt32(comandCode, 2).ToString("x") + strTab;
                tmp += FindString(i);

                resultTable.Add(tmp);
            }
        }

        private string FindIntegerAdress(string integerName)
        {
            foreach (var item in resultTable)
            {
                if (item.Contains(integerName))
                {
                    return item.Split(' ')[1];
                }
            }
            return "error";
        }

        private string FindString(int strNum)
        {
            string number = resultText[strNum].Split(' ')[0];

            return strList[Convert.ToInt32(number) - 1];
        }

        public bool CheckAlreadyDW(int n)
        {
            string varName = resultText[n].Split(' ')[2];
            for (int i = 0; i < resultText.Count - 1; ++i)
            {
                if (resultText[i].Contains(varName))
                {
                    if (resultText[i + 1].Contains(" 3 "))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void SynthAnalize()
        {
            int number = resultText.Count;
            for (int i = 0; i < number; ++i)
            {
                if (resultText[i].Contains(" 12 ")) // integer
                {
                    ProcessInteger(i);
                    i += 2;
                }
                else if (resultText[i].Contains(" 4 ")) // MOV
                {
                    ProcessMOV(i);              
                    i += CommandSize(i);
                }
                else if (resultText[i].Contains(" 5 ")) // MUL
                {
                    ProcessMUL(i);
                    i++;
                }
                else if (resultText[i].Contains(" 6 ")) // POP
                {
                    ProcessPOP(i);
                    i += CommandSize(i);
                }
            }
        }
    }
}
