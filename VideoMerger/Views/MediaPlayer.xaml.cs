﻿using System;
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
            TimelineSlider.Value = mediaPlayer.Position.TotalMilliseconds;
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
             DependencyProperty.Register(nameof(FilePath), typeof(string), typeof(MediaPlayer), new
                PropertyMetadata("", new PropertyChangedCallback(OnFilePathChanged)));

        public static readonly DependencyProperty CropMarksCollectionProperty =
            DependencyProperty.Register(nameof(CropMarksCollection), typeof(ObservableCollection<CropMarks>), typeof(MediaPlayer),
                new PropertyMetadata(null, new PropertyChangedCallback(OnCropMarksCollectionChanged)));

        public string FilePath
        {
            get => (string)GetValue(FilePathProperty);
            set => SetValue(FilePathProperty, value);
        }

        public ObservableCollection<CropMarks> CropMarksCollection
        {
            get => (ObservableCollection<CropMarks>)GetValue(CropMarksCollectionProperty);
            set => SetValue(CropMarksCollectionProperty, value);
        }

        private static void OnFilePathChanged(DependencyObject d,
           DependencyPropertyChangedEventArgs e)
        {
            var userControl1Control = d as MediaPlayer;
            userControl1Control.OnFilePathChanged(e);
        }

        private static void OnCropMarksCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var userControl1Control = d as MediaPlayer;
        }

        private void OnFilePathChanged(DependencyPropertyChangedEventArgs e)
        {
            mediaPlayer.Stop();
            mediaPlayer.Source = new Uri(e.NewValue as string);
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
            TimelineSlider.Value = 0;
            DidUserPlayVideo = false;
        }

        private void MediaPlayer_OnMediaOpened(object sender, RoutedEventArgs e)
        {
            TimelineSlider.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;

            timerVideoTime.Start();
        }

        private void MediaPlayer_OnMediaEnded(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
            timerVideoTime.Stop();
            TimelineSlider.Value = 0;
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

        private void AddMarksButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.CropMarksCollection.Add(new CropMarks
            {
                Start = TimeSpan.Zero, End = TimeSpan.FromMilliseconds(TimelineSlider.Maximum)
            });
        }
    }
}
