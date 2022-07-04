using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static RL.Experiment;
using static System.Math;

namespace RL
{
    public partial class CapWindow : Window, INotifyPropertyChanged
    {
        private CapWindowExperiment win;


        private int numberExperiment = 1; // отсчет экспериментов
        public static int typeOfExperiment = 0; // 0 - не выбран, 1 - заряд от емкости, 2 - заряд от сопротивления

        public static double capValue = 100;
        static public double voltageValue = 5;
        static public double resValue = 10;

        static private bool startExperiment = false;
        static public bool fixExperiment = false;
        static public bool changeScheme = true;

        static string[] textScript;
        public string nameCol = "tgrtgrt";
        private string[] forReportHead = { "Номер опыта", "Емкость, мкФ", "Сопротивление, Ом", "Напряжение,В", "Время, мс" };
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
                forReport += $"{numberExperiment};{capValue};{resValue};{voltageValue};{Round(Capacitor.timeOfCharge * 1000, 6)}\n";
                numberExperiment = value;

            }
        }
        private List<expInfo> expTable = new();


        private MainWindow main;
        
        public CapWindow()
        {

            InitializeComponent();
            logo.Source = new BitmapImage(new Uri($"{MainWindow.resourceAdress}\\Resources\\logo.png"));
            Close.Source = new BitmapImage(new Uri($"{MainWindow.resourceAdress}\\Resources\\Close.png"));
            Home.Source = new BitmapImage(new Uri($"{MainWindow.resourceAdress}\\Resources\\Home.png"));
            RCScheme.Source = new BitmapImage(new Uri($"{MainWindow.resourceAdress}\\Resources\\RCoff.png"));

            experimentTable.Visibility = Visibility.Hidden;  // изначально таблица невидима
            graf2.Visibility = Visibility.Hidden;  // график тоже скрыт завесой тайны
            grafOrTableButton.Visibility = Visibility.Hidden; // кнопка смены режима "таблица/график"
            Script.Visibility = Visibility.Hidden;
            completeExpC.Visibility = Visibility.Hidden;
            stackForButtonC.Visibility = Visibility.Hidden;

            CapHelp.Text = HelpText.CapacitorHelp;

            this.Show();
            this.DataContext = this;

        }

        private void TextBoxChanged(object sender, TextChangedEventArgs e)
        {
            var nameTextBox = (sender as TextBox).Name;
            if (nameTextBox == "Capacitance") capValue = double.Parse(Capacitance.Text);
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
                    if (nameTextBox == "Capacitance") Capacitance.Text = $"{capValue++}";
                    if (nameTextBox == "Voltage") Voltage.Text = $"{voltageValue++}";
                    if (nameTextBox == "Resistance") Resistance.Text = $"{resValue++}";
                }
                if (e.Delta < 0)
                {
                    if (nameTextBox == "Capacitance") Capacitance.Text = $"{capValue--}";
                    if (nameTextBox == "Voltage") Voltage.Text = $"{voltageValue--}";
                    if (nameTextBox == "Resistance") Resistance.Text = $"{resValue--}";
                }
            }
            
        }

        private void Exp_Click(object sender, RoutedEventArgs e)  // /*--------кнопка для начала эксперимента--------*/
        {
            if (startExperiment) // если кнопка уже была нажата - фиксируем значения и передаем точки в график
            {
                if (typeOfExperiment == 1)
                {
                    GetPoint(capValue, Capacitor.timeOfCharge, "Емкость, мкФ");
                    expTable.Add(new expInfo {Capacite = capValue, Time = Round(Capacitor.timeOfCharge * 1000, 6) });

                }
                if (typeOfExperiment == 2)
                {
                    GetPoint(resValue, Capacitor.timeOfCharge, "Сопротивление, Ом");
                    expTable.Add(new expInfo { Capacite = resValue, Time = Round(Capacitor.timeOfCharge * 1000, 6) });
                    column1.Header = "Сопротивление, Ом";
                }
                
                experimentTable.ItemsSource = null;
                experimentTable.ItemsSource = expTable;
                if(numberExperiment < textScript.Length) Script.Text = textScript[numberExperiment];
                NumberExperiment++;
            }
            else  // если кнопка нажата впервые, выводим кнопки для выбора типа эксперимента
            {
                startExperiment = true;
                ExperimentButton.Visibility = Visibility.Hidden;
                stackForButtonC.Visibility = Visibility.Visible; // кнопки выбора экспериментов
                ExperimentLabel.Content = "Зафиксировать значение";
                CapHelp.Visibility = Visibility.Hidden;
            }
        }

        private void Button_ClickRC(object sender, RoutedEventArgs e)  //-----  ОБРАБОТЧИК КНОПОК  -----//
        {
            string nameButton = "";
            if (sender is Button) nameButton = (sender as Button).Name;
            if (sender is Border) nameButton = (sender as Border).Name;
            if (sender is Image) nameButton = (sender as Image).Name;

            if (nameButton == "RCPower" || nameButton == "RCScheme")  // включение/выключение источника питания
            {
                // false - зарядка конденсатора
                // true  - разрядка конденсатора
                if (changeScheme)
                {
                    changeScheme = false;
                    RCScheme.Source = new BitmapImage(new Uri($"{MainWindow.resourceAdress}\\Resources\\RCOn.png"));
                    RCPowerName.Content = "Выключить источник питания";
                }
                else
                {
                    changeScheme = true;
                    RCScheme.Source = new BitmapImage(new Uri($"{MainWindow.resourceAdress}\\Resources\\RCOff.png"));
                    RCPowerName.Content = "Включить источник питания";
                }
            }

            if (nameButton == "Experiment1")
            {
                typeOfExperiment = 1;
                Resistance.IsReadOnly = true;
                Voltage.IsReadOnly = true;
                column1.Header = "Емкость, мкФ";
            }
            if (nameButton == "Experiment2")
            {
                typeOfExperiment = 2;
                Capacitance.IsReadOnly = true;
                Voltage.IsReadOnly = true;
            }
            if (nameButton == "Experiment1" || nameButton == "Experiment2")
            {
                VisibleButton();
                textScript = RWclass.ReadScript(RWclass.AllScript("Конденсатор")[typeOfExperiment - 1]);
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
                forReport = "Номер опыта;Емкость, мкФ;Сопротивление, Ом;Напряжение,В;Время, мс\n";

            }
            if (nameButton == "completeExpC") // кнопка завершить
            {
                //RWclass.WriteReport("Конденсатор", forReport);
                var reportData = new Reporter().GetReport(forReportHead, forReport);
                var reportExcel = new ExcelGenerator().Generate(reportData, 2, 6);
                //File.WriteAllBytes("report.xlsx", reportExcel);
                RWclass.SaveAsReport("Конденсатор", reportExcel);
                /* нужно реализовать возможно выгрузки результатов в документ */

                
            }
            if (nameButton == "grafOrTableButton") // кнопка смены отображения таблицы и графика
            {
               if(graf2.Visibility == Visibility.Visible)
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
        private void CreateButton()  // создаем кнопки для выбора эксперимента. В будущем название эксперимента будем брать из файла сценария
        {
            Button[] stackButton = new Button[2];
            stackButton[0] = new Button();
            stackButton[0].Content = "Зависимость времени зарядки конденсатора от ёмкости";
            stackButton[0].Height = 50;
            stackButton[0].Width = 600;
            stackButton[0].Click += Button_ClickRC;
            stackButton[0].Name = "stackBut0";
            stackForButton.Children.Add(stackButton[0]);

            stackButton[1] = new Button();
            stackButton[1].Content = "Зависимость времени зарядки конденсатора от сопротивления";
            stackButton[1].Height = 50;
            stackButton[1].Width = 600;
            stackButton[1].Click += Button_ClickRC;
            stackButton[1].Name = "stackBut1";
            stackForButton.Children.Add(stackButton[1]);

        }

        private void VisibleButton() // скрываем кнопки выбора опытов и открываем управление экспериментом
        {
            ExperimentButton.Visibility = Visibility.Visible; //кнопка фиксации результатов
            startAgain.Visibility = Visibility.Visible; // кнопка "Начать сначала"
            completeExpC.Visibility = Visibility.Visible; // кнопка "Завершить"
            graf2.Visibility = Visibility.Visible; // график эксперимента
            grafOrTableButton.Visibility = Visibility.Visible; // кнопка смены режима "таблица/график"

            stackForButtonC.Visibility = Visibility.Hidden; // кнопки выбора экспериментов
            
        }

        public event PropertyChangedEventHandler PropertyChanged; // отслеживание изменений в текстовом блоке
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }



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


        //Кнопка закрытия окна

        private void Close_MouseDown(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        //Обработчик наведения на кнопки
        private void MouseRoutedEvent(object sender, MouseEventArgs e) // новый обработчик
        {
            string? nameEvent = (e as MouseEventArgs).RoutedEvent.Name;
            if (nameEvent == "MouseEnter") (sender as Border).BorderBrush = colorMouseEnter; //(SolidColorBrush)(new BrushConverter().ConvertFrom("#5199FF"));
            if (nameEvent == "MouseLeave") (sender as Border).BorderBrush = colorMouseLeave;//(SolidColorBrush)(new BrushConverter().ConvertFrom("#B7D4FF"));

        }

        private void Button_Click(object sender, MouseButtonEventArgs e)
        {

        }
    }

    public class expInfo
    {
        public string NameColumn2 { get; set; }
        public double Capacite { get; set; }
        public double Time { get; set; }
    }

    public class Capacitor
    {
        public static double timeOfCharge = 0;
        private LineSeries capVoltage;
        private LineSeries capCurrent;
        //public string textToTextBox { get; set; } = "Тестовое значение";
        public Capacitor()
        {
            this.MyModel = new PlotModel { Title = "cap" };
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

            MyModel.Axes.Add(yAxis);
            MyModel.Axes.Add(y2Axis);
            MyModel.Axes.Add(xAxis);

            

            capVoltage = new LineSeries();
            capVoltage.LineStyle = LineStyle.Solid;
            capVoltage.Color = OxyColor.FromRgb(0, 0, 0);
            capVoltage.StrokeThickness = 1.0;
            this.MyModel.Series.Add(capVoltage);

            capCurrent = new LineSeries();
            capCurrent.LineStyle = LineStyle.Solid;
            capCurrent.Color = OxyColor.FromRgb(255, 0, 0);
            capCurrent.StrokeThickness = 1.0;
            capCurrent.YAxisKey = y2Axis.Key;
            this.MyModel.Series.Add(capCurrent);
            //axesLineX.Points.Add(new DataPoint(0,0));
            //axesLineX.Points.Add(new DataPoint(10, 10));
            this.MyModel.InvalidatePlot(true);

            Thread receiveThread = new Thread(new ThreadStart(WorkingWindow));
            receiveThread.Start();
        }

        private void WorkingWindow()
        {
            int tmpDel = 0;
            while(true)
            {
                if(CapWindow.resValue > 0 && CapWindow.voltageValue > 0 && CapWindow.capValue > 0)
                {
                    if (CapWindow.changeScheme) CapDischarge(CapWindow.resValue, CapWindow.voltageValue, CapWindow.capValue);
                    else CapCharge(CapWindow.resValue, CapWindow.voltageValue, CapWindow.capValue, MyModel, capVoltage, capCurrent);
                }
                else
                {
                    capVoltage.Points.Clear();
                    capCurrent.Points.Clear();
                    this.MyModel.InvalidatePlot(true);
                }
                System.Threading.Thread.Sleep(100);
                
            }
            
        }

        public static void CapCharge(double resistance, double voltage, double capacitance, PlotModel Model, LineSeries cVoltage, LineSeries cCurrent)  // значение емкости конденсатора приходит в микрофарадах (пока временно),
        {
            capacitance = capacitance / 1000000; //!!!из мкФ в Ф!!!         //TODO: необходимо переделать логику работы следующим образом: значения текст боксов привязать к свойствам в этом классе 
            double tau = resistance * capacitance;                          // в set-тере толкать эту функцию при внесении изменений
            double Uc = 0;
            double Ic = 0;
            double time = 0;
            //Model.Title = "Заряд конденсатора";
            cVoltage.Title = "Напряжение";
            cCurrent.Title = "Ток";
            cVoltage.Points.Clear();
            cCurrent.Points.Clear();
            int i = 0;

            while (true)
            {
                if(tau < 0.0006) time = i / 1000000.0; // для очень быстрых процессов (маленькая емкость)
                else time = i / 10000.0;
                Uc = voltage * (1 - Math.Exp(-time / tau));
                Ic = (voltage / resistance) * Math.Exp(-time / tau);
                if (Uc < voltage * 0.95)
                {
                    cVoltage.Points.Add(new DataPoint(time, Uc));
                    cCurrent.Points.Add(new DataPoint(time, Ic));
                }
                else break;
                i++;
            }
            timeOfCharge = time;
            Model.InvalidatePlot(true);
        }

        public void CapDischarge(double resistance, double voltage, double capacitance)
        {
            capacitance = capacitance / 1000000; //!!!из мкФ в Ф!!!  
            double tau = resistance * capacitance;
            double Uc = 0;
            double Ic = 0;
            double time = 0;
            MyModel.Title = "Разряд конденсатора";
            capVoltage.Title = "Напряжение";
            capCurrent.Title = "Ток";
            capVoltage.Points.Clear();
            capCurrent.Points.Clear();
            int i = 0;
            while(true)
            {
                if (tau < 0.0006) time = i / 1000000.0; // для очень быстрых процессов (маленькая емкость)
                else time = i / 10000.0;
                Uc = voltage * Math.Exp(-time / tau);
                Ic = -(voltage / resistance) * Math.Exp(-time / tau);
                if (Uc > 0.1)
                {
                    capVoltage.Points.Add(new DataPoint(time, Uc));
                    capCurrent.Points.Add(new DataPoint(time, Ic));
                }
                else break;
                i++;
            }
            timeOfCharge = time;
            this.MyModel.InvalidatePlot(true);
        }

        public PlotModel MyModel { get; private set; }
    }

    
}
