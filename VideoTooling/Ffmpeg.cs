using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace VideoTooling
{
    public class Ffmpeg
    {
        public static void MergeFiles(IEnumerable<string> inputFilePaths, string outputPath, out string shellOutput)
        {
            var ffmpeg = new Process();
            //ffmpeg.StartInfo.UseShellExecute = false;
            //ffmpeg.StartInfo.CreateNoWindow = true;
            //ffmpeg.StartInfo.RedirectStandardOutput = true;
            //ffmpeg.StartInfo.RedirectStandardError = true;
            ffmpeg.StartInfo.FileName = @"D:\Temp\ffmpeg-5.1.2-full_build\bin\ffmpeg.exe";
            ffmpeg.StartInfo.Arguments = "-y -f concat -safe 0 -i files.txt -c copy \"" + outputPath + "\"";
            
            const string parameterFile = "files.txt";
            using (var file = new StreamWriter(parameterFile))
            {
                foreach (var inputFile in inputFilePaths)
                {
                    file.WriteLine("file '" + inputFile + "'");
                }
            }
            ffmpeg.Start();
            shellOutput = string.Empty;
            //shellOutput = ffmpeg.StandardOutput.ReadToEnd() + ffmpeg.StandardError.ReadToEnd();
            ffmpeg.WaitForExit();
            File.Delete(parameterFile);
        }

        public static void CreateVideoPreview(string videoFilePath, string previewImagePath, out string processOutput)
        {
            var ffmpeg = new Process();
            ffmpeg.StartInfo.FileName = Path.Combine(FileSystem.GetAssemblyDirectory(), "ffmpeg.exe");
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
