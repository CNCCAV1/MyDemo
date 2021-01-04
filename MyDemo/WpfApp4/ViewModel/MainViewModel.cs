using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

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

                Title = "≤‚ ‘";
                Btn = new Button()
                {
                    Width = 200,
                    Height = 80,
                    Background = Brushes.Red,
                    Content = "∞¢≈£"
                };
            }
            else
            {
                // Code runs "for real"
                Title = "±ÍÃ‚";
                ColorA = 100;
                colorR = 100;
                colorG = 100;
                colorB = 100;
                SliderValue = 0;
                Timer.Tick += new EventHandler(EventTimer);
                Timer.Interval = new TimeSpan(0, 0, 0, 1, 0);
                Timer.Start();
                //bColor = new SolidColorBrush(Color.FromArgb(Convert.ToByte(ColorA), Convert.ToByte(ColorR), Convert.ToByte(ColorG), Convert.ToByte(ColorB)));
            }
        }

        private void EventTimer(object sender, EventArgs e)
        {
            if (SliderValue < 100)
            {
                SliderValue++;
            }
        }

        public static DispatcherTimer Timer = new DispatcherTimer();
        #region  Ù–‘
        private double sliderValue;

        public double SliderValue
        {
            get { return sliderValue; }
            set { sliderValue = value; RaisePropertyChanged(() => SliderValue); }
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
        #endregion
        private string cardId;

        public string CardID
        {
            get { return cardId; }
            set { cardId = value; RaisePropertyChanged(() => CardID); }
        }

        private Button btn;

        public Button Btn
        {
            get { return btn; }
            set { btn = value; RaisePropertyChanged(() => Btn); }
        }

        #region √¸¡Ó
        private RelayCommand<TextBox> loadCommand;

        public RelayCommand<TextBox> LoadCommand
        {
            get
            {
                if (loadCommand == null)
                {
                    loadCommand = new RelayCommand<TextBox>((e) => Load(e));
                }
                return loadCommand;
            }
            set { loadCommand = value; }
        }
        private void Load(TextBox box)
        {
            //if (box != null)
            //{

            //    Keyboard.Focus(box);
            //}
            Class1 class1 = new Class1();
            Class1.BardCodeHooK hooK = new Class1.BardCodeHooK();
            hooK.BarCodeEvent += HooK_BarCodeEvent;
            hooK.Start();
        }
        
        private void HooK_BarCodeEvent(Class1.BardCodeHooK.BarCodes barCode)
        {
            CardID = barCode.BarCode;
        }

        private RelayCommand keyCommand;

        public RelayCommand KeyCommand
        {
            get
            {
                if (keyCommand == null)
                {
                    keyCommand = new RelayCommand(() => Key());
                }
                return keyCommand;
            }
            set { keyCommand = value; }
        }
        private void Key()
        {

        }

        #endregion
    }
}