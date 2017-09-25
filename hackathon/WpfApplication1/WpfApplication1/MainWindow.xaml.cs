using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using System.Windows.Threading;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        private string _timestamp;
        public string TimeStamp
        {
            get { return _timestamp; }
            set
            {
                _timestamp = value;
                RaisePropertyChanged("TimeStamp");
            }
        }

        DispatcherTimer _timer;
        public ObservableCollection<JobMedia> JobMedia { get; set; }
        public bool IsPlaying { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Slider.ValueChanged += slider_ValueChanged;
            Play.Click += play_Click;
            Stop.Click += stop_Click;
          
        }



        void stop_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            IsPlaying = false;
        }

        void play_Click(object sender, RoutedEventArgs e)
        {
            IsPlaying = true;
            _timer.Start();
        }

        void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsPlaying)
            {
                Slider.DataContext = JobMedia[(int)Slider.Value];
                TimeStamp = JobMedia[(int) Slider.Value].TimeStamp;
            }
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            JobMedia = new ObservableCollection<JobMedia>();

            for (int i = 0; i < 20; i++)
            {
                JobMedia.Add(new JobMedia { Title = "This is: " + i, Progress = i, TimeStamp = DateTime.Now.AddDays(i).ToShortDateString(), Image = System.Drawing.Image.FromFile(@"C:\Users\Ravikanth.Kaja\Desktop\Images\con1.jpg"), });
            }

            _timer = new DispatcherTimer();
            _timer.Tick += timer_Tick;
            _timer.Interval = new TimeSpan(0, 0, 0, 2);

            Slider.Minimum = 0;
            Slider.Maximum = JobMedia.Count - 1;

            _timestamp = "";

            var track = Slider.Template.FindName("Thumb", Slider) as Thumb;
            AttachLoadingAdorner(track);

        }

        void timer_Tick(object sender, EventArgs e)
        {
            Slider.DataContext = JobMedia[(int)Slider.Value + 1];
            TimeStamp = JobMedia[(int)Slider.Value].TimeStamp;
        }

        private void AttachLoadingAdorner(FrameworkElement element)
        {
            LoadingAdorner loading = new LoadingAdorner(element);
            loading.FontSize = 15;
            loading.Typeface = new Typeface(FontFamily, FontStyles.Italic,
                FontWeights.Bold, FontStretch);
            Binding bind = new Binding("TimeStamp");
            bind.Source = this;
            loading.SetBinding(LoadingAdorner.OverlayedTextProperty, bind);
            AdornerLayer.GetAdornerLayer(element).Add(loading);
        }
    }

    public class NotifiedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }


    public class JobMedia : NotifiedBase
    {
        public BitmapImage Image;
        public string Title { get; set; }
        private int _progress;
        public int Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                RaisePropertyChanged("Progress");
            }
        }

        public string TimeStamp { get; set; }
    }
}
