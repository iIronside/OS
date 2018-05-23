using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Syntactical_analyzer_form_
{
    public partial class Form1 : Form
    {
        SyntacticalAnalyzer1 analyzer;
        string path;
        string fileName;
        List<string> listBuf;

        public Form1()
        {
            InitializeComponent();
            analyzer = new SyntacticalAnalyzer1();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Text files (*.txt)|*.txt";
            if (ofd.ShowDialog() == DialogResult.OK  )
            {
                fileName = ofd.FileName;
                path = ofd.FileName;
                textBox1.Text = path;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            analyzer.SetPath(path);
            analyzer.SetFileName(fileName);

            analyzer.ReadFileInList();

            listBuf = analyzer.GetStrList();
            foreach (string s in listBuf)
            {
                listBox1.Items.Add(s);
            }

            analyzer.DellExcessSpaceAndUpperCase();
            analyzer.CheckAlphabetError();

            // если есть ошибки алфавита, дальше не проверять 
            if (analyzer.CheckErrorList())
            {
                listBuf = analyzer.GetErrorList();
                foreach (string s in listBuf)
                {
                    listBox3.Items.Add(s);
                }
                //analyzer.WriteErrorInFile();
                MessageBox.Show("Обнаружены некоректные символы!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                analyzer.StringAnalize();
                //analyzer.WriteInFile();
                listBuf = analyzer.GetResultText();
                foreach (string s in listBuf)
                {
                    listBox2.Items.Add(s);
                }

                analyzer.SynthAnalize();
                if (analyzer.CheckErrorList())
                {
                    //analyzer.WriteErrorInFile();
                    listBuf = analyzer.GetErrorList();
                    foreach (string s in listBuf)
                    {
                        listBox3.Items.Add(s);
                    }
                }
                else
                {
                    MessageBox.Show("Ошибок нет.", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }
    }
}
