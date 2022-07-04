using OxyPlot;
using OxyPlot.Series;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace RL
{
    /// <summary>
    /// Логика взаимодействия для CapWindowExperiment.xaml
    /// </summary>
    public partial class CapWindowExperiment : Window
    {

        public static string experiment = "Don't Select";

        

        public CapWindowExperiment(string typeOfExperiments)
        {
            experiment = typeOfExperiments;  // при инициализации полученное значение присваиваем для внутреннего пользования классу
            InitializeComponent();
            //Experiment exp = new Experiment();
            plotModel.Visibility = Visibility.Hidden;  // прописать в ксамле
            Script.Visibility = Visibility.Hidden;   // прописать в ксамле
            
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var nameButton = (sender as Button).Name;
            //** скрываем кнопки и пр. **//
            Button1.Visibility = Visibility.Collapsed;
            Button2.Visibility = Visibility.Collapsed;
            //** открываем интерфейс эксперимента **//
            plotModel.Visibility = Visibility.Visible;
            Script.Visibility = Visibility.Visible;
            Script = new TextBlock { Text = "Hello" };
            
            if (nameButton == "Button1")
            {
                CapWindow.typeOfExperiment = 1;
                
            }
            if (nameButton == "Button2")
            {
                CapWindow.typeOfExperiment = 2;
                
            }
            Experiment.textFromFile = RWclass.ReadScript(RWclass.AllScript(experiment)[CapWindow.typeOfExperiment]);  // текст = чтение(список файлов в папке с типом эксперимента[номер эксперимента])
        }

    }


    public class Experiment: INotifyPropertyChanged, IDisposable
    {
      
        public string textToTextBox { get; set; } = "Тестовое значение";
        public static string[] textFromFile = { " "};

        private static int step = 0;

        public Experiment()
        {
            ExperimentModel = new PlotModel { Title = "Эксперимент!" };
            
            yAxis.Key = "time";
            yAxis.Title = "Время, с";// = "Напряжение, В";
            yAxis.TextColor = OxyColor.FromRgb(0, 0, 0);
            yAxis.MajorGridlineStyle = LineStyle.Dot;

            //CapWindowExperiment.
            xAxis.Key = "Voltage";
           
            xAxis.MajorGridlineStyle = LineStyle.Dot;
            xAxis.Position = OxyPlot.Axes.AxisPosition.Bottom;

            ExperimentModel.Axes.Add(yAxis);
            ExperimentModel.Axes.Add(xAxis);

            resultLine.LineStyle = LineStyle.Solid;
            resultLine.Color = OxyColor.FromRgb(200, 0, 0);
            resultLine.StrokeThickness = 2.0;
            ExperimentModel.Series.Add(resultLine);

            cupVoltLine.LineStyle = LineStyle.Solid;
            cupVoltLine.Color = OxyColor.FromRgb(0, 200, 0);
            cupVoltLine.StrokeThickness = 2.0;
            cupVoltLine.Title = "Конденсатор";
            ExperimentModel.Series.Add(cupVoltLine);

            indVoltLine.LineStyle = LineStyle.Solid;
            indVoltLine.Color = OxyColor.FromRgb(0, 0, 200);
            indVoltLine.StrokeThickness = 2.0;
            indVoltLine.Title = "Катушка";
            ExperimentModel.Series.Add(indVoltLine);

            resVoltLine.LineStyle = LineStyle.Solid;
            resVoltLine.Color = OxyColor.FromRgb(200, 0, 0);
            resVoltLine.StrokeThickness = 2.0;
            resVoltLine.Title = "Резистор";
            ExperimentModel.Series.Add(resVoltLine);

            resultPoint.LineStyle = LineStyle.None;
            resultPoint.MarkerType = MarkerType.Square;
            resultPoint.MarkerSize = 5.0;
            resultPoint.MarkerFill = OxyColor.FromRgb(0, 200, 0);
            ExperimentModel.Series.Add(resultPoint);

            ExperimentModel.InvalidatePlot(true);

            Thread receiveThread = new Thread(new ThreadStart(WorkingWindow));
            receiveThread.Start();

        }

        public static void Destruct()
        {
            ExperimentModel.Axes.Clear();
            ExperimentModel.Series.Clear();
            resultLine.ClearSelection();
            resultPoint.ClearSelection();
        }

        private void WorkingWindow()
        {
            int i = 0;
            while(step < textFromFile.Length)
            {

                textToTextBox = textFromFile[step];//$"{i} ,  {i*i}";
                i++;
                OnPropertyChanged("textToTextBox");
                System.Threading.Thread.Sleep(100);
            }
           
        }

        public static void GetPoint(double xValue, double yValue, string xAxisTitle, string yAxisTitle = null)
        {
            resultLine.Points.Add(new DataPoint(xValue, yValue));
            resultPoint.Points.Add(new DataPoint(xValue, yValue));

            step++;
            xAxis.Title = xAxisTitle;
            if (yAxisTitle == null) yAxis.Title = "Время, с";// = "Напряжение, В";
            else yAxis.Title = yAxisTitle;
            //if (typeExperiment == 1) xAxis.Title = "Емкость конденсатора, Ф";
            //if (typeExperiment == 2) xAxis.Title = "Сопротивление резистора, Ом";
            ExperimentModel.InvalidatePlot(true);
            
        }

        public static void GetThreePoint(double xValue, double yValue, double y2Value, double y3Value)
        {
            resVoltLine.Points.Add(new DataPoint(xValue, yValue));
            cupVoltLine.Points.Add(new DataPoint(xValue, y2Value));
            indVoltLine.Points.Add(new DataPoint(xValue, y3Value));
            

            xAxis.Title = "Частота, кГц";
            yAxis.Title = "Амплитуда, В";

            //xAxis.Title = xAxisTitle;
            //if (yAxisTitle == null) yAxis.Title = "Время, с";// = "Напряжение, В";
            //else yAxis.Title = yAxisTitle;
            //if (typeExperiment == 1) xAxis.Title = "Емкость конденсатора, Ф";
            //if (typeExperiment == 2) xAxis.Title = "Сопротивление резистора, Ом";
            ExperimentModel.InvalidatePlot(true);

        }


        public static void ClearPoint()
        {
            resultLine.Points.Clear();
            resultPoint.Points.Clear();
            ExperimentModel.InvalidatePlot(true);

        }

        public void Dispose()
        {
            Console.WriteLine($" has been disposed");
        }

        public static LineSeries resultLine = new LineSeries();
        public static LineSeries cupVoltLine = new LineSeries();
        public static LineSeries indVoltLine = new LineSeries();
        public static LineSeries resVoltLine = new LineSeries();
        public static LineSeries resultPoint = new LineSeries();

        public static OxyPlot.Axes.LinearAxis yAxis = new OxyPlot.Axes.LinearAxis(); //Makes the axes.
        public static OxyPlot.Axes.LinearAxis xAxis = new OxyPlot.Axes.LinearAxis(); //Makes the axes.

        public static PlotModel ExperimentModel { get; set; }

        public event PropertyChangedEventHandler PropertyChanged; // отслеживание изменений в текстовом блоке
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        //public event PropertyChangedEventHandler PropertyChanged; // отслеживание изменений в текстовом блоке
        //public void OnPropertyChanged([CallerMemberName] string prop = "")
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        //}

        //public MainWindow mw { get; set; }

        //static class utils
        //{
        //    public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        //    {
        //        if (depObj != null)
        //        {
        //            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
        //            {
        //                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
        //                if (child != null && child is T)
        //                {
        //                    yield return (T)child;
        //                }

        //                foreach (T childOfChild in FindVisualChildren<T>(child))
        //                {
        //                    yield return childOfChild;
        //                }
        //            }
        //        }
        //    }
        //}
    }

       
}
