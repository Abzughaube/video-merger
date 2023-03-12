using System.Collections.ObjectModel;

namespace VideoMerger.ViewModels
{
    public class FileItem : ViewModelBase
    {
        public FileItem()
        {
            this.cropMarksCollection = new ObservableCollection<CropMarks>();
        }

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
    }

}
