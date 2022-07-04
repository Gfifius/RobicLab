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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static string resourceAdress = "";

        SolidColorBrush colorMouseEnter = (SolidColorBrush)(new BrushConverter().ConvertFrom("#5199FF"));
        SolidColorBrush colorMouseLeave = (SolidColorBrush)(new BrushConverter().ConvertFrom("#B7D4FF"));

        public MainWindow()
        {
            InitializeComponent();
            resourceAdress = Environment.CurrentDirectory;
            logo.Source = new BitmapImage(new Uri($"{MainWindow.resourceAdress}\\Resources\\logo.png"));
            Close.Source = new BitmapImage(new Uri($"{MainWindow.resourceAdress}\\Resources\\Close.png"));

            //Magnetic_field.
        }

        private void Magnetic_field_Click(object sender, RoutedEventArgs e)
        {
            Magnetic_field win = new Magnetic_field();
            win.Show();
        }

        private void Capacitor_Click(object sender, RoutedEventArgs e)
        {
            //this.Topmost = true;
            CapWindow win = new CapWindow();
           //win.Owner = this;
            //win.Show();
            //win.a
            
            //this.Close();
        }

        private void Coil_Click(object sender, RoutedEventArgs e)
        {
            CoilWindow win = new CoilWindow();
            win.Show();
        }

        private void Oscillating_circuit_Click(object sender, RoutedEventArgs e)
        {
            Oscillating_circuit win = new Oscillating_circuit();
            win.Show();
        }

        private void Resonance_Click(object sender, RoutedEventArgs e)
        {
            Resonance win = new Resonance();
            win.Show();
        }

        private void Lens_Click(object sender, RoutedEventArgs e)
        {
            Lens win = new Lens();
            win.Show();
        }

        private void Interference_Click(object sender, RoutedEventArgs e)
        {
            Interference win = new Interference();
            win.Show();
        }

        private void Diffraction_Click(object sender, RoutedEventArgs e)
        {
            Diffraction win = new Diffraction();
            win.Show();
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
    }
}
