using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using VideoMerger.Helper;

namespace VideoMerger.ViewModels
{
    public class FileItem : ViewModelBase
    {
        public FileItem()
        {
            this.cropMarksCollection = new ObservableCollection<CropMarks>();
        }

        public ICommand AddCropMarksCommand =>
            new RelayCommand(_ =>
                CropMarksCollection.Add(new CropMarks
                    { Start = TimeSpan.Zero, End = MediaLength }),
                _ => CropMarksCollection.Count < 1);

        private string filePath;
        public string FilePath
        {
            get => filePath; set
            {
                filePath = value;
                OnPropertyChanged();
            }
        }

        private string previewImagePath;
        public string PreviewImagePath
        {
            get => previewImagePath; set
            {
                previewImagePath = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<CropMarks> cropMarksCollection;
        public ObservableCollection<CropMarks> CropMarksCollection
        {
            get => cropMarksCollection;
            set
            {
                cropMarksCollection = value;
                OnPropertyChanged();
            }
        }

        private TimeSpan mediaLength;
        public TimeSpan MediaLength
        {
            get => mediaLength;
            set
            {
                mediaLength = value;
                OnPropertyChanged();
            }
        }
    }

}
