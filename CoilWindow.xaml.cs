using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OxyPlot;
using OxyPlot.Series;

using static RL.Experiment;
using static System.Math;

namespace RL
{
    /// <summary>
    /// Логика взаимодействия для CoilWindow.xaml
    /// </summary>
    public partial class CoilWindow : Window
    {
        public static int typeOfExperiment = 0; // 0 - не выбран, 1 - заряд от емкости, 2 - заряд от сопротивления
        private int numberExperiment = 1; // отсчет экспериментов

        static public double inductValue = 0.01;
        static public double voltageValue = 5;
        static public double resValue = 10;

        static private bool startExperiment = false;
        static public bool changeScheme = true;

        static string[] textScript;
        private string[] forReportHead = { "Номер опыта", "Индуктивность, мГн", "Сопротивление, Ом", "Напряжение,В", "Время, мс" };
        private string forReport = "";

        SolidColorBrush colorMouseEnter = (SolidColorBrush)(new BrushConverter().ConvertFrom("#5199FF"));
        SolidColorBrush colorMouseLeave = (SolidColorBrush)(new BrushConverter().ConvertFrom("#B7D4FF"));

        private int NumberExperiment
        {
            get
            {
                return numberExperiment;
            }
            set
            {
                forReport += $"{numberExperiment};{inductValue};{resValue};{voltageValue};{Round(Coil.timeOfCharge * 1000, 6)}\n";
                numberExperiment = value;

            }
        }

        private List<expInfo> expTable = new();

        public CoilWindow()
        {
            InitializeComponent();
            logo.Source = new BitmapImage(new Uri($"{MainWindow.resourceAdress}\\Resources\\logo.png"));
            Close.Source = new BitmapImage(new Uri($"{MainWindow.resourceAdress}\\Resources\\Close.png"));
            Home.Source = new BitmapImage(new Uri($"{MainWindow.resourceAdress}\\Resources\\Home.png"));
            RLScheme.Source = new BitmapImage(new Uri($"{MainWindow.resourceAdress}\\Resources\\RLoff.png"));

            experimentTable.Visibility = Visibility.Hidden;  // изначально таблица невидима
            graf2.Visibility = Visibility.Hidden;  // график тоже скрыт завесой тайны
            grafOrTableButton.Visibility = Visibility.Hidden; // кнопка смены режима "таблица/график"
            Script.Visibility = Visibility.Hidden;
            stackForButton.Visibility = Visibility.Hidden;

            CoilHelp.Text = HelpText.CoilHelp;
        }


        private void TextBox_KeyEnterUpdate(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                DependencyProperty prop = TextBox.TextProperty;
                BindingExpression binding = BindingOperations.GetBindingExpression(Inductance, prop);

                if (binding != null) { binding.UpdateSource(); }
                inductValue = double.Parse(Inductance.Text);
                resValue = double.Parse(Resistance.Text);
                voltageValue = double.Parse(Voltage.Text);


            }
        }

        private void TextBoxChanged(object sender, TextChangedEventArgs e)
        {
            var nameTextBox = (sender as TextBox).Name;
            if (nameTextBox == "Capacitance") inductValue = double.Parse(Inductance.Text);
            if (nameTextBox == "Voltage") voltageValue = double.Parse(Voltage.Text);
            if (nameTextBox == "Resistance") resValue = double.Parse(Resistance.Text);

            //TODO: посмотреть, как сгенерировать события, возможно, есть смысл закрывать родительское окно после завершения открывания дочернего   main.closed - 

        }

        private void MouseWheelHandler(object sender, MouseWheelEventArgs e)
        {
            // If the mouse wheel delta is positive, move the box up.
            var nameTextBox = (sender as TextBox).Name;
            if (!(sender as TextBox).IsReadOnly)  // проверка на запрет редактирования
            {
                if (e.Delta > 0)
                {
                    if (nameTextBox == "Inductance") Inductance.Text = $"{inductValue += 0.1}";
                    if (nameTextBox == "Voltage") Voltage.Text = $"{voltageValue += 0.1}";
                    if (nameTextBox == "Resistance") Resistance.Text = $"{resValue += 0.1}";

                }
                if (e.Delta < 0)
                {
                    if (nameTextBox == "Inductance") Inductance.Text = $"{inductValue -= 0.1}";
                    if (nameTextBox == "Voltage") Voltage.Text = $"{voltageValue -= 0.1}";
                    if (nameTextBox == "Resistance") Resistance.Text = $"{resValue -= 0.1}";

                }
            }

        }

        private void Exp_Click(object sender, RoutedEventArgs e)  // кнопка для начала эксперимента
        {
            if (startExperiment) // если кнопка уже была нажата - фиксируем значения и передаем точки в график
            {
                if (typeOfExperiment == 1)
                {
                    Resistance.IsReadOnly = true;
                    Voltage.IsReadOnly = true;
                    GetPoint(inductValue, Coil.timeOfCharge, "Емкость, мкФ");
                    expTable.Add(new expInfo { Capacite = inductValue, Time = Round(Coil.timeOfCharge * 1000, 6) }); //TODO: исправить косяки

                    column1.Header = "Емкость, мкФ";
                    //phonesGrid.SourceUpdated += PhonesGrid_SourceUpdated;

                }
                if (typeOfExperiment == 2)
                {
                    Inductance.IsReadOnly = true;
                    Voltage.IsReadOnly = true;
                    GetPoint(resValue, Coil.timeOfCharge, "Сопротивление, Ом");
                    expTable.Add(new expInfo { Capacite = resValue, Time = Round(Coil.timeOfCharge * 1000, 6) });
                    column1.Header = "Сопротивление, Ом";
                }

                experimentTable.ItemsSource = null;
                experimentTable.ItemsSource = expTable;
                if (numberExperiment < textScript.Length) Script.Text = textScript[numberExperiment];
                NumberExperiment++;
            }
            else  // если кнопка нажата впервые, выводим кнопки для выбора типа эксперимента
            {
                startExperiment = true;
                stackForButton.Visibility = Visibility.Visible; // кнопки выбора экспериментов
                //CreateButton();
                ExperimentButton.Visibility = Visibility.Hidden;
                ExperimentLabel.Content = "Зафиксировать значение";
                CoilHelp.Visibility = Visibility.Hidden;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)  //-----  ОБРАБОТЧИК КНОПОК  -----//
        {
            string nameButton = "";
            if (sender is Button) nameButton = (sender as Button).Name;
            if (sender is Border) nameButton = (sender as Border).Name;
            if (sender is Image) nameButton = (sender as Image).Name;

            if (nameButton == "RLPower" || nameButton == "RLScheme")  // включение/выключение источника питания
            {
                // false - зарядка катушки
                // true  - разрядка катушки
                if (changeScheme)
                {
                    changeScheme = false;
                    RLScheme.Source = new BitmapImage(new Uri($"{MainWindow.resourceAdress}\\Resources\\RLOn.png"));
                    RLPowerName.Content = "Выключить источник питания";
                }
                else
                {
                    changeScheme = true;
                    RLScheme.Source = new BitmapImage(new Uri($"{MainWindow.resourceAdress}\\Resources\\RLOff.png"));
                    RLPowerName.Content = "Включить источник питания";
                }
            }

            if (nameButton == "Experiment1") { typeOfExperiment = 1; VisibleButton(); }
            if (nameButton == "Experiment2") { typeOfExperiment = 2; VisibleButton(); }
            if (nameButton == "Experiment1" || nameButton == "Experiment2")
            {
                textScript = RWclass.ReadScript(RWclass.AllScript("Индуктивность")[typeOfExperiment - 1]);
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
                //forReport = "Номер опыта;Емкость, мкФ;Сопротивление, Ом;Напряжение,В;Время, мс\n";

            }

            if (nameButton == "completeExp") // кнопка завершить
            {
                var reportData = new Reporter().GetReport(forReportHead, forReport);
                var reportExcel = new ExcelGenerator().Generate(reportData, 2, 6);
                RWclass.SaveAsReport("Индуктивность", reportExcel);
                //phonesGrid.ItemsSource = expTable;
                /* нужно реализовать возможно выгрузки результатов в документ */
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

        //private void Experiment_MouseEnter(object sender, MouseEventArgs e)
        //{
        //    if (startExperiment) Experiment.Source = new BitmapImage(new Uri($"{MainWindow.resourceAdress}\\Resources\\Fix_Hover.png"));
        //    else Experiment.Source = new BitmapImage(new Uri($"{MainWindow.resourceAdress}\\Resources\\Exp_Hover.png"));
        //}

        //private void Experiment_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    if (startExperiment) Experiment.Source = new BitmapImage(new Uri($"{MainWindow.resourceAdress}\\Resources\\Fix.png"));
        //    else Experiment.Source = new BitmapImage(new Uri($"{MainWindow.resourceAdress}\\Resources\\Exp.png"));
        //}

        //Кнопка возвращения на главную страницу
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            //MainWindow win = new MainWindow();
            Destruct();  //TODO: убрать повторы
            ClearPoint();
            experimentTable.ItemsSource = null;
            expTable.Clear();
            startExperiment = false;
            //win.Show();
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
            var nameElement = (sender as Border).Name;
            var nameEvent = (e as MouseEventArgs).RoutedEvent.Name;
            //if (nameElement == "onOffPower")
            //{
            if (nameEvent == "MouseEnter") (sender as Border).BorderBrush = colorMouseEnter; //(SolidColorBrush)(new BrushConverter().ConvertFrom("#5199FF"));
            if (nameEvent == "MouseLeave") (sender as Border).BorderBrush = colorMouseLeave;//(SolidColorBrush)(new BrushConverter().ConvertFrom("#B7D4FF"));
            //}

            //if(nameElement == .....
        }


        //Кнопка закрытия окна

        private void Close_MouseDown(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        //Обработчики для наведения на кнопку "Начать эксперимент"
       
    }


    public class Coil
    {
        public static double timeOfCharge = 0;

        private LineSeries inductVoltage;
        private LineSeries inductCurrent;
        public Coil()
        {
            this.MyModelCoil = new PlotModel { Title = "cap" };
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

            xAxis.Key = "time";
            xAxis.Title = "Время, с";
            xAxis.MajorGridlineStyle = LineStyle.Dot;
            xAxis.Position = OxyPlot.Axes.AxisPosition.Bottom;

            MyModelCoil.Axes.Add(yAxis);
            MyModelCoil.Axes.Add(y2Axis);
            MyModelCoil.Axes.Add(xAxis);



            inductVoltage = new LineSeries();
            inductVoltage.LineStyle = LineStyle.Solid;
            inductVoltage.Color = OxyColor.FromRgb(0, 0, 0);
            inductVoltage.StrokeThickness = 1.0;
            this.MyModelCoil.Series.Add(inductVoltage);

            inductCurrent = new LineSeries();
            inductCurrent.LineStyle = LineStyle.Solid;
            inductCurrent.Color = OxyColor.FromRgb(255, 0, 0);
            inductCurrent.StrokeThickness = 1.0;
            inductCurrent.YAxisKey = y2Axis.Key;
            this.MyModelCoil.Series.Add(inductCurrent);
            //axesLineX.Points.Add(new DataPoint(0,0));
            //axesLineX.Points.Add(new DataPoint(10, 10));
            this.MyModelCoil.InvalidatePlot(true);

            Thread receiveThread = new Thread(new ThreadStart(WorkingWindow));
            receiveThread.Start();
        }

        private void WorkingWindow()
        {
            while (true)
            {
                if (CoilWindow.resValue > 0 && CoilWindow.voltageValue > 0 && CoilWindow.inductValue > 0)
                {
                    if (CoilWindow.changeScheme) CoilDischarge(CoilWindow.resValue, CoilWindow.voltageValue, CoilWindow.inductValue);
                    else СoilCharge(CoilWindow.resValue, CoilWindow.voltageValue, CoilWindow.inductValue);
                }
                else
                {
                    inductVoltage.Points.Clear();
                    inductCurrent.Points.Clear();
                    this.MyModelCoil.InvalidatePlot(true);
                }

                System.Threading.Thread.Sleep(100);

            }

        }

        public void СoilCharge(double resistance, double voltage, double inductance)
        {
            double tau = inductance / resistance;
            double UCoil = 0;
            double ICoil = 0;
            double time = 0;
            MyModelCoil.Title = "Заряд индуктивности";
            inductVoltage.Title = "Напряжение";
            inductCurrent.Title = "Ток";
            inductVoltage.Points.Clear();
            inductCurrent.Points.Clear();
            int i = 0;
            while (true)
            {
                if (tau < 0.0006) time = i / 1000000.0; // для очень быстрых процессов (маленькая емкость)
                else time = i / 10000.0;
                UCoil = voltage * Math.Exp(-time / tau);
                ICoil = (voltage / resistance) * (1 - Math.Exp(-time / tau));
                if (UCoil > 0.1)
                {
                    inductVoltage.Points.Add(new DataPoint(time, UCoil));
                    inductCurrent.Points.Add(new DataPoint(time, ICoil));
                }
                else break;
                i++;
            }
            timeOfCharge = time;
            this.MyModelCoil.InvalidatePlot(true);
        }

        public void CoilDischarge(double resistance, double voltage, double inductance)
        {
            double tau = inductance / resistance;
            double iInit = voltage / resistance;
            double UCoil = 0;
            double ICoil = 0;
            double time = 0;
            MyModelCoil.Title = "Разряд индуктивности";
            inductVoltage.Title = "Напряжение";
            inductCurrent.Title = "Ток";
            inductVoltage.Points.Clear();
            inductCurrent.Points.Clear();
            int i = 0;
            while (true)
            {
                if (tau < 0.0006) time = i / 1000000.0; // для очень быстрых процессов (маленькая емкость)
                else time = i / 10000.0;
                UCoil = -resistance * iInit * Math.Exp(-time / tau);//voltage * Math.Exp(-time / tau);
                ICoil = iInit* Math.Exp(-time / tau); //-(voltage / resistance) * Math.Exp(-time / tau);
                if (ICoil > 0.1)
                {
                    inductVoltage.Points.Add(new DataPoint(time, UCoil));
                    inductCurrent.Points.Add(new DataPoint(time, ICoil));
                }
                else break;
                i++;
            }
            timeOfCharge = time;
            this.MyModelCoil.InvalidatePlot(true);
        }


        public PlotModel MyModelCoil { get; private set; }
        //public  { get; private set; }
    }

}
