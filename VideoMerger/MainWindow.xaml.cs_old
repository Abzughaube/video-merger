using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using Microsoft.Win32;
using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Linq;

namespace VideoMerger
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //private void addInputFileButton_Click(object sender, RoutedEventArgs e)
        //{
        //    var openFileDialog = new OpenFileDialog();
        //    openFileDialog.Filter = "MP4 Files (*.mp4)|*.mp4|MPEG Files (*.mpg)|*.mpg|All Files (*.*)|*.*";
        //    openFileDialog.Multiselect = true;
        //    if (openFileDialog.ShowDialog() == true)
        //    {
        //        foreach (var file in openFileDialog.FileNames)
        //        {
        //            inputFilesListView.Items.Add(file);
        //        }
        //    }
        //}

        //private List<Bitmap> _previewImages = new List<Bitmap>();

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
                    inputFilesListView.Items.Add(fileItem);
                }
            }

        }

        private void removeInputFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (inputFilesListView.SelectedIndex != -1)
            {
                inputFilesListView.Items.RemoveAt(inputFilesListView.SelectedIndex);
            }
        }

        private void mergeButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "MP4 Files (*.mp4)|*.mp4|MPEG Files (*.mpg)|*.mpg|All Files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                // Create a new file to hold the list of input files
                using (var file = new StreamWriter("files.txt"))
                {
                    // Write the list of input files to the file
                    foreach (var inputFile in inputFilesListView.Items.Cast<FileItem>())
                    {
                        file.WriteLine("file '" + inputFile.FilePath + "'");
                    }
                }

                var ffmpeg = new Process();
                //ffmpeg.StartInfo.UseShellExecute = false;
                //ffmpeg.StartInfo.CreateNoWindow = true;
                ffmpeg.StartInfo.RedirectStandardOutput = true;
                ffmpeg.StartInfo.RedirectStandardError = true;
                ffmpeg.StartInfo.FileName = @"D:\Temp\ffmpeg-5.1.2-full_build\bin\ffmpeg.exe";
                ffmpeg.StartInfo.Arguments = "-y -f concat -safe 0 -i files.txt -c copy \"" + saveFileDialog.FileName + "\"";

                ffmpeg.Start();
                var output = ffmpeg.StandardOutput.ReadToEnd();
                var error = ffmpeg.StandardError.ReadToEnd();
                LogBox.Text = output + Environment.NewLine + error;
                ffmpeg.WaitForExit();

                System.IO.File.Delete("files.txt");
            }
        }
        private void moveUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (inputFilesListView.SelectedIndex > 0)
            {
                var selected = inputFilesListView.SelectedItem;
                var index = inputFilesListView.SelectedIndex;
                inputFilesListView.Items.RemoveAt(index);
                inputFilesListView.Items.Insert(index - 1, selected);
                inputFilesListView.SelectedIndex = index - 1;
            }
        }
        private void moveDownButton_Click(object sender, RoutedEventArgs e)
        {
            if (inputFilesListView.SelectedIndex != -1 && inputFilesListView.SelectedIndex < inputFilesListView.Items.Count - 1)
            {
                var selected = inputFilesListView.SelectedItem;
                var index = inputFilesListView.SelectedIndex;
                inputFilesListView.Items.RemoveAt(index);
                inputFilesListView.Items.Insert(index + 1, selected);
                inputFilesListView.SelectedIndex = index + 1;
            }
        }

    }
}
