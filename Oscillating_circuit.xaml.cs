using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static RL.Experiment;
using static System.Math;

namespace RL
{
    /// <summary>
    /// Логика взаимодействия для Oscillating_circuit.xaml
    /// </summary>
    public partial class Oscillating_circuit : Window
    {
        int typeOfExperiment = 0; // 0 - не выбран, 1 - время колебаний от емкости, 2 - время колебаний от индуктивности, 3 - время колебаний от сопротивления
        private int numberExperiment = 1; // отсчет экспериментов

        double voltageValue = 5; // В
        double capValue = 100;  // мкФ
        double indValue = 100;  // мГн
        double resValue = 100; // Ом
        double[] timeFromExperiment = new double[2];

        static private bool startExperiment = false;
        bool changeScheme = false;

        static string[] textScript;
        private string[] forReportHead = { "Номер опыта", "Емкость, мкФ", "Индуктивность, мГн", "Сопротивление, Ом", "Напряжение,В", "Время, мс" };
        private string forReport = "";

        private int NumberExperiment
        {
            get
            {
                return numberExperiment;
            }
            set
            {
                double time = 0;
                if (typeOfExperiment == 1) time = timeFromExperiment[1] * 1000; // период колебаний, при экспериментах с конденсатором
                else time = timeFromExperiment[0] * 1000;  // время 3 тау, для остальных опытов
                forReport += $"{numberExperiment};{capValue};{indValue};{resValue};{voltageValue};{Round(time, 6)}\n"; //TODO: исправить
                numberExperiment = value;

            }
        }

        public double VoltageValue 
        {
            get 
            { return voltageValue; }
            set 
            { 
                if(value != 0) voltageValue = value;
            }
        }
        public double CapValue
        {
            get
            { return capValue; }
            set
            {
                if (value != 0) capValue = value;
            }
        }
        public double IndValue
        {
            get
            { return indValue; }
            set
            {
                if (value != 0) indValue = value;
            }
        }
        public double ResValue
        {
            get
            { return resValue; }
            set
            {
                if (value != 0) resValue = value;
            }
        }

        SolidColorBrush colorMouseEnter = (SolidColorBrush)(new BrushConverter().ConvertFrom("#5199FF"));
        SolidColorBrush colorMouseLeave = (SolidColorBrush)(new BrushConverter().ConvertFrom("#B7D4FF"));
        
        private List<expInfo> expTable = new();

        public Oscillating_circuit()
        {
            InitializeComponent();
            logo.Source = new BitmapImage(new Uri($"{MainWindow.resourceAdress}\\Resources\\logo.png"));
            Close.Source = new BitmapImage(new Uri($"{MainWindow.resourceAdress}\\Resources\\Close.png"));
            Home.Source = new BitmapImage(new Uri($"{MainWindow.resourceAdress}\\Resources\\Home.png"));
            RLCScheme.Source = new BitmapImage(new Uri($"{MainWindow.resourceAdress}\\Resources\\RLCon.png"));

            experimentTable.Visibility = Visibility.Hidden;  // изначально таблица невидима
            graf2.Visibility = Visibility.Hidden;  // график тоже скрыт завесой тайны
            grafOrTableButton.Visibility = Visibility.Hidden; // кнопка смены режима "таблица/график"
            Script.Visibility = Visibility.Hidden;
            stackForButton.Visibility = Visibility.Hidden;

            RLCHelp.Text = HelpText.RLCHelp;
        }

        private void TextBoxChanged(object sender, TextChangedEventArgs e)
        {
            var nameTextBox = (sender as TextBox).Name;
            if((sender as TextBox).Text != "")
            {
                if (nameTextBox == "Voltage") VoltageValue = double.Parse(Voltage.Text);
                if (nameTextBox == "Capacitance") CapValue = double.Parse(Capacitance.Text);
                if (nameTextBox == "Inductance") IndValue = double.Parse(Inductance.Text);
                if (nameTextBox == "Resistance") ResValue = double.Parse(Resistance.Text);
            }
            if (changeScheme) Capacitor.CapCharge(ResValue, VoltageValue, CapValue, Oscillate.OscilPlot, Oscillate.voltageLine, Oscillate.currentLine);
            else timeFromExperiment = Oscillate.OscillateProcces(VoltageValue, CapValue, IndValue, ResValue);
            
        }

        private void MouseWheelHandler(object sender, MouseWheelEventArgs e)
        {
            var nameTextBox = (sender as TextBox).Name;
            if (!(sender as TextBox).IsReadOnly)  // проверка на запрет редактирования
            {
                if (e.Delta > 0)
                {
                    if (nameTextBox == "Voltage") Voltage.Text = $"{VoltageValue++}";
                    if (nameTextBox == "Capacitance") Capacitance.Text = $"{CapValue++}";
                    if (nameTextBox == "Inductance") Inductance.Text = $"{IndValue++}";
                    if (nameTextBox == "Resistance") Resistance.Text = $"{ResValue++}";
                }
                if (e.Delta < 0)
                {
                    if (nameTextBox == "Voltage") Voltage.Text = $"{VoltageValue--}";
                    if (nameTextBox == "Capacitance") Capacitance.Text = $"{CapValue--}";
                    if (nameTextBox == "Inductance") Inductance.Text = $"{IndValue--}";
                    if (nameTextBox == "Resistance") Resistance.Text = $"{ResValue--}";
                }
            }

        }

        private void Exp_ClickRLC(object sender, RoutedEventArgs e)  // кнопка для начала и проведения эксперимента
        {
            if (startExperiment)
            {
                //   timeFromExperiment   0 - время колебаний 3 тау, 1 - период колебаний
                if (typeOfExperiment == 1)
                {
                    GetPoint(capValue, timeFromExperiment[1] * 1000, "Ёмкость, мкФ", "Период колебаний, мс");
                    expTable.Add(new expInfo { Capacite = capValue, Time = Round(timeFromExperiment[1] * 1000, 6) }); //формат времени???
                                    
                }
                if (typeOfExperiment == 2)
                {
                    GetPoint(indValue, timeFromExperiment[0], "Индуктивность, мГн");
                    expTable.Add(new expInfo { Capacite = indValue, Time = Round(timeFromExperiment[0] * 1000, 6) });
                }
                if (typeOfExperiment == 3)
                {
                    GetPoint(resValue, timeFromExperiment[0], "Сопротивление, Ом");
                    expTable.Add(new expInfo { Capacite = resValue, Time = Round(timeFromExperiment[0] * 1000, 6) });
                }

                experimentTable.ItemsSource = null;
                experimentTable.ItemsSource = expTable;
                if (numberExperiment < textScript.Length) Script.Text = textScript[numberExperiment];
                NumberExperiment++;
            }
            else
            {
                startExperiment = true;
                stackForButton.Visibility = Visibility.Visible; // кнопки выбора экспериментов
                ExperimentButton.Visibility = Visibility.Hidden;
                ExperimentLabel.Content = "Зафиксировать значение";

                RLCHelp.Visibility = Visibility.Hidden;
            }
        }

        private void Button_ClickRLC(object sender, RoutedEventArgs e)  //-----  ОБРАБОТЧИК КНОПОК  -----//
        {
            string nameButton = "";
            if (sender is Button) nameButton = (sender as Button).Name;
            if (sender is Border) nameButton = (sender as Border).Name;
            if (sender is Image) nameButton = (sender as Image).Name;

            if (nameButton == "RLCPower" || nameButton == "RLCScheme")  // включение/выключение источника питания
            {
                if (startExperiment == false) // Не меняем, если эксперимент уже начат
                { 
                    // false - свободные колебания
                    // true  - зарядка конденсатора
                    if (changeScheme)
                    {
                        changeScheme = false;
                        RLCScheme.Source = new BitmapImage(new Uri($"{MainWindow.resourceAdress}\\Resources\\RLCon.png"));
                        Oscillate.OscillateProcces(VoltageValue, CapValue, IndValue, ResValue);
                        RLCPowerName.Content = "Включить источник питания";
                    }
                    else
                    {
                        changeScheme = true;
                        RLCScheme.Source = new BitmapImage(new Uri($"{MainWindow.resourceAdress}\\Resources\\RLCoff.png"));
                        Capacitor.CapCharge(ResValue, VoltageValue, CapValue, Oscillate.OscilPlot, Oscillate.voltageLine, Oscillate.currentLine);
                        RLCPowerName.Content = "Выключить источник питания";
                    }
                }
            }
            if (nameButton == "Experiment1")
            {
                typeOfExperiment = 1;
                Resistance.IsReadOnly = true;
                Voltage.IsReadOnly = true;
                Inductance.IsReadOnly = true;
                forReportHead[5] = "Период, мс";
                column2.Header = "Период, мс";
                column1.Header = "Емкость, мкФ";
                ExperimentModel.Title = "Период колебаний от ёмкости";
            }

            if (nameButton == "Experiment2")
            {
                typeOfExperiment = 2;
                Resistance.IsReadOnly = true;
                Capacitance.IsReadOnly = true;
                Voltage.IsReadOnly = true;
                forReportHead[5] = "Время, мс";
                column2.Header = "Время, мс";
                column1.Header = "Индуктивность, мГн";
                ExperimentModel.Title = "Время колебаний от индуктивности";
            }

            if (nameButton == "Experiment3")
            {
                typeOfExperiment = 3;
                Capacitance.IsReadOnly = true;
                Voltage.IsReadOnly = true;
                Inductance.IsReadOnly = true;
                forReportHead[5] = "Время, мс";
                column2.Header = "Время, мс";
                column1.Header = "Сопротивление, Ом";
                ExperimentModel.Title = "Время колебаний от сопротивления";
            }

            if (nameButton == "Experiment1" || nameButton == "Experiment2" || nameButton == "Experiment3")
            {
                VisibleButton();
                textScript = RWclass.ReadScript(RWclass.AllScript("Колебательный контур")[typeOfExperiment - 1]);
                Script.Text = textScript[0];
                Script.Visibility = Visibility.Visible;
            }

            if (nameButton == "startAgain") // кнопка начать заново
            {
                ClearPoint();
                experimentTable.ItemsSource = null;
                expTable.Clear();
                experimentTable.ItemsSource = expTable;
                numberExperiment = 1;
                Script.Text = textScript[0];
                //forReport = "Номер опыта;Емкость, мкФ;Сопротивление, Ом;Напряжение,В;Время, мс\n"; //TODO: рудимент, нужно переделать под другой отчет
            }

            if (nameButton == "completeExp") // кнопка завершить
            {
                var reportData = new Reporter().GetReport(forReportHead, forReport);
                var reportExcel = new ExcelGenerator().Generate(reportData, 2, 7);
                RWclass.SaveAsReport("Колебательный контур", reportExcel);
            }

            if (nameButton == "grafOrTableButton") // кнопка смены отображения таблицы и графика
            {
                if (graf2.Visibility == Visibility.Visible)
                {
                    graf2.Visibility = Visibility.Hidden;
                    experimentTable.Visibility = Visibility.Visible;
                    grafOrTableLabel.Content = "График";
                }
                else
                {
                    experimentTable.Visibility = Visibility.Hidden;
                    graf2.Visibility = Visibility.Visible;
                    grafOrTableLabel.Content = "Таблица";
                }
            }
        }

        private void VisibleButton() // скрываем кнопки выбора опытов и открываем управление экспериментом
        {
            ExperimentButton.Visibility = Visibility.Visible; //кнопка фиксации результатов
            startAgain.Visibility = Visibility.Visible; // кнопка "Начать сначала"
            completeExp.Visibility = Visibility.Visible; // кнопка "Завершить"
            graf2.Visibility = Visibility.Visible; // график эксперимента
            grafOrTableButton.Visibility = Visibility.Visible; // кнопка смены режима "таблица/график"
            stackForButton.Visibility = Visibility.Hidden; // кнопки выбора экспериментов

        }

        //Кнопка возвращения на главную страницу
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            Destruct();  //TODO: убрать повторы
            ClearPoint();
            experimentTable.ItemsSource = null;
            expTable.Clear();
            startExperiment = false;
            this.Close();
        }

        private void Home_MouseEnter(object sender, MouseEventArgs e)
        {
            Home.Source = new BitmapImage(new Uri($"{MainWindow.resourceAdress}\\Resources\\Home_Hover.png"));
        }

        private void Home_MouseLeave(object sender, MouseEventArgs e)
        {
            Home.Source = new BitmapImage(new Uri($"{MainWindow.resourceAdress}\\Resources\\Home.png"));
        }

        //Обработчик наведения на кнопки
        private void MouseRoutedEvent(object sender, MouseEventArgs e) // новый обработчик
        {
            var nameEvent = (e as MouseEventArgs).RoutedEvent.Name;
            if (nameEvent == "MouseEnter") (sender as Border).BorderBrush = colorMouseEnter; //(SolidColorBrush)(new BrushConverter().ConvertFrom("#5199FF"));
            if (nameEvent == "MouseLeave") (sender as Border).BorderBrush = colorMouseLeave;//(SolidColorBrush)(new BrushConverter().ConvertFrom("#B7D4FF"));

        }

        private void Close_MouseDown(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }


    public class Oscillate
    {
        public static double Voltage { get; set; }
        public static double Capacite { get; set; }
        public static double Resistance { get; set; }
        public static double Inductance { get; set; }

        public static LineSeries voltageLine;
        public static LineSeries currentLine;

        public Oscillate()
        {
            OscilPlot = new PlotModel { Title = "Колебательный контур" };
            OxyPlot.Axes.LinearAxis yAxis = new OxyPlot.Axes.LinearAxis(); //Makes the axes.
            OxyPlot.Axes.LinearAxis y2Axis = new OxyPlot.Axes.LinearAxis(); //Makes the axes.
            OxyPlot.Axes.LinearAxis xAxis = new OxyPlot.Axes.LinearAxis(); //Makes the axes.
            yAxis.Key = "Voltage";
            yAxis.Title = "Напряжение, В";
            yAxis.TextColor = OxyColor.FromRgb(0, 0, 0);
            yAxis.MajorGridlineStyle = LineStyle.Dot;

            y2Axis.Key = "Current";
            y2Axis.Title = "Сила тока, А";
            y2Axis.TextColor = OxyColor.FromRgb(255, 0, 0);
            y2Axis.TitleColor = OxyColor.FromRgb(255, 0, 0);
            y2Axis.Position = OxyPlot.Axes.AxisPosition.Right;
            //y2Axis.Position = PositionTier

            xAxis.Key = "time";
            xAxis.Title = "Время, с";
            xAxis.MajorGridlineStyle = LineStyle.Dot;
            xAxis.Position = OxyPlot.Axes.AxisPosition.Bottom;

            OscilPlot.Axes.Add(yAxis);
            OscilPlot.Axes.Add(y2Axis);
            OscilPlot.Axes.Add(xAxis);



            voltageLine = new LineSeries();
            voltageLine.LineStyle = LineStyle.Solid;
            voltageLine.Color = OxyColor.FromRgb(0, 0, 0);
            voltageLine.StrokeThickness = 1.0;
            OscilPlot.Series.Add(voltageLine);

            currentLine = new LineSeries();
            currentLine.LineStyle = LineStyle.Solid;
            currentLine.Color = OxyColor.FromRgb(255, 0, 0);
            currentLine.StrokeThickness = 1.0;
            currentLine.YAxisKey = y2Axis.Key;
            OscilPlot.Series.Add(currentLine);
            //axesLineX.Points.Add(new DataPoint(0,0));
            //axesLineX.Points.Add(new DataPoint(10, 10));
            OscilPlot.InvalidatePlot(true);
            //OscillateProcces(2, 10, 1000, 1000);  //scillateProcces(5, 100, 1, 0.00001);
            //Thread receiveThread = new Thread(new ThreadStart(WorkingWindow));
            //receiveThread.Start();
        }


        public static double[] OscillateProcces(double voltage, double cap, double ind, double res)
        {
            /* В, мкФ, мГн, Ом */
            cap /= 1000000; // из мкФ и Ф
            ind /= 1000;    // из мГн в Гн
            double currentT;
            double voltageT;
            double A1 = 0;
            double A2 = 0;
            double time = 0;
            double max = 0;
            double min = 0;
            double tau = 0;
            double wt = 0;
            double[] result = new double[2];

            Complex p1 = 0;
            Complex p2 = 0;
            double discriminant = Pow(res, 2) - (4 * ind / cap);
            voltageLine.Points.Clear();
            currentLine.Points.Clear();
            if (discriminant > 0)
            {
                p1 = (-res + Sqrt(discriminant)) / 2 * ind;
                p2 = (-res - Sqrt(discriminant)) / 2 * ind;
                A2 = voltage * p1.Real / (p1.Real - p2.Real);
                A1 = voltage - A2;

            }
            if (discriminant < 0)
            {
                p1 = new Complex(-res / (2 * ind), Sqrt(Abs(discriminant)) / (2 * ind));
                p2 = new Complex(-res / (2 * ind), -Sqrt(Abs(discriminant)) / (2 * ind));
                A1 = voltage;
                A2 = A1 * p1.Real / p1.Imaginary;//Abs(p1.Imaginary);
            }
            if(discriminant == 0)
            {
                p1 = -res / 2 * ind; // случай такой сверху
            }
            // тау = 1 / декр.затухания. - время затухания в 2.7 раза
            // декремент затухания  = p1.Real
            tau = Abs(1.0 / p1.Real);
            
            for (int t = 0; t < tau * 5000; t++)
            {
                time = t / 1000.0;
                wt = p1.Imaginary * time;

                //currentT = (voltage / (2 * ind * p1 + res)) * Complex.Pow(2.17, p1);
                if (discriminant < 0)
                {
                    voltageT = (A1 * Cos(p1.Imaginary * time) + A2 * Sin(p1.Imaginary * time)) * Exp(-Abs(p1.Real) * time);
                    currentT = Exp(-Abs(p1.Real) * time) * (((A2 * Cos(wt) - A1 * Sin(wt)) * p1.Imaginary) - ((A1 * Cos(wt) + A2 * Sin(wt)) * p1.Real)) * cap * (-1);
                }
                else
                {
                    voltageT = A1 * Exp(p1.Real * time) + A2 * Exp(p2.Real * time);
                    currentT = (A1 * p1.Real * Exp(p1.Real * time) + A2 * p2.Real * Exp(p2.Real * time)) * cap * (-1);
                } 
                voltageLine.Points.Add(new DataPoint(time, voltageT));
                currentLine.Points.Add(new DataPoint(time, currentT));
            }
            OscilPlot.InvalidatePlot(true);
            result[0] = 3 * tau;
            result[1] = (2 * PI) / p1.Imaginary;
            OscilPlot.Title = "Колебательный контур";
            return result;

        }


        public static PlotModel OscilPlot { get; private set; }
    }
}
