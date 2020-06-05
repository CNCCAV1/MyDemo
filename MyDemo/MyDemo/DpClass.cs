using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyDemo
{
    public class DpClass:DependencyObject
    {


        public static Thickness GetMyThickness(DependencyObject obj)
        {
            return (Thickness)obj.GetValue(MyThicknessProperty);
        }

        public static void SetMyThickness(DependencyObject obj, Thickness value)
        {
            obj.SetValue(MyThicknessProperty, value);
        }

        // Using a DependencyProperty as the backing store for MyThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MyThicknessProperty =
            DependencyProperty.RegisterAttached("MyThickness", typeof(Thickness), typeof(DpClass), new PropertyMetadata(new Thickness()));



        public static double GetMyR(DependencyObject obj)
        {
            return (double)obj.GetValue(MyRProperty);
        }

        public static void SetMyR(DependencyObject obj, double value)
        {
            obj.SetValue(MyRProperty, value);
        }

        // Using a DependencyProperty as the backing store for MyR.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MyRProperty =
            DependencyProperty.RegisterAttached("MyR", typeof(double), typeof(DpClass), new PropertyMetadata((double)100));


    }
}
