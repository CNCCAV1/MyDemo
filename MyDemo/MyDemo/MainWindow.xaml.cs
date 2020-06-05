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

namespace MyDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            Button btn = sender as Button;
            Point point = e.GetPosition(btn);
            Console.WriteLine(point.X + "||" + point.Y);
            double X = Math.Round(point.X / btn.ActualWidth, 2);
            double Y = Math.Round(point.Y / btn.ActualHeight, 2);
            btn.RenderTransformOrigin = new Point() { X = X, Y = Y };
            //计算遮罩层圆半径
            double R;
            if (point.Y > (btn.ActualHeight /2))
            {
                     R= Math.Round(Math.Sqrt(Math.Pow((btn.ActualWidth - point.X), 2) + Math.Pow(point.Y, 2)), 2);
            }
            else
            {
                R= Math.Round(Math.Sqrt(Math.Pow((btn.ActualHeight - point.Y), 2) + Math.Pow(point.X, 2)), 2);
            }
            //圆形遮罩的位置
            Thickness thickness = new Thickness() { Left = (point.X - R), Top = (point.Y - R), Right = btn.ActualWidth - point.X  - R, Bottom = btn.ActualHeight-point.Y -R };
            Console.WriteLine("X:" + X + ",Y:" + Y);
            Console.WriteLine("半径:" +R);
            Console.WriteLine("Left:" + thickness.Left + ",Top:" + thickness.Top);

            DpClass.SetMyThickness(btn, thickness);
            DpClass.SetMyR(btn, R*2);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }
    }
}
