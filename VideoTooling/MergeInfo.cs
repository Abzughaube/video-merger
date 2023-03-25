using System;
using System.Collections.Generic;

namespace VideoTooling
{
    public class MergeInfo
    {
        public string OutputFilePath { get; set; }
        public IList<VideoInfo> VideoInfos { get; set; } = new List<VideoInfo>();
    }

    public class VideoInfo
    {
        public string InputFilePath { get; set; }

        public IList<CropMarks> CropMarksCollection { get; set; }
    }

    public class CropMarks
    {
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
    }
}
