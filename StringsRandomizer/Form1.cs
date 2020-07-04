using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace StringsRandomizer
{
    public partial class Form1 : Form
    {
        private Random rnd = new Random();
        private List<List<int>> usedStrings = new List<List<int>>();
        private List<List<string>> stringsFromFiles = new List<List<string>>();
        private const int COUNT_FILES = 7;
        private const int COUNT_LINES = 20;
        #region The constructor of form.
        public Form1()
        {
            this.MaximizeBox = false;
            for (int i = 0; i < COUNT_FILES; i++) usedStrings.Add(new List<int>());
            InitializeComponent();
        }
        #endregion
        #region Functions that show information to the user.
        private void showError(string text, string nameOfError)
        {
            MessageBox.Show(text, nameOfError, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
        }

        private void showInfoLoad(string text)
        {
            infoLoad.Text = text;
        }

        private void showInfoGeneration(string text)
        {
            infoGeneration.Text = text;
        }
        #endregion
        #region Function that configure standart settings for a search of file in a directory.
        private void standartFileSettings(ref OpenFileDialog file, string filter, int filterIndex, bool restoreDirectory)
        {
            file.Filter = filter;
            file.FilterIndex = filterIndex;
            file.RestoreDirectory = restoreDirectory;
        }
        #endregion
        #region Function that check a format of all files in the directory.
        private bool checkFormat(ref string[] files)
        {
            for (int i = 0; i < files.Length; i++)
            {
                FileInfo file = new FileInfo(files[i]);
                if (file.Name.Split('.').Last<string>() != "txt") return false;
            }
            return true;
        }
        #endregion
        #region Function that load a datas from an input files.
        private void loadFiles(object sender, EventArgs e)
        {
            stringsFromFiles.Clear();

            if(!Directory.Exists(@"Input files"))
            {
                showError("Создайте папку \"Input files\" в корне папки с программой и закиньте туда все входные файлы", "Ошибка загрузки!");
                showInfoLoad("Данные файлов не загружены!");
                return;
            }

            string[] namesOfFiles = Directory.GetFiles(@"Input files");

            if (!checkFormat(ref namesOfFiles))
            {
                showError("В папке \"Input Files\" присутствуют файлы не .txt формата!", "Ошибка загрузки!");
                showInfoLoad("Данные файлов не загружены!");
                return;
            }

            if (namesOfFiles.Length > COUNT_FILES || namesOfFiles.Length < COUNT_FILES)
            {
                showError("Входных файлов должно быть "+ COUNT_FILES +" штук!", "Ошибка загрузки!");
                showInfoLoad("Данные файлов не загружены!");
                return;
            }

            for(int i = 0; i < namesOfFiles.Length; i++)
            {
                stringsFromFiles.Add(new List<string>(File.ReadAllLines(namesOfFiles[i], Encoding.Default)));
                int countOfLines = stringsFromFiles.Last<List<string>>().Count;
                if (countOfLines < COUNT_LINES || countOfLines > COUNT_LINES)
                {
                    showError("В файле \"" + namesOfFiles[i] + "\" записано(-а) " + countOfLines + " строк(-а)", "Ошибка загрузки!");
                    showInfoLoad("Данные файлов не загружены!");
                    stringsFromFiles.Clear();
                    return;
                }
            }

            showInfoLoad("Данные файлов успешно загружены!");
        }
        #endregion
        #region Function that generate a random number of line from the input file.
        private int genRandNumbOfLine(int fileIndex)
        {
            if (usedStrings[fileIndex].Count == COUNT_LINES) usedStrings[fileIndex].Clear();

            int randomNumb;
            do{
                randomNumb = rnd.Next(COUNT_LINES - 1);
            }while(usedStrings[fileIndex].Contains(randomNumb));
            usedStrings[fileIndex].Add(randomNumb);

            return randomNumb;
        }
        #endregion
        #region Function that generate a result of program work.
        private void generateResult(object sender, EventArgs e)
        {
            StreamWriter threadResult = new StreamWriter(File.Create(@"Output file\Результат.txt"));

            for (int i = 0; i < COUNT_FILES; i++)
            {
                int numberOfString = genRandNumbOfLine(i);
                string randomString = stringsFromFiles[i][numberOfString];

                threadResult.WriteLine(randomString);
            }

            threadResult.Close();
            showInfoGeneration("Файл успешно сгенерирован!");
        }
    }
        #endregion
}
