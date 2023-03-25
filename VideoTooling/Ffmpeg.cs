using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace VideoTooling
{
    public class Ffmpeg
    {
        public static void MergeFiles(MergeInfo mergeInfo, out string shellOutput)
        {
            var fileParametersStringBuilder = new StringBuilder();
            foreach (var videoInfo in mergeInfo.VideoInfos)
            {
                fileParametersStringBuilder.AppendLine($"file '{videoInfo.InputFilePath}'");
            }

            var mergeFilePath = "files.txt";
            File.WriteAllText(mergeFilePath, fileParametersStringBuilder.ToString());

            var ffmpeg = new Process();
            //ffmpeg.StartInfo.UseShellExecute = false;
            //ffmpeg.StartInfo.CreateNoWindow = true;
            //ffmpeg.StartInfo.RedirectStandardOutput = true;
            //ffmpeg.StartInfo.RedirectStandardError = true;
            ffmpeg.StartInfo.FileName = @"D:\Temp\ffmpeg-5.1.2-full_build\bin\ffmpeg.exe";
            ffmpeg.StartInfo.Arguments = $"-y -f concat -safe 0 -i {mergeFilePath} -c copy \"" + mergeInfo.OutputFilePath + "\"";

            ffmpeg.Start();
            shellOutput = string.Empty;
            //shellOutput = ffmpeg.StandardOutput.ReadToEnd() + ffmpeg.StandardError.ReadToEnd();
            ffmpeg.WaitForExit();

            File.Delete(mergeFilePath);
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
        }

        public static TimeSpan ReadVideoLength(string videoFilePath)
        {
            var ffprobe = new Process();
            ffprobe.StartInfo.FileName = Path.Combine(FileSystem.GetAssemblyDirectory(), "ffprobe.exe");
            ffprobe.StartInfo.Arguments = $"-i \"{videoFilePath}\" -show_entries format=duration -v quiet -of csv=\"p=0\"";
            ffprobe.StartInfo.UseShellExecute = false;
            ffprobe.StartInfo.CreateNoWindow = true;
            ffprobe.StartInfo.RedirectStandardOutput = true;
            ffprobe.StartInfo.RedirectStandardError = true;
            ffprobe.Start();

            var output = ffprobe.StandardOutput.ReadToEnd();
            var error = ffprobe.StandardError.ReadToEnd();

            ffprobe.WaitForExit();

            var videoLengthInSeconds = double.Parse(output.Trim(), CultureInfo.InvariantCulture);
            return TimeSpan.FromSeconds(videoLengthInSeconds);
        }
    }
}
