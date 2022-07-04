using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RL
{
    public class RWclass
    {

        public static string[] ReadScript(string nameScript)
        {
            // string[] textScript;
            string path = nameScript;//$"{MainWindow.resourceAdress}\\Scripts\\{nameScript}";
            string[] data;
            using (StreamReader sr = new StreamReader(path))
            {
                data = sr.ReadToEnd().Split(new char[] { '$' });
                //textScript = data.Split(new char[] { '\n' });
                //Console.WriteLine(await sr.ReadToEndAsync());
            }
            //textScript = textScript;
            return data;
            //mw.Sc
        }

        public static string[] AllScript(string fold) // поиск среди файлов с параметрами файла с нужным названием
        {
            string[] files = Directory.GetFiles($"{MainWindow.resourceAdress}\\Scripts\\{fold}\\");

            //for(int i = 0; i  < files.Length; i++)
            //{
            //    string[] mas = files[i].Split('\\');
            //    files[i] = mas[mas.Length-1];
            //    Console.WriteLine("ok");
            //}

            return files;
        }

        public static void WriteReport(string path, string info)
        {
            string fullPath = $"{MainWindow.resourceAdress}\\Отчеты\\{DateTime.Now.ToString("dd.MM.yyyy_hh_mm")} - {path}.csv";
            //string info = "";
            using (var sw = new StreamWriter(fullPath, false, Encoding.UTF8))
            {
                sw.WriteLine(info);
                sw.Close();
            }

        }

        public static void WriteReportExcel(string path, byte[] reportExcel)
        {
            string fullPath = $"{MainWindow.resourceAdress}\\Отчеты\\{DateTime.Now.ToString("dd.MM.yyyy_hh_mm")} - {path}.xlsx";
            //File.WriteAllBytes(fullPath, reportExcel);
        }

        public static void SaveAsReport(string path, byte[] reportExcel)  //object sender, RoutedEventArgs e
        {

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = $"{path}.xlsx"; // Default file name
            dlg.DefaultExt = ".xlsx"; // Default file extension
            dlg.Filter = "Document (.xlsx)|*.xlsx"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                File.WriteAllBytes(dlg.FileName, reportExcel);

            }
        }

    }
}
