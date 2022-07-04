using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RL
{

    public class Reporter
    {
        public ReportForm GetReport(string[] headData, string data) // передаем массив строк с названиями и данные
        {
            var rowData = data.Split('\n');
            int row = rowData.Length;
            int column = headData.Length;
            double[,] sortData = new double[row, column];  // в массиве кол-во столбцов равно количеству колон в экселе, количество строк равно кол-ву провереденных опытов
            int j = 0;
            foreach(string str in rowData)
            {
                if (str == "") break;
                var tmpData = str.Split(';');
                for (int i = 0; i < column; i++)
                {
                    sortData[j,i] = Convert.ToDouble(tmpData[i]);
                }
                j++;

            }
            row = j;

            return new ReportForm
            {
                Row = row,
                Column = column,
                
                Head = new HeadTable
                {
                    Params = headData
                },
                Experiment = new ExperimentItem
                {
                    ParamsD = sortData
                  
                }
            };
        }
    }


    public class ReportForm
    {
        public int Row { set; get; }
        public int Column { set; get; }
        public HeadTable Head { set; get; }
        public ExperimentItem Experiment { set; get; }
    }

    public class HeadTable
    {
        public string NameExp { set; get; }
        //public string Step { set; get; }
        public string[] Params { set; get; }  //Ячейки для записи данных об экспериментах. В этом классе помещаются заголовки
        //public string Param2 { set; get; }
        //public string Param3 { set; get; }
        //public string Param4 { set; get; }
        //public string Param5 { set; get; }
        //public string Param6 { set; get; }
        //
    }

    public class ExperimentItem  // сюда помещаем данные. Не обязательно помещать все данные, главное укладывать их в соответствии с именами столбцов
    {
        //public int Step { set; get; }
        public double[,] ParamsD { set; get; } // записываем в каждый массив данные. Один массив, одна строка
        //public double Param2d { set; get; }
        //public double Param3d { set; get; }
        //public double Param4d { set; get; }
        //public double Param5d { set; get; }
        //public double Param6d { set; get; }

        public DateTime Date { set; get; }
    }
}
