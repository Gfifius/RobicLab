using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using OxyPlot;
using OxyPlot.Series;
using static RL.Experiment;
using static System.Math;

namespace RL
{
    /// <summary>
    /// Логика взаимодействия для Oscillating_circuit.xaml
    /// </summary>
    public partial class Resonance: Window
    {
        int typeOfExperiment = 0; // 0 - не выбран, ??1 - время колебаний от емкости, ??2 - время колебаний от индуктивности, ??3 - время колебаний от сопротивления
        private int numberExperiment = 1; // отсчет экспериментов

        double voltageValue = 5; // В
        double freqValue = 100000;  // Гц
        double capValue = 100;  // мкФ
        double indValue = 100;  // мГн
        double indResValue = 100;  // мОм
        double resValue = 100; // Ом
        double[] timeFromExperiment = new double[2];

        static private bool startExperiment = false;
        bool changeScheme = false;

        static string[] textScript;
        private string[] forReportHead = { "Номер опыта", "Напряжение генератора,В", "Частота, Гц", "Емкость, мкФ", "Индуктивность, мГн", "Сопротивление катушки, мОм", "Сопротивление резистора, Ом", "Сила тока, А", "Напряжение на конденсаторе, В", "Напряжение на катушке, В", "Напряжение на резисторе, В" };  //резисторе, конденсаторе, катушке
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
                //if (typeOfExperiment == 1) time = timeFromExperiment[1] * 1000; // 
                //else time = timeFromExperiment[0] * 1000;  // 
                forReport += $"{numberExperiment};{voltageValue};{freqValue};{capValue};{indValue};{indResValue};{resValue};{timeFromExperiment[0]};{timeFromExperiment[2]};{timeFromExperiment[3]};{timeFromExperiment[1]}\n"; //TODO: исправить
                numberExperiment = value;

            }
        }

        public double VoltageValue
        {
            get
            { return voltageValue; }
            set
            {
                if (value != 0) voltageValue = value;
                resParam.amplitude = voltageValue;
            }
        }
        public double FreqValue
        {
            get
            { return freqValue; }
            set
            {
                if (value != 0) freqValue = value;
                resParam.freq = freqValue;
            }
        }
        public double CapValue
        {
            get
            { return capValue; }
            set
            {
                if (value != 0) capValue = value;
                resParam.capacitance = capValue;
            }
        }
        public double IndValue
        {
            get
            { return indValue; }
            set
            {
                if (value != 0) indValue = value;
                resParam.inductive = indValue;
            }
        }
        public double IndResValue
        {
            get
            { return indResValue; }
            set
            {
                if (value != 0) indResValue = value;
                resParam.inductiveRes = indResValue;
            }
        }
        public double ResValue
        {
            get
            { return resValue; }
            set
            {
                if (value != 0) resValue = value;
                resParam.resistance = resValue;
            }
        }


        ResonanceModel.ResonanceParams resParam = new ResonanceModel.ResonanceParams();

        SolidColorBrush colorMouseEnter = (SolidColorBrush)(new BrushConverter().ConvertFrom("#5199FF"));
        SolidColorBrush colorMouseLeave = (SolidColorBrush)(new BrushConverter().ConvertFrom("#B7D4FF"));

        private List<expInfoResonance> expTable = new();

        public Resonance()
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
            startAgain.Visibility = Visibility.Hidden;
            completeExp.Visibility = Visibility.Hidden; 

            ResonanceHelp.Text = HelpText.ResonanceHelp;

            ResonanceModel.voltageIndLine.MouseDown += PlotView_PreviewMouseLeftButtonDown;//(s, e) =>
        }

        private void TextBoxChanged(object sender, TextChangedEventArgs e)
        {
            var nameTextBox = (sender as TextBox).Name;
            if ((sender as TextBox).Text != "")
            {
                if (nameTextBox == "Voltage") VoltageValue = double.Parse(Voltage.Text);
                if (nameTextBox == "Frequency") FreqValue = double.Parse(Frequency.Text);
                if (nameTextBox == "Capacitance") CapValue = double.Parse(Capacitance.Text);
                if (nameTextBox == "Inductance") IndValue = double.Parse(Inductance.Text);
                if (nameTextBox == "InductanceResistance") IndResValue = double.Parse(InductanceResistance.Text);
                if (nameTextBox == "Resistance") ResValue = double.Parse(Resistance.Text);
            }
            timeFromExperiment = ResonanceModel.ResonanceCalculate(resParam); // ток, напряжение на: резисторе, конденсаторе, катушке
            //if (changeScheme) Capacitor.CapCharge(ResValue, VoltageValue, CapValue, Oscillate.OscilPlot, Oscillate.voltageLine, Oscillate.currentLine);
            //else timeFromExperiment = Oscillate.OscillateProcces(VoltageValue, CapValue, IndValue, ResValue);

        }

        private void MouseWheelHandler(object sender, MouseWheelEventArgs e)
        {
            var nameTextBox = (sender as TextBox).Name;
            if (!(sender as TextBox).IsReadOnly)  // проверка на запрет редактирования
            {
                if (e.Delta > 0)
                {
                    if (nameTextBox == "Voltage") Voltage.Text = $"{VoltageValue++}";
                    if (nameTextBox == "Frequency") Frequency.Text = $"{FreqValue++}";
                    if (nameTextBox == "Capacitance") Capacitance.Text = $"{CapValue++}";
                    if (nameTextBox == "Inductance") Inductance.Text = $"{IndValue++}";
                    if (nameTextBox == "InductanceResistance") InductanceResistance.Text = $"{IndResValue++}";
                    if (nameTextBox == "Resistance") Resistance.Text = $"{ResValue++}";
                }
                if (e.Delta < 0)
                {
                    if (nameTextBox == "Voltage") Voltage.Text = $"{VoltageValue--}";
                    if (nameTextBox == "Frequency") Frequency.Text = $"{FreqValue--}";
                    if (nameTextBox == "Capacitance") Capacitance.Text = $"{CapValue--}";
                    if (nameTextBox == "Inductance") Inductance.Text = $"{IndValue--}";
                    if (nameTextBox == "InductanceResistance") InductanceResistance.Text = $"{IndResValue--}";
                    if (nameTextBox == "Resistance") Resistance.Text = $"{ResValue--}";
                }
            }

        }

        private void Exp_ClickResonance(object sender, RoutedEventArgs e)  // кнопка для начала и проведения эксперимента
        {
            if (startExperiment)
            {
                //   timeFromExperiment   0 - Сила тока от частоты генератора, 1 - Амплитудно-частотная характеристика
                if (typeOfExperiment == 1)
                {
                    GetPoint(FreqValue, timeFromExperiment[0], "Частота, кГц", "Сила тока, А");
                    //expTable.Add(new expInfoResonance { Capacite = capValue, Time = Round(timeFromExperiment[1] * 1000, 6) }); //формат времени???

                }
                if (typeOfExperiment == 2)
                {
                    // ток, напряжение на: резисторе, конденсаторе, катушке
                    GetThreePoint(FreqValue, timeFromExperiment[1], timeFromExperiment[2], timeFromExperiment[3]); 
                    //GetPoint(indValue, timeFromExperiment[0], "Индуктивность, мГн");
                    //expTable.Add(new expInfoResonance { Capacite = indValue, Time = Round(timeFromExperiment[0] * 1000, 6) });
                }
                expTable.Add(new expInfoResonance { Freq = freqValue, ResVolt = timeFromExperiment[1], CapVolt = timeFromExperiment[2], IndVolt = timeFromExperiment[3], Current = timeFromExperiment[0] }); //формат времени???
                //if (typeOfExperiment == 3)
                //{
                //    GetPoint(resValue, timeFromExperiment[0], "Сопротивление, Ом");
                //    expTable.Add(new expInfo { Capacite = resValue, Time = Round(timeFromExperiment[0] * 1000, 6) });
                //}

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

                ResonanceHelp.Visibility = Visibility.Hidden;
            }
        }

        private void Button_ClickResonance(object sender, RoutedEventArgs e)  //-----  ОБРАБОТЧИК КНОПОК  -----//
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
                        //RLCPowerName.Content = "Включить источник питания";
                    }
                    else
                    {
                        changeScheme = true;
                        RLCScheme.Source = new BitmapImage(new Uri($"{MainWindow.resourceAdress}\\Resources\\RLCoff.png"));
                        Capacitor.CapCharge(ResValue, VoltageValue, CapValue, Oscillate.OscilPlot, Oscillate.voltageLine, Oscillate.currentLine);
                       // RLCPowerName.Content = "Выключить источник питания";
                    }
                }
            }
            if (nameButton == "Experiment1")
            {
                typeOfExperiment = 1;
                Resistance.IsReadOnly = true;
                Capacitance.IsReadOnly = true;
                Voltage.IsReadOnly = true;
                Inductance.IsReadOnly = true;
                InductanceResistance.IsReadOnly = true;
               // forReportHead[5] = "Период, мс";
                ExperimentModel.Title = "Сила тока в от частоты";
            }

            if (nameButton == "Experiment2")
            {
                typeOfExperiment = 2;
                Resistance.IsReadOnly = true;
                Capacitance.IsReadOnly = true;
                Voltage.IsReadOnly = true;
                Inductance.IsReadOnly = true;
                InductanceResistance.IsReadOnly = true;
                //forReportHead[5] = "Время, мс";
                ExperimentModel.Title = "Напряжение элементов от частоты";
            }

            if (nameButton == "Experiment1" || nameButton == "Experiment2")
            {
                VisibleButton();
                textScript = RWclass.ReadScript(RWclass.AllScript("Резонанс")[typeOfExperiment - 1]);
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
                RWclass.SaveAsReport("Резонанс", reportExcel);
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

        private void PlotView_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("sfsdfsdf");
        }

        private void PlotView_PreviewMouseLeftButtonDown(object sender, OxyMouseDownEventArgs e) //MouseButtonEventArgs e)
        {
            double x;
            double y;
            x = (sender as LineSeries).InverseTransform(e.Position).X;
            y = (sender as LineSeries).InverseTransform(e.Position).Y;

        }
    }

    public class ResonanceModel
    {
        public static double Voltage { get; set; }
        public static double Capacite { get; set; }
        public static double Resistance { get; set; }
        public static double Inductance { get; set; }

        public static LineSeries voltageResLine;
        public static LineSeries voltageCapLine;
        public static LineSeries voltageIndLine;

        public struct ResonanceParams
        {
            public double freq;  // 
            public double amplitude;   // 
            public double capacitance;   // 
            public double resistance;
            public double inductive;
            public double inductiveRes;
        }


        public ResonanceModel()
        {
            ResonancePlot = new PlotModel { Title = "Резонанс" };
            OxyPlot.Axes.LinearAxis yAxis = new OxyPlot.Axes.LinearAxis(); //Makes the axes.
           // OxyPlot.Axes.LinearAxis y2Axis = new OxyPlot.Axes.LinearAxis(); //Makes the axes.
            OxyPlot.Axes.LinearAxis xAxis = new OxyPlot.Axes.LinearAxis(); //Makes the axes.
            yAxis.Key = "Voltage";
            yAxis.Title = "Напряжение, В";
            yAxis.TextColor = OxyColor.FromRgb(0, 0, 0);
            yAxis.MajorGridlineStyle = LineStyle.Dot;

            xAxis.Key = "time";
            xAxis.Title = "Время, мс";
            xAxis.MajorGridlineStyle = LineStyle.Dot;
            xAxis.Position = OxyPlot.Axes.AxisPosition.Bottom;

            ResonancePlot.Axes.Add(yAxis);
            ResonancePlot.Axes.Add(xAxis);

            voltageResLine = new LineSeries();
            voltageResLine.Title = "U резистор";
            voltageResLine.LineStyle = LineStyle.Solid;
            voltageResLine.Color = OxyColor.FromRgb(0, 0, 0);
            voltageResLine.StrokeThickness = 1.0;
            ResonancePlot.Series.Add(voltageResLine);

            voltageCapLine = new LineSeries();
            voltageCapLine.Title = "U конденсатор";
            voltageCapLine.LineStyle = LineStyle.Solid;
            voltageCapLine.Color = OxyColor.FromRgb(255, 0, 0);
            voltageCapLine.StrokeThickness = 1.0;
            //currentLine.YAxisKey = y2Axis.Key;
            ResonancePlot.Series.Add(voltageCapLine);

            voltageIndLine = new LineSeries();
            voltageIndLine.Title = "U индуктивность";
            voltageIndLine.LineStyle = LineStyle.Solid;
            voltageIndLine.Color = OxyColor.FromRgb(0, 0, 255);
            voltageIndLine.StrokeThickness = 1.0;
            ResonancePlot.Series.Add(voltageIndLine);

            //double x;
            //voltageIndLine.MouseDown += PlotView_PreviewMouseLeftButtonDown;//(s, e) =>
            ////{
            ////    x = (s as LineSeries).InverseTransform(e.Position).X;
            ////};

            ResonancePlot.InvalidatePlot(true);
        }

        public static double[] ResonanceCalculate(ResonanceParams param)
        {
            double R = param.resistance;
            double C = param.capacitance / 1000000000.0;   // из нФ в Ф
            double L = param.inductive / 1000000.0;        // из мкГн в Гн
            double Zind = param.inductiveRes / 1000.0;  // из мОм в Ом
            double U = param.amplitude;
            double w = param.freq * 2 * PI * 1000; // приходит в кГц

            double XL = w * L;
            double XC = 1 / (w * C);

            double I = U / (Sqrt(Pow(R, 2) + Pow((w * L - (1 / (w * C))), 2) + Sqrt(Pow(Zind, 2))));

            double fi = Atan((XL - XC)/R);

            double t = 0;
            double voltageRes = 0;
            double voltageCap = 0;
            double voltageInd = 0;
            double[] amplOnElements = new double[4]; // ток, напряжение на: резисторе, конденсаторе, катушке
            amplOnElements[0] = I;
            amplOnElements[1] = I * R;
            amplOnElements[2] = I * XC;
            amplOnElements[3] = I * XL;

            voltageResLine.Points.Clear();
            voltageIndLine.Points.Clear();
            voltageCapLine.Points.Clear();
            for (int i = 0; i < 5000; i++)
            {
                t = (double)i / (param.freq * 1000000);//1000000000.0;

                voltageRes = I * R * Sin(w * t - fi);
                voltageInd = I * (w * L * Sin(w * t - fi + PI / 2.0) + Zind);
                voltageCap = I * Sin(w * t - fi - PI / 2.0) / (w * C);
                //if(param.freq < 100) { t *= 1000000; xAxis.Title = "Время, мс"; }
                //if (param.freq > 100) { t *= 1000000; xAxis.Title = "Время, мкс"; }
                t *= 1000;
                voltageResLine.Points.Add(new DataPoint(t, voltageRes));
                voltageIndLine.Points.Add(new DataPoint(t, Round(voltageInd, 3)));
                voltageCapLine.Points.Add(new DataPoint(t, voltageCap));
            }
            ResonancePlot.InvalidatePlot(true);
            return amplOnElements;
        }

        public static PlotModel ResonancePlot { get; private set; }
    }

    public class expInfoResonance
    {
        public string NameColumn2 { get; set; }
        public double Freq { get; set; }
        public double ResVolt { get; set; }
        public double CapVolt { get; set; }
        public double IndVolt { get; set; }
        public double Current { get; set; }
    }
}
