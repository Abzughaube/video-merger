using System;

namespace VideoMerger.ViewModels
{
    public class CropMarks : ViewModelBase
    {
        private TimeSpan start;
        public TimeSpan Start
        {
            get => start;
            set
            {
                start = value;
                OnPropertyChanged();
            } 
        }

        private TimeSpan end;
        public TimeSpan End
        {
            get => end;
            set
            {
                end = value;
                OnPropertyChanged();
            }
        }
    }
}
