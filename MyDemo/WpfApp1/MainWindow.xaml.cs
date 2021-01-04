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
using System.Windows.Threading;

namespace WpfApp1
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
        DispatcherTimer timer = new DispatcherTimer();
        IDCardReader reader;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            txt.Text = "";
            if (cbox.SelectionBoxItem.ToString() == "华视")
            {
                reader = new ChinaVisionReader();
            }
            else
            {
                reader = new SynthesisReader();
            }

            reader.OnReadCardCompleted += Reader_OnReadCardCompleted;
            
           box.Text =  reader.Start().ToString();

        }

        private void Reader_OnReadCardCompleted(object sender, ReadCardCompletedEventArgs e)
        {
            txt.Dispatcher.BeginInvoke(new Action(() =>
            {
                txt.Text = e.Name + e.IDC;

            }));
            reader.Stop();
            reader.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

    }
}
