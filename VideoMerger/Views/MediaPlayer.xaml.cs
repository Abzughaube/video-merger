using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using VideoMerger.ViewModels;

namespace VideoMerger.Views
{
    /// <summary>
    /// Interaktionslogik für MediaPlayer.xaml
    /// </summary>
    public partial class MediaPlayer : UserControl
    {
        CompositeDisposable DisposableSubscriptions = new CompositeDisposable();
        private readonly IObservable<EventPattern<RoutedPropertyChangedEventArgs<double>>> _timelineSliderObservable;
        private DispatcherTimer timerVideoTime;

        public MediaPlayer()
        {
            InitializeComponent();

            _timelineSliderObservable = Observable
                .FromEventPattern<RoutedPropertyChangedEventHandler<double>, RoutedPropertyChangedEventArgs<double>>(
                    h => TimelineSlider.ValueChanged += h,
                    h => TimelineSlider.ValueChanged -= h);
            DisposableSubscriptions.Add(_timelineSliderObservable
                .Where(predicate => IsSliderMovedByUser)
                .Buffer(TimeSpan.FromMilliseconds(500)).Subscribe(OnNext));

            timerVideoTime = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(250)
            };
            timerVideoTime.Tick += new EventHandler(timer_Tick);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            TimelineSlider.Value = MediaElement.Position.TotalMilliseconds;
        }

        private void OnNext(IList<EventPattern<RoutedPropertyChangedEventArgs<double>>> values)
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
                    MediaElement.Position = TimeSpan.FromMilliseconds(newValue);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }));

        }

        public static readonly DependencyProperty FileItemProperty =
            DependencyProperty.Register(nameof(FileItem), typeof(FileItem), typeof(MediaPlayer),
                new PropertyMetadata(null, new PropertyChangedCallback(OnFileItemChanged)));

        private static void OnFileItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var mediaPlayer = d as MediaPlayer;
            mediaPlayer.MediaElement.Stop();
            mediaPlayer.MediaElement.Source = new Uri(((FileItem)e.NewValue).FilePath);
        }

        public FileItem FileItem
        {
            get => (FileItem)GetValue(FileItemProperty);
            set => SetValue(FileItemProperty, value);
        }

        public bool DidUserPlayVideo { get; private set; }
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            MediaElement.Play();
            timerVideoTime.Start();
            DidUserPlayVideo = true;
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            MediaElement.Pause();
            timerVideoTime.Stop();
            DidUserPlayVideo = false;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            MediaElement.Stop();
            timerVideoTime.Stop();
            TimelineSlider.Value = 0;
            DidUserPlayVideo = false;
        }

        private void MediaElement_OnMediaOpened(object sender, RoutedEventArgs e)
        {
            FileItem.MediaLength = MediaElement.NaturalDuration.TimeSpan;
            TimelineSlider.Maximum = FileItem.MediaLength.TotalMilliseconds;

            timerVideoTime.Start();
        }

        private void MediaElement_OnMediaEnded(object sender, RoutedEventArgs e)
        {
            MediaElement.Stop();
            timerVideoTime.Stop();
            TimelineSlider.Value = 0;
            DidUserPlayVideo = false;
        }

        public bool IsSliderMovedByUser { get; private set; }

        private void TimelineSlider_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.MediaElement.Pause();
            IsSliderMovedByUser = true;
        }

        private void TimelineSlider_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (DidUserPlayVideo)
            {
                this.MediaElement.Play();
            }
            IsSliderMovedByUser = false;
        }

        private void AddMarksButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.FileItem.CropMarksCollection.Add(new CropMarks
            {
                Start = TimeSpan.Zero, End = TimeSpan.FromMilliseconds(TimelineSlider.Maximum)
            });
        }
    }
}
