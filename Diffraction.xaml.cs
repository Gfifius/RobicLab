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

namespace RL
{
    /// <summary>
    /// Логика взаимодействия для Oscillating_circuit.xaml
    /// </summary>
    public partial class Diffraction : Window
    {
        static private bool startExperiment = false;

        SolidColorBrush colorMouseEnter = (SolidColorBrush)(new BrushConverter().ConvertFrom("#5199FF"));
        SolidColorBrush colorMouseLeave = (SolidColorBrush)(new BrushConverter().ConvertFrom("#B7D4FF"));

        public Diffraction()
        {
            InitializeComponent();

            logo.Source = new BitmapImage(new Uri($"{MainWindow.resourceAdress}\\Resources\\logo.png"));
            Close.Source = new BitmapImage(new Uri($"{MainWindow.resourceAdress}\\Resources\\Close.png"));
            Home.Source = new BitmapImage(new Uri($"{MainWindow.resourceAdress}\\Resources\\Home.png"));
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
                CapWindowExperiment win = new CapWindowExperiment("Capacitors");
                win.Show();
            }

            //this.Close();
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
