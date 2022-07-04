using OxyPlot;
using OxyPlot.Series;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Math;

namespace RL
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Magnetic_field : Window
    {
        private int numberLoop = 0;
        private int diameter = 0;
        private string[] picture = { "N16_L30_D20.png", "N16_L30_D40.png", "N32_L60_D20.png", "N32_L60_D40.png" };

        SolidColorBrush colorMouseEnter = (SolidColorBrush)(new BrushConverter().ConvertFrom("#5199FF"));
        SolidColorBrush colorMouseLeave = (SolidColorBrush)(new BrushConverter().ConvertFrom("#B7D4FF"));

        //MagnetField magnetField = new MagnetField();
        MagnetField.SolenoidParams solenoidParams = new MagnetField.SolenoidParams();

        static private bool startExperiment = false;
        public Magnetic_field()
        {

            InitializeComponent();

            logo.Source = new BitmapImage(new Uri($"{MainWindow.resourceAdress}\\Resources\\logo.png"));
            Close.Source = new BitmapImage(new Uri($"{MainWindow.resourceAdress}\\Resources\\Close.png"));
            Home.Source = new BitmapImage(new Uri($"{MainWindow.resourceAdress}\\Resources\\Home.png"));

            solenoidParams.current = 1;
            solenoidParams.length = 0.3;//0.05;//0.3
            solenoidParams.radius = 0.025;//0.025;
            solenoidParams.NLoop = 600;

            MagnetField.drawGraf(solenoidParams);
            //MagnetField.drawGrafZ(0.02, -30, 30, solenoidParams);


        }


        //private void TextBoxChanged(object sender, TextChangedEventArgs e)
        //{
        //    var nameTextBox = (sender as TextBox).Name;
        //    if (nameTextBox == "test1") Example1 = double.Parse(test1.Text);
        //    if (nameTextBox == "test2") Example2 = double.Parse(test2.Text);


        //}

            private void N1_Checked(object sender, RoutedEventArgs e)
        {

            var nameRadioButton = (sender as RadioButton).Name;
            if (nameRadioButton == "N1") numberLoop = 1; 
            if (nameRadioButton == "N2") numberLoop = 2;
            if (nameRadioButton == "L1") diameter = 1;
            if (nameRadioButton == "L2") diameter = 2;


            if (numberLoop != 0 && diameter != 0)
            {
                int j = 0;
                if (numberLoop == 1 && diameter == 1) j = 0;
                if (numberLoop == 1 && diameter == 2) j = 1;
                if (numberLoop == 2 && diameter == 1) j = 2;
                if (numberLoop == 2 && diameter == 2) j = 3;
                coil.Source = new BitmapImage(new Uri($"{MainWindow.resourceAdress}\\Resources\\{picture[j]}"));
            }
            
        }

        private void Exp_Click(object sender, RoutedEventArgs e)  // кнопка для начала эксперимента
        {
            if (startExperiment)
            {
                Console.WriteLine("Уже начат");

            }
            else
            {
                startExperiment = true;
                //Coil_Grid.Children.Add(coilDraw);
                //Coil_Grid.Children.Add(vectorLine);  // arrowLine1
                //Coil_Grid.Children.Add(arrowLine1);
                //Coil_Grid.Children.Add(arrowLine2);
                //CapWindowExperiment win = new CapWindowExperiment("Megnetic");
                //win.Show();
            }
            Example1 = double.Parse(test1.Text);
            Example2 = double.Parse(test2.Text);
        }




        //Кнопка возвращения на главную страницу
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            MainWindow win = new MainWindow();
            win.Show();
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


        //Кнопка закрытия окна

        private void Close_MouseDown(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        double example1;
        public double Example1
        {
            get
            {
                return example1;    // возвращаем значение свойства
            }
            set
            {
                example1 = value;
            }
        }

        double example2;
        double example3= 0;
        double Bz = 0;
        double Bp = 0;
        double alpha = 0;
        double deltaX = 0;
        double deltaY = 0;

        int X0;  // координаты верхнего левого угла катушки
        int Y0;  // координаты верхнего левого угла катушки
        int X1; // координаты нижнего правого угла катушки
        int Y1; // координаты нижнего правого угла катушки
        int P0;  // середина прямоугольника
        int Z0;  //  середина прямоугольника

        Line vectorLine = new Line();
        Line arrowLine1 = new Line();
        Line arrowLine2 = new Line();
        //Polygon coilDraw = new Polygon();
        //public void drawCoil(int X0, int X1, int Y0, int Y1)
        //{
        //    Polygon coilDraw = new Polygon();
        //    coilDraw.Points.Add(new Point(X0, Y0));
        //    coilDraw.Points.Add(new Point(X1, Y0));
        //    coilDraw.Points.Add(new Point(X1, Y1));
        //    coilDraw.Points.Add(new Point(X0, Y1));
        //    coilDraw.Stroke = Brushes.Black;
        //    Coil_Grid.Children.Add(coilDraw);
        //}
        public double Example2
        {
            get
            {
                return example2;    // возвращаем значение свойства
            }
            set
            {

                example2 = value;
                // example3 = magnetField.CalculateVectorP(example1, Example2, solenoidParams);
                //MagnetField.drawGrafZ(0.2, -0.3, 0.3, solenoidParams);
                //MagnetField.drawBzGrafZ(example1, Example2, Example2 + 30, solenoidParams);
                //MagnetField.drawGrafPo(example1, example1+3, Example2, solenoidParams);
                //MagnetField.drawBzGrafPo(example1, example1 + 1, Example2, solenoidParams);
                Bz = MagnetField.CalculateVectorZ(-example1, Example2, solenoidParams);
                Bp = MagnetField.CalculateVectorP(-example1, Example2, solenoidParams);
                X0 = 400;  // координаты верхнего левого угла катушки
                Y0 = 400;  // координаты верхнего левого угла катушки
                X1 = (int)(X0 + (solenoidParams.length * 1000)); // координаты нижнего правого угла катушки
                Y1 = (int)(Y0 + (solenoidParams.radius * 2000)); // координаты нижнего правого угла катушки
                P0 = (int)(Y0 + (solenoidParams.radius * 1000));  // середина прямоугольника
                Z0 = (int)(X0 + ((solenoidParams.length * 1000) / 2));  //  середина прямоугольника

                example3 = example3 * 1000; // из Тл в мТл
                res1.Text = example3.ToString();

                DrawMF.drawCoil(X0, X1, Y0, Y1, Coil_Grid);
                var info =  DrawMF.drawVector(X0, X1, Y0, Y1, P0, Z0, example1, example2, Coil_Grid, solenoidParams);  //TODO: уменьшить кол-во параметров

                res1.Text = "Z- " + (info[0]*1000).ToString();
                res2.Text = "P- " + (info[1]*1000).ToString();
                res4.Text = $"{Sqrt((info[0] * info[0]) + (info[1] * info[1])) * 1000}";

                /*
                //coilDraw.Points.Add(new Point(X0, Y0));
                //coilDraw.Points.Add(new Point(X1, Y0));
                //coilDraw.Points.Add(new Point(X1, Y1));
                //coilDraw.Points.Add(new Point(X0, Y1));
                //coilDraw.Stroke = Brushes.Black;
                //Coil_Grid.Children.Add(coilDraw);

                //vectorLine.X1 = Z0 + Example2 * 1000;
                //vectorLine.Y1 = P0 + example1 * 1000;
                //vectorLine.X2 = vectorLine.X1 + (Bz * 1000000); //vertL.X1 + 5;//
                //vectorLine.Y2 =vectorLine.Y1 - (Bp * 1000000);  // vertL.Y1 + 5;//

                //deltaX = vectorLine.X1 - vectorLine.X2;
                //deltaY = vectorLine.Y2 - vectorLine.Y1;
                //alpha = Atan2(deltaY, deltaX); // угол альфа

                //arrowLine1.X1 = vectorLine.X2;  //конец вектора - начала стрелочки 
                //arrowLine1.Y1 = vectorLine.Y2;
                //arrowLine1.X2 = arrowLine1.X1 + 10 * Cos(alpha + 0.26);
                //arrowLine1.Y2 = arrowLine1.Y1  - 10 * Sin(alpha + 0.26);

                //arrowLine2.X1 = vectorLine.X2;  //конец вектора - начала стрелочки (вторая стрелочка)
                //arrowLine2.Y1 = vectorLine.Y2;
                //arrowLine2.X2 = arrowLine2.X1 + 10 * Cos(alpha - 0.26);
                //arrowLine2.Y2 = arrowLine2.Y1 - 10 * Sin(alpha - 0.26);

                //vectorLine.Stroke = Brushes.Black;
                //arrowLine1.Stroke = Brushes.Black;
                //arrowLine2.Stroke = Brushes.Black;
                //Coil_Grid.Children.Clear();
                */

            }
        }

        private void CoilClick(object sender, MouseButtonEventArgs e)
        {
            Point pointMoveTo;
            pointMoveTo = this.PointToScreen(new Point(e.GetPosition(null).X, e.GetPosition(null).Y));
            double Zc = (e.GetPosition(null).X - Z0)/1000;
            double Pc = (e.GetPosition(null).Y - P0)/1000;
            Zc = Round(Zc, 4);
            Pc = Round(Pc, 4);
            var info = DrawMF.drawVector(X0, X1, Y0, Y1, P0, Z0, Pc, Zc, Coil_Grid, solenoidParams);  //TODO: уменьшить кол-во параметров
            res1.Text = "Z: " + (info[0] * 1000).ToString();
            res2.Text = "P: " + (info[1] * 1000).ToString();
            res3.Text = (info[2] - 90).ToString();
            res3.Text = $"{Sqrt((info[0] * info[0]) + (info[1] * info[1])) * 1000}";
        }

        private void Field_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DrawMF.drawVectorField(X0, X1, Y0, Y1, P0, Z0, 0, 0, Coil_Grid, solenoidParams);  //TODO: уменьшить кол-во параметров
        }
    }

    public class DrawMF
    {  //TODO: сделать контруктор, при инициализации которого буду выдавать начальные параметры (длина, радиус и т.п.)

        public static void drawCoil(int X0, int X1, int Y0, int Y1,  Grid Coil_Grid)
        {
            Polygon coilDraw = new Polygon();
            coilDraw.Points.Add(new Point(X0, Y0));  // левый верхний угол катушки
            coilDraw.Points.Add(new Point(X1, Y0));  // Правый верхний 
            coilDraw.Points.Add(new Point(X1, Y1));  // Правый нижний 
            coilDraw.Points.Add(new Point(X0, Y1));  // Левый нижний 



            coilDraw.Stroke = Brushes.Black;
            Coil_Grid.Children.Add(coilDraw);
        }

        public static double[] drawVector(int X0, int X1, int Y0, int Y1, int P0, int Z0, double po, double z, Grid Coil_Grid, MagnetField.SolenoidParams param)
        {
            Line vectorLine = new Line();
            Line arrowLine1 = new Line();
            Line arrowLine2 = new Line();
            double Bz = 0;
            double Bp = 0;
            double alpha = 0;
            double deltaX = 0;
            double deltaY = 0;

            double[] result = new double[3];

            Bz = MagnetField.CalculateVectorZ(po, z, param);
            Bp = MagnetField.CalculateVectorP(po, z, param);

            vectorLine.X1 = Z0 + z * 1000;
            vectorLine.Y1 = P0 + po * 1000;
            vectorLine.X2 = vectorLine.X1 + (Bz * 1000000); //vertL.X1 + 5;//20
            vectorLine.Y2 = vectorLine.Y1 + (Bp * 1000000);  // vertL.Y1 + 5;//23

            deltaX = vectorLine.X1 - vectorLine.X2;
            deltaY = vectorLine.Y2 - vectorLine.Y1;
            alpha = Atan2(deltaY, deltaX); // угол альфа

            arrowLine1.X1 = vectorLine.X2;  //конец вектора - начала стрелочки 
            arrowLine1.Y1 = vectorLine.Y2;
            arrowLine1.X2 = arrowLine1.X1 + 10 * Cos(alpha + 0.26);
            arrowLine1.Y2 = arrowLine1.Y1 - 10 * Sin(alpha + 0.26);

            arrowLine2.X1 = vectorLine.X2;  //конец вектора - начала стрелочки (вторая стрелочка)
            arrowLine2.Y1 = vectorLine.Y2;
            arrowLine2.X2 = arrowLine2.X1 + 10 * Cos(alpha - 0.26);
            arrowLine2.Y2 = arrowLine2.Y1 - 10 * Sin(alpha - 0.26);


            vectorLine.Stroke = Brushes.Black;
            arrowLine1.Stroke = Brushes.Black;
            arrowLine2.Stroke = Brushes.Black;

            Coil_Grid.Children.Add(vectorLine);  // arrowLine1
            Coil_Grid.Children.Add(arrowLine1);
            Coil_Grid.Children.Add(arrowLine2);

            result[0] = Bz;
            result[1] = Bp;
            result[2] = alpha * 57.296;
            return result;
        }

        public static void drawVectorField(int X0, int X1, int Y0, int Y1, int P0, int Z0, double po, double z, Grid Coil_Grid, MagnetField.SolenoidParams param)
        {
            double Zinit = -0.3;
            double Z = Zinit;
            double Pinit = 0;
            //double P = Pinit;
            for (int i = 0; i < 20; i++)
            {
                for(int j = 0; j < 50; j++)
                {
                    drawVector(X0, X1, Y0, Y1, P0, Z0, Pinit, Z, Coil_Grid, param);
                    Z += 0.025;
                    Z = Round(Z, 4);
                }
                Z = Zinit;
                Pinit += 0.025;
                Pinit = Round(Pinit, 4);
            }
        }
    }




    public class MagnetField
    {
        public static LineSeries resultLine2 = new LineSeries();
        public static LineSeries resultPoint2 = new LineSeries();

        public MagnetField()
        {
            MyModel2 = new PlotModel { Title = "Эксперимент!" };


            resultLine2.LineStyle = LineStyle.Solid;
            resultLine2.Color = OxyColor.FromRgb(200, 0, 0);
            resultLine2.StrokeThickness = 2.0;
            MyModel2.Series.Add(resultLine2);

            resultLine2.Points.Add(new DataPoint(1, 1));
            resultLine2.Points.Add(new DataPoint(10, 10));

            MyModel2.InvalidatePlot(true);


        }

        public struct SolenoidParams
        {
            //public string name;
            public int NLoop;  // кол-во витков
            public double current;  // сила тока, А
            public double length;   // длина катушки
            public double radius;   // радиус катушки (а)
            
        }

        public static void drawGraf(SolenoidParams param)
        {
            resultLine2.Points.Clear();
            //  po - Y
            //  Z  - X
            double Bp = 0;
            int X0 = -400;
            int Xend = 400;
            int Y0 = 0;
            int yend = 1;
            for (int i = X0; i < Xend; i++)
            {
                Bp = CalculateVectorZ(Y0 / 1000.0, i / 1000.0, param);

                resultLine2.Points.Add(new DataPoint(i, Bp));

            }
            MyModel2.InvalidatePlot(true);
        }

        public static void drawGrafZ(double po, double zInit, double zEnd, SolenoidParams param)
        {
            double Bp = 0;
            zEnd = Math.Abs(zInit);
            resultLine2.Points.Clear();
            for (int i = 0; i < (zEnd - zInit); i++)
            {
                Bp = CalculateVectorP(po, (zInit + i)/100.0, param);
                
                resultLine2.Points.Add(new DataPoint(zInit + i, Bp*1000));
                
            }
            MyModel2.InvalidatePlot(true);

        }

        public static void drawBzGrafZ(double po, double zInit, double zEnd, SolenoidParams param)
        {
            double Bp = 0;
            //zEnd = Math.Abs(zInit);
            resultLine2.Points.Clear();
            for (int i = 0; i < (zEnd - zInit); i++)
            {
                Bp = CalculateVectorZ(po, (zInit + i) / 100.0, param);

                resultLine2.Points.Add(new DataPoint(zInit + i, Bp * 1000));

            }
            MyModel2.InvalidatePlot(true);

        }

        public static void drawGrafPo(double poInit, double poEnd, double z, SolenoidParams param)
        {
            double Bp = 0;
            //zEnd = Math.Abs(zInit);
            resultLine2.Points.Clear();
            for (int i = 0; i < (poEnd - poInit)*100; i++)  // 0 - 300
            {
                Bp = CalculateVectorP((poInit + i)/10000, z, param);

                resultLine2.Points.Add(new DataPoint((poInit + i)/100, Bp * 1000));

            }
            MyModel2.InvalidatePlot(true);

        }

        public static void drawBzGrafPo(double poInit, double poEnd, double z, SolenoidParams param)
        {
            double Bp = 0;
            //zEnd = Math.Abs(zInit);
            resultLine2.Points.Clear();
            for (int i = 0; i < (poEnd - poInit) * 100; i++)  // 0 - 300
            {
                Bp = CalculateVectorP((poInit + i) / 100000, z, param);

                resultLine2.Points.Add(new DataPoint((poInit + i) / 1000, Bp * 1000));

            }
            MyModel2.InvalidatePlot(true);

        }

        public static double CalculateVectorP(double po, double z, SolenoidParams param)
        {
            /* для нас это ось Y*/
            //double n = param.NLoop;
            double s = z - param.length / 2.0; // ok  0.05
            double ti = z + param.length / 2.0; // ok  // 0.35
            double cPow = Pow(param.radius, 2) + (2 * param.radius * po);  //ok  // -0.001875
            double mu0 = 1.256637 * Pow(10, -6);

            //double test = Math.Pow(12, 3.0 / 2);

            double part1 = (mu0 * param.NLoop * param.current * Pow(param.radius, 2) * po) / 4.0;  //-5.890485937500001E-09
            double part2 = 1 / Pow((Pow(s, 2) + Pow(po, 2) + cPow), 3.0 / 2);  // 5724.334022399
            double part3 = 1 / Pow((Pow(ti, 2) + Pow(po, 2) +cPow), 3.0 / 2);  // 23.146249

            double Bp = 3 * part1 * (part2 - part3);  // -0.000100748

            return Bp;
        }

        public static double CalculateVectorZ(double po, double z, SolenoidParams param)
        {
            double s = z - param.length / 2.0; // ok  //ро 0.05 0,05
            double ti = z + param.length / 2.0; // ok   // 0,35
            double cPow = Pow(param.radius, 2) + 2 * param.radius * po;  //ok  //-0.001875
            double mu0 = 1.256637 * Pow(10, -6);

            double part1 = (mu0 * param.NLoop * param.current * Pow(param.radius, 2)) / 4.0;  // 1.1780971875000002E-07
            double part2 = Pow(po, 4) - Pow(param.radius, 4) + (Pow(po, 2) * Pow(param.radius, 2)) + (Pow(po, 3) * param.radius) + (3 * po * Pow(param.radius, 3));  //1.953125E-06
            double part3 = Pow(po, 2) + cPow; // 0.000625
            double part4 = Pow((Pow(s, 2) + part3), 3.0 / 2);  // 0.00017469
            double part5 = Pow((Pow(ti, 2) + part3), 3.0 / 2);  // 0.04320

            double Bz = 3* part1 * ( ((-2*cPow* Pow(s, 3)) + part2 * s)/(Pow(part3, 2) * part4 ) - ((-2*cPow* Pow(ti, 3) + part2*ti)/(Pow(part3, 2)*part5))); //-0.000149



            return Bz;
        }

        public static PlotModel MyModel2 { get; private set; }



    }
}