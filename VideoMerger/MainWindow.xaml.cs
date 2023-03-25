using System;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Controls;
using VideoTooling;
using ICSharpCode.SharpZipLib.Zip;
using System.Reflection;
using System.Resources;
using VideoMerger.Helper;
using VideoMerger.ViewModels;

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
            SetupFfmpeg();
        }

        private void SetupFfmpeg()
        {
            var assemblyDirectory = FileSystem.GetAssemblyDirectory();
            if (File.Exists(Path.Combine(assemblyDirectory, "ffmpeg.exe")))
            {
                return;
            }

            try
            {
                ResourceManager objResMgr = new ResourceManager
                    ("VideoMerger.Resource", Assembly.GetExecutingAssembly());
                byte[] objData = (byte[])objResMgr.GetObject("ffmpeg");
                MemoryStream objMS = new MemoryStream(objData);
                ZipInputStream objZIP = new ZipInputStream(objMS);
                ZipEntry theEntry;
                while ((theEntry = objZIP.GetNextEntry()) != null)
                {
                    FileStream streamWriter =
                        File.Create(Path.Combine(assemblyDirectory, theEntry.Name));
                    int size = objData.Length;
                    byte[] data = new byte[size];
                    while (true)
                    {
                        size = objZIP.Read(data, 0, data.Length);
                        if (size > 0)
                        {
                            streamWriter.Write(data, 0, size);
                        }
                        else
                        {
                            break;
                        }
                    }
                    streamWriter.Close();
                }
                objZIP.Close();
            }
            catch (MissingManifestResourceException mmre)
            {
                Console.WriteLine(mmre);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
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
                    Ffmpeg.CreateVideoPreview(file, previewImagePath, out var processOutput);
                    var videoLength = Ffmpeg.ReadVideoLength(file);
                    var fileItem = new FileItem { FilePath = file, PreviewImagePath = previewImagePath, MediaLength = videoLength};
                    LogBox.AppendText(processOutput);
                    ViewModel.FileItems.Add(fileItem);
                }

                ViewModel.SelectedItem = ViewModel.FileItems.FirstOrDefault();
            }

        }

        private void removeInputFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedItem != null)
            {
                var index = ViewModel.FileItems.IndexOf(ViewModel.SelectedItem);
                ViewModel.FileItems.Remove(ViewModel.SelectedItem);
                if (ViewModel.FileItems.Any())
                {
                    ViewModel.SelectedItem = ViewModel.FileItems[index - 1 >= 0 ? index - 1 : index];
                }
            }
        }

        private void mergeButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "MP4 Files (*.mp4)|*.mp4|MPEG Files (*.mpg)|*.mpg|All Files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                var mergeInfo = new MergeInfo
                {
                    OutputFilePath = saveFileDialog.FileName,
                    VideoInfos = ViewModel.FileItems.Select(fi => new VideoInfo
                    {
                        InputFilePath = fi.FilePath,
                        CropMarksCollection = fi.CropMarksCollection.Select(cm => cm.GetCropMarks).ToList()
                    }).ToList()
                };

                Ffmpeg.MergeFiles(mergeInfo, out var shellOutput);
            }
        }
        private void moveLeftButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedItem != null)
            {
                var index = ViewModel.FileItems.IndexOf(ViewModel.SelectedItem);
                if (index == 0)
                {
                    return;
                }
                var temp = ViewModel.SelectedItem;
                ViewModel.FileItems.Remove(ViewModel.SelectedItem);
                ViewModel.FileItems.Insert(index - 1, temp);
                ViewModel.SelectedItem = temp;
            }
        }
        private void moveRightButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedItem != null && ViewModel.FileItems.IndexOf(ViewModel.SelectedItem) < ViewModel.FileItems.Count - 1)
            {
                var index = ViewModel.FileItems.IndexOf(ViewModel.SelectedItem);
                var temp = ViewModel.SelectedItem;
                ViewModel.FileItems.Remove(ViewModel.SelectedItem);
                ViewModel.FileItems.Insert(index + 1, temp);
                ViewModel.SelectedItem = temp;
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
                ViewModel.SelectedItem = fileItem;
            }
        }

    }
}
