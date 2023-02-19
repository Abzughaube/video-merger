using System;
using System.Diagnostics;
using System.Drawing;

namespace VideoTooling
{
    public class Ffmpeg
    {
        static void MergeFiles(string[] inputFilePaths, string outputPath)
        {
            // Set the path of the output file
            string outputFile = "output.mpg";
            // Create a new process
            var ffmpeg = new Process();
            // Set the filename of ffmpeg.exe
            ffmpeg.StartInfo.FileName = "ffmpeg.exe";
            // Use the concat filter to join the files
            ffmpeg.StartInfo.Arguments = "-f concat -i files.txt -c copy " + outputFile;
            // Create a new file to hold the list of input files
            using (var file = new System.IO.StreamWriter("files.txt"))
            {
                // Write the list of input files to the file
                foreach (var inputFile in inputFilePaths)
                {
                    file.WriteLine("file '" + inputFile + "'");
                }
            }
            // Start the process
            ffmpeg.Start();
            // Wait for the process to complete
            ffmpeg.WaitForExit();
            // Clean up the file
            System.IO.File.Delete("files.txt");
        }

        public static void CreateVideoPreview(string videoFilePath, string previewImagePath, out string processOutput)
        {
            var ffmpeg = new Process();
            ffmpeg.StartInfo.FileName = @"D:\Temp\ffmpeg-5.1.2-full_build\bin\ffmpeg.exe";
            ffmpeg.StartInfo.Arguments = $"-y -i \"{videoFilePath}\" -ss 00:00:01.000 -vframes 1 \"{previewImagePath}\"";
            //ffmpeg.StartInfo.UseShellExecute = false;
            //ffmpeg.StartInfo.CreateNoWindow = true;
            //ffmpeg.StartInfo.RedirectStandardOutput = true;
            //ffmpeg.StartInfo.RedirectStandardError = true;

            ffmpeg.Start();

            var output = string.Empty;
            var error = string.Empty;
            //var output = ffmpeg.StandardOutput.ReadToEnd();
            //var error = ffmpeg.StandardError.ReadToEnd();
            ffmpeg.WaitForExit();
            processOutput = output + Environment.NewLine + error;
            return;
        }

        //public static Bitmap GetVideoPreview(string videoFilePath, string previewImagePath, out string processOutput)
        //{
        //    var ffmpeg = new Process();
        //    ffmpeg.StartInfo.FileName = @"D:\Temp\ffmpeg-5.1.2-full_build\bin\ffmpeg.exe";
        //    ffmpeg.StartInfo.Arguments = $"-y -i \"{videoFilePath}\" -ss 00:00:01.000 -vframes 1 \"{previewImagePath}\"";
        //    //ffmpeg.StartInfo.UseShellExecute = false;
        //    //ffmpeg.StartInfo.CreateNoWindow = true;
        //    //ffmpeg.StartInfo.RedirectStandardOutput = true;
        //    //ffmpeg.StartInfo.RedirectStandardError = true;

        //    ffmpeg.Start();

        //    //var output = ffmpeg.StandardOutput.ReadToEnd();
        //    //var error = ffmpeg.StandardError.ReadToEnd();
        //    ffmpeg.WaitForExit();
        //    processOutput = string.Empty; // output + Environment.NewLine + error;
        //    var bitmap = new BitmapImage(new Uri(previewImagePath));
        //    return bitmap;
        //}
    }
}
