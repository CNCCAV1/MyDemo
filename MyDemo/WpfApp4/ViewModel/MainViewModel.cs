using GalaSoft.MvvmLight;
using System;
using System.ComponentModel;
using System.Windows.Media;

namespace WpfApp4.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
                ColorA = 100;
                colorR = 100;
                colorG = 100;
                colorB = 100;

                Title = "²âÊÔ";
            }
            else
            {
                // Code runs "for real"
                Title = "±êÌâ";
                ColorA = 100;
                colorR = 100;
                colorG = 100;
                colorB = 100;
                //bColor = new SolidColorBrush(Color.FromArgb(Convert.ToByte(ColorA), Convert.ToByte(ColorR), Convert.ToByte(ColorG), Convert.ToByte(ColorB)));
            }
        }
        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; RaisePropertyChanged(() => Title); }
        }
        private int colorA;

        public int ColorA
        {
            get { return colorA; }
            set {
                colorA = value; RaisePropertyChanged(() => ColorA);

            }
        }
        private int colorR;

        public int ColorR
        {
            get { return colorR; }
            set { colorR = value; RaisePropertyChanged(() => ColorR); }
        }
        private int colorG;

        public int ColorG
        {
            get { return colorG; }
            set { colorG = value; RaisePropertyChanged(() => ColorG); }
        }
        private int colorB;

        public int ColorB
        {
            get { return colorB; }
            set { colorB = value; RaisePropertyChanged(() => ColorB); }
        }
        private Brush bColor;

        public Brush BColor
        {
            get { return bColor; }
            set { bColor = value; RaisePropertyChanged(() => BColor); }
        }
    }
}