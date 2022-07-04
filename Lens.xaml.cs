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
using System.Windows.Media.Animation;

namespace RL
{
    /// <summary>
    /// Логика взаимодействия для Window3.xaml
    /// </summary>
    public partial class Lens : Window
    {
        static private bool startExperiment = false;

        SolidColorBrush colorMouseEnter = (SolidColorBrush)(new BrushConverter().ConvertFrom("#5199FF"));
        SolidColorBrush colorMouseLeave = (SolidColorBrush)(new BrushConverter().ConvertFrom("#B7D4FF"));

        public Lens()
        {
            InitializeComponent();

            logo.Source = new BitmapImage(new Uri($"{MainWindow.resourceAdress}\\Resources\\logo.png"));
            Close.Source = new BitmapImage(new Uri($"{MainWindow.resourceAdress}\\Resources\\Close.png"));
            Home.Source = new BitmapImage(new Uri($"{MainWindow.resourceAdress}\\Resources\\Home.png"));
            Lens_ris.Source = new BitmapImage(new Uri($"{MainWindow.resourceAdress}\\Resources\\Lens_ris.png"));
        }

        static public double Left_Radius_Value = 300;

        static public double Right_Radius_Value = 300;

        static public double Refraction_Value = 1.8;

        static public double LightHeightValue = 3;

        static public double Focal_Value = 180;
        

        private void TextBox_KeyEnterUpdate(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                DependencyProperty prop = TextBox.TextProperty;
                BindingExpression binding = BindingOperations.GetBindingExpression(Light, prop);

                if (binding != null) { binding.UpdateSource(); }



                Left_Radius_Value = double.Parse(Left_Radius.Text);
                Right_Radius_Value = double.Parse(Right_Radius.Text);
                Refraction_Value = double.Parse(Refraction.Text);
                LightHeightValue = double.Parse(Height.Text);


            }
        }

        private void MouseWheelHandler(object sender, MouseWheelEventArgs e)
        {
            // If the mouse wheel delta is positive, move the box up.
            var nameTextBox = (sender as TextBox).Name;
            if (e.Delta > 0)
            {
                if (nameTextBox == "Left_Radius") Left_Radius.Text = $"{Left_Radius_Value += 1}";
                if (nameTextBox == "Right_Radius") Right_Radius.Text = $"{Right_Radius_Value += 1}";
                if (nameTextBox == "Refraction") Refraction.Text = $"{Refraction_Value += 0.05}";
                if (nameTextBox == "Height") Height.Text = $"{LightHeightValue += 1}";

            }
            if (e.Delta < 0)
            {
                if (nameTextBox == "Left_Radius") Left_Radius.Text = $"{Left_Radius_Value -= 1}";
                if (nameTextBox == "Right_Radius") Right_Radius.Text = $"{Right_Radius_Value -= 1}";
                if (nameTextBox == "Refraction") Refraction.Text = $"{Refraction_Value -= 0.1}";
                if (nameTextBox == "Height") Height.Text = $"{LightHeightValue -= 1}";

            }


            if ((Refraction_Value < 1) || (Refraction_Value == 1))
            {
                Focal.Content = "Ошибка: показатель преломления должен быть больше 1!";
            }

            if (Refraction_Value > 1)
            {
                Focal_Value = 1 / ((Refraction_Value - 1) * (1 / Left_Radius_Value + 1 / Right_Radius_Value));
                Focal.Content = "Фокусное расстояние = " + Focal_Value + " мм";
            }

            if ((LightHeightValue < 0) || (LightHeightValue == 0)) Fail.Content = "Ошибка: радиус пучка света должен быть больше 0!";

            if (LightHeightValue > 0)
            {
                Fail.Content = "";
                Light.Height = LightHeightValue;
                Light_z.Height = LightHeightValue;
            }
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

        private void Exp_Click(object sender, RoutedEventArgs e)
        {


            this.Light.Visibility = System.Windows.Visibility.Visible;
            this.Light_z.Visibility = System.Windows.Visibility.Visible;

            Animation.To = 180;

            Angle.Angle = Math.Tan(54 / (Focal_Value * 2));

            Fail.Content = Math.Tan(54 / (Focal_Value * 2));



            DoubleAnimation buttonAnimation = new DoubleAnimation();


            buttonAnimation.From = Light.ActualWidth;
            buttonAnimation.To = 200;
            buttonAnimation.Duration = TimeSpan.FromSeconds(3);
            Light.BeginAnimation(Button.WidthProperty, buttonAnimation);


            //Light_z.Visibility = System.Windows.Visibility.Visible;


            //buttonAnimation.From = (double?)Light_z.VerticalAlignment;
            //buttonAnimation.BeginTime = TimeSpan.FromSeconds(3);
            //buttonAnimation.To = 243;
            //buttonAnimation.Duration = TimeSpan.FromSeconds(3);
            //Light_z.BeginAnimation(Button.WidthProperty, buttonAnimation);



            //this.Experiment.Visibility = System.Windows.Visibility.Collapsed;



            //var transformGroup = new TransformGroup();


            //double time = 3;
            //var scaleTransform = new ScaleTransform()
            //{
            //    CenterX = 300,
            //    CenterY = 300
            //};
            //transformGroup.Children.Add(scaleTransform);

            //double scaleFrom = 0;
            //double scaleTo = 40;

            //var daScaleY = new DoubleAnimation
            //{
            //    From = scaleFrom,
            //    To = scaleTo,
            //    Duration = TimeSpan.FromSeconds(time),

            //};
            //Light_z.BeginAnimation(ScaleTransform.ScaleYProperty, daScaleY);



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


        //Кнопка закрытия окна

        private void Close_MouseDown(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
