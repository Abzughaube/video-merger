using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace VideoMerger
{
    /// <summary>
    /// Interaktionslogik für MediaplayerControl.xaml
    /// </summary>
    public partial class MediaplayerControl : UserControl
    {
        CompositeDisposable DisposableSubscriptions = new CompositeDisposable();

        public MediaplayerControl()
        {
            InitializeComponent();
            _timelineSliderObservable = Observable
                .FromEventPattern<RoutedPropertyChangedEventHandler<double>, RoutedPropertyChangedEventArgs<double>>(
                    h => timelineSlider.ValueChanged += h,
                    h => timelineSlider.ValueChanged -= h);
            DisposableSubscriptions.Add(_timelineSliderObservable
                .Where(predicate => IsSliderMovedByUser)
                .Buffer(TimeSpan.FromMilliseconds(500)).Subscribe(onNext));

            timerVideoTime = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(250)
            };
            timerVideoTime.Tick += new EventHandler(timer_Tick);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            timelineSlider.Value = mediaPlayer.Position.TotalMilliseconds;
        }

        private void onNext(IList<EventPattern<RoutedPropertyChangedEventArgs<double>>> values)
        {
            if (!values.Any())
            {
                return;
            }

            var newValue = values.Last().EventArgs.NewValue;

            this.Dispatcher.BeginInvoke((Action)(() =>
            {
                try
                {
                    mediaPlayer.Position = TimeSpan.FromMilliseconds(newValue);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }));

        }

        public static readonly DependencyProperty FilePathProperty =
             DependencyProperty.Register("FilePath", typeof(string), typeof(MediaplayerControl), new
                PropertyMetadata("", new PropertyChangedCallback(OnFilePathChanged)));

        private readonly IObservable<EventPattern<RoutedPropertyChangedEventArgs<double>>> _timelineSliderObservable;
        private DispatcherTimer timerVideoTime;

        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value); }
        }

        private static void OnFilePathChanged(DependencyObject d,
           DependencyPropertyChangedEventArgs e)
        {
            var UserControl1Control = d as MediaplayerControl;
            UserControl1Control.OnFilePathChanged(e);
        }

        private void OnFilePathChanged(DependencyPropertyChangedEventArgs e)
        {
            mediaPlayer.Stop();
            mediaPlayer.Source = new Uri(e.NewValue as string);

            //Binding binding = new Binding("Position"); // CH: does not work because Position does not change while the video is runnig
            //binding.Source = mediaPlayer;               // CH: the other way around it's also not working because there are triggered to many events to be processed per sec
            //binding.Mode = BindingMode.TwoWay;      // CH: last problem: the slider does not move the value to the position the user moved to cursor to but the slider adds 1/removes 1 depending if the cursor is after or before the slider handle
            //binding.Converter = new MillisecondsToMediaElementPositionConverter();
            //BindingOperations.ClearBinding(timelineSlider, Slider.ValueProperty);

            //timelineSlider.SetBinding(Slider.ValueProperty, binding);
        }

        public bool DidUserPlayVideo { get; private set; }
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Play();
            timerVideoTime.Start();
            DidUserPlayVideo = true;
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Pause();
            timerVideoTime.Stop();
            DidUserPlayVideo = false;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
            timerVideoTime.Stop();
            timelineSlider.Value = 0;
            DidUserPlayVideo = false;
        }

        private void MediaPlayer_OnMediaOpened(object sender, RoutedEventArgs e)
        {
            timelineSlider.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;

            timerVideoTime.Start();
        }

        private void MediaPlayer_OnMediaEnded(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
            timerVideoTime.Stop();
            timelineSlider.Value = 0;
            DidUserPlayVideo = false;
        }

        public bool IsSliderMovedByUser { get; private set; }

        private void TimelineSlider_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.mediaPlayer.Pause();
            IsSliderMovedByUser = true;
        }

        private void TimelineSlider_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (DidUserPlayVideo)
            {
                this.mediaPlayer.Play();
            }
            IsSliderMovedByUser = false;
        }
    }

    public class MillisecondsToMediaElementPositionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TimeSpan))
            {
                return DependencyProperty.UnsetValue;
            }
            return ((TimeSpan)value).TotalMilliseconds;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is double))
            {
                return DependencyProperty.UnsetValue;
            }
            return TimeSpan.FromMilliseconds((double)value);
        }
    }
}
