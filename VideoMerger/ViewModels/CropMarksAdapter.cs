using System;
using VideoTooling;

namespace VideoMerger.ViewModels
{
    public class CropMarksAdapter : ViewModelBase
    {
        private CropMarks cropMarks = new CropMarks();
        public TimeSpan Start
        {
            get => cropMarks.Start;
            set
            {
                cropMarks.Start = value;
                OnPropertyChanged();
            } 
        }

        public TimeSpan End
        {
            get => cropMarks.End;
            set
            {
                cropMarks.End = value;
                OnPropertyChanged();
            }
        }

        public CropMarks GetCropMarks => cropMarks;
    }
}
