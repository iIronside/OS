using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Syntactical_analyzer
{
    class Analyzer
    {

        char[] alphabet = {'@', '&', '$', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I',
                           'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U',
                           'V', 'W', 'X', 'Y', 'Z', 'H', ' ', ',', '"', '0', '1', '2',
                           '3', '4', '5', '6', '7', '8', '9', '_', '?'};

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
            {"CS", "10"},
        };

        List<string> strList = new List<string>();
        List<string> errorList = new List<string>();
        List<string> resultText = new List<string>();
        
        int indexStr = 1;

        public void ReadFileInList(string path)
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
                Console.WriteLine("Ошибка111: " + ex.Message);
            }
            finally
            {
                if (readFile != null)
                    readFile.Close();
            }
        }


        public void DellExcessSpaceAndUpperCase()
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

        public void CheckAlphabetError()
        {
            for (int i = 0; i < strList.Count; ++i)
            {
                foreach (char ch in strList[i])
                {
                    if (!alphabet.Contains(ch))
                    {
                        errorList.Add("Неопределённый символ. Строка - " + (i + 1).ToString() + " Элемент - " + ch);
                    }
                }
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

        public void WriteInFile(string path)
        {
            StreamWriter writeFile = null;
            try
            {
                writeFile = File.CreateText(path);

                for (int i = 0; i < resultText.Count; ++i)
                {
                    writeFile.WriteLine(resultText[i]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
            finally
            {
                if (writeFile != null)
                    writeFile.Close();
            }
        }

        public bool CheckErrorList()
        {
            if (errorList.Any())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void WriteErrorInList(string errorStr, int integer)
        {
            errorList.Add(errorStr + " Строка - " + resultText[integer].Split(' ')[0]);
        }

        public void CheckInteger(int i)
        {
            if (resultText[i + 1].Contains(" 2 ") || resultText[i + 1].Contains(" 3 "))
            {
                if (!resultText[i + 2].Contains(" 11 "))
                {
                    WriteErrorInList("Неверное значение переменной.", i);
                }
            }
            else
            {
                WriteErrorInList("Неверный тип переменной.", i);
            }
        }

        public void CheckMUL(int i)
        {
            if (!(resultText[i + 1].Contains(" 12 ") || resultText[i + 1].Contains(" 8 ") || resultText[i + 1].Contains(" 7 ")))
            {
                WriteErrorInList("MUL работает только с регистрами общего назначения и областью памяти.", i);
            }
        }

        public void CheckPOP(int i)
        {
            if (resultText[i + 1].Contains(" 12 "))
            {
                if (!CheckAlreadyDW(i+1))
                {
                    WriteErrorInList("Тип ячейки неверен для POP.", i);
                }
            }
            else if (!(resultText[i + 1].Contains(" 9 ") || resultText[i + 1].Contains(" 8 ")))
            {
                WriteErrorInList("POP восстанавливает значение в регистр, ячейку памяти или сегментный регистр.", i);
            }
        }

        public void CheckMOV(int i)
        {    // регистр DB
            if (resultText[i + 1].Contains(" 7 "))
            {
                if (resultText[i + 3].Contains(" 12 "))
                {
                    if (!CheckAlreadyDB(i+3))
                    {
                        WriteErrorInList("Тип ячейки неверен.", i);
                    }
                }
                else if (!(resultText[i + 3].Contains(" 7 ") || resultText[i + 3].Contains(" 11 ")))
                {
                    WriteErrorInList("Неверный тип источника.", i);
                }
            }// регистр DW
            else if (resultText[i + 1].Contains(" 8 ") || resultText[i + 1].Contains(" 9 ") || resultText[i + 1].Contains(" 10 "))
            {
                if (resultText[i + 3].Contains(" 12 "))
                {
                    if (!CheckAlreadyDW(i + 3))
                    {
                        WriteErrorInList("Тип ячейки не верен.", i);
                    }
                }
                else if (!(resultText[i + 3].Contains(" 8 ") || resultText[i + 1].Contains(" 9 ") || resultText[i + 1].Contains(" 10 ") || resultText[i + 3].Contains(" 11 ")))
                {
                    WriteErrorInList("Неверный тип источника.", i);
                }
            }// ячейка памяти
            else if (resultText[i + 1].Contains(" 12 "))
            {
                
                if(CheckAlreadyDW(i + 1))
                {
                    if (!(resultText[i + 3].Contains(" 8 ") || resultText[i + 3].Contains(" 9 ") || resultText[i + 3].Contains(" 10 ") || resultText[i + 3].Contains(" 11 ")))
                    {
                        WriteErrorInList("Неверный тип источника.", i);
                    }
                }
                else if (CheckAlreadyDB(i + 1))
                {
                    Console.WriteLine(22222);
                    if (!(resultText[i + 3].Contains(" 7 ") || resultText[i + 3].Contains(" 11 ")))
                    {
                        WriteErrorInList("Неверный тип источника1.", i);
                    }
                }
                else
                {
                    WriteErrorInList("Неверный тип источника3.", i);
                }
            }
            else
            {
                WriteErrorInList("Неверный приёмник.", i);
            }
        }

        public bool CheckAlreadyDB(int n)
        {
            string varName = resultText[n].Split(' ')[2];
            for (int i = 0; i < resultText.Count - 1; ++i)
            {
                if (resultText[i].Contains(varName))
                {
                    if (resultText[i + 1].Contains(" 2 "))
                    {
                        return true;
                    }
                }
            }
            return false;
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
                    if (WrongCommandSize(i) < 3)
                    {
                        CheckInteger(i);
                    }
                    else
                    {
                        WriteErrorInList("Неизвестная команда.", i);
                    }
                    i += WrongCommandSize(i);
                }
                else if (resultText[i].Contains(" 4 ")) // MOV
                {
                    if (WrongCommandSize(i) <= 4)
                    {
                        CheckMOV(i); 
                    }
                    else
                    {
                        WriteErrorInList("Превышение количества операторов для команды MOV.", i);
                    }
                    i += WrongCommandSize(i);
                }
                else if (resultText[i].Contains(" 5 ")) // MUL
                {
                    if (WrongCommandSize(i) < 2)
                    {
                        CheckMUL(i);
                    }
                    else
                    {
                        WriteErrorInList("Превышение количества операторов для команды MUL.", i);
                    }
                    i += WrongCommandSize(i);
                }
                else if (resultText[i].Contains(" 6 ")) // POP
                {
                    if (WrongCommandSize(i) < 2)
                    {
                        CheckPOP(i);
                    }
                    else
                    {
                        WriteErrorInList("Превышение количества операторов для команды POP.", i);
                    }
                    i += WrongCommandSize(i);
                }
                else
                {
                    WriteErrorInList("Неверное объявление команды или переменной.", i);
                    i += WrongCommandSize(i);
                }
            } 
        }

        public int WrongCommandSize(int integer)
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

        public void WriteReadText()
        {
            if (strList.Any())
            {
                foreach (string s in strList)
                {
                    Console.WriteLine(s);
                }
            }
            else
            {
                Console.WriteLine("Список пуст.");
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

        public void WriteErrorList()
        {
            if (errorList.Any())
            {
                foreach (string s in errorList)
                {
                    Console.WriteLine(s);
                }
            }
            else
            {
                Console.WriteLine("Список ошибок пуст.");
            }
        }

    }
}
