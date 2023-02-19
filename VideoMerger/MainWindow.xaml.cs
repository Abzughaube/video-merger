using System;
using System.Diagnostics;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using VideoTooling;

namespace VideoMerger
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal ViewModel ViewModel { get; set; } = new ViewModel();

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = ViewModel;
        }

        private void addInputFileButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "MP4 Files (*.mp4)|*.mp4|MPEG Files (*.mpg)|*.mpg|All Files (*.*)|*.*";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (var file in openFileDialog.FileNames)
                {
                    string previewImagePath = Path.ChangeExtension(file, ".jpg");
                    VideoTooling.Ffmpeg.CreateVideoPreview(file, previewImagePath, out var processOutput);
                    var fileItem = new FileItem { FilePath = file, PreviewImagePath = previewImagePath };
                    LogBox.AppendText(processOutput);
                    ViewModel.FileItems.Add(fileItem);
                }
            }

        }

        private void removeInputFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedItem != null)
            {
                ViewModel.FileItems.Remove(ViewModel.SelectedItem);
            }
        }

        private void mergeButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "MP4 Files (*.mp4)|*.mp4|MPEG Files (*.mpg)|*.mpg|All Files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                Ffmpeg.MergeFiles(ViewModel.FileItems.Select(fi => fi.FilePath), saveFileDialog.FileName, out var shellOutput);
            }
        }
        private void moveUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedItem != null)
            {
                var index = ViewModel.FileItems.IndexOf(ViewModel.SelectedItem);
                ViewModel.FileItems.Remove(ViewModel.SelectedItem);
                ViewModel.FileItems.Insert(index - 1, ViewModel.SelectedItem);
                ViewModel.SelectedItem = ViewModel.SelectedItem;
            }
        }
        private void moveDownButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedItem != null && ViewModel.FileItems.IndexOf(ViewModel.SelectedItem) < ViewModel.FileItems.Count - 1)
            {
                var index = ViewModel.FileItems.IndexOf(ViewModel.SelectedItem);
                ViewModel.FileItems.Remove(ViewModel.SelectedItem);
                ViewModel.FileItems.Insert(index + 1, ViewModel.SelectedItem);
                ViewModel.SelectedItem = ViewModel.SelectedItem;
            }
        }

        private System.Windows.Point _startPoint;
        private void inputFilesListView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
        }
        private void inputFilesListView_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && (Math.Abs(e.GetPosition(null).X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(e.GetPosition(null).Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                if (ViewModel.SelectedItem != null)
                {
                    var data = new DataObject("FileItem", ViewModel.SelectedItem);
                    DragDrop.DoDragDrop(sender as ListView, data, DragDropEffects.Move);
                }
            }
        }

        private void inputFilesListView_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("FileItem"))
            {
                var fileItem = (FileItem)e.Data.GetData("FileItem");
                var droppedOn = UIHelper.FindVisualParent<ListViewItem>(sender as DependencyObject);
                if (droppedOn == null) { return; }
                var droppedFileItem = droppedOn.DataContext as FileItem;
                var droppedIndex = this.ViewModel.FileItems.IndexOf(droppedFileItem);

                ViewModel.FileItems.Remove(fileItem);
                ViewModel.FileItems.Insert(droppedIndex, fileItem);
            }
        }
    }
}
