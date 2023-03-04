using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml.Linq;

namespace VideoMerger
{
    /// <summary>
    /// Interaktionslogik für MediaplayerControl.xaml
    /// </summary>
    public partial class MediaplayerControl : UserControl
    {
        public MediaplayerControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty FilePathProperty =
             DependencyProperty.Register("FilePath", typeof(string), typeof(MediaplayerControl), new
                PropertyMetadata("", new PropertyChangedCallback(OnFilePathChanged)));

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
            mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;

            Binding binding = new Binding("Position"); // CH: does not work because Position does not change while the video is runnig
            binding.Source = mediaPlayer;               // CH: the other way around it's also not working because there are triggered to many events to be processed per sec
            binding.Mode = BindingMode.TwoWay;      // CH: last problem: the slider does not move the value to the position the user moved to cursor to but the slider adds 1/removes 1 depending if the cursor is after or before the slider handle
            binding.Converter = new MillisecondsToMediaElementPositionConverter();
            BindingOperations.ClearBinding(timelineSlider, Slider.ValueProperty);

            timelineSlider.SetBinding(Slider.ValueProperty, binding);
        }

        private void MediaPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            timelineSlider.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;
            mediaPlayer.MediaOpened -= MediaPlayer_MediaOpened;
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Play();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Pause();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
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
