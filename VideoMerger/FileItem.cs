namespace VideoMerger
{
    public class FileItem : ViewModelBase
    {
        private string filePath;
        private string previewImagePath;

        public string FilePath
        {
            get => filePath; set
            {
                filePath = value;
                OnPropertyChanged();
            }
        }

        public string PreviewImagePath
        {
            get => previewImagePath; set
            {
                previewImagePath = value;
                OnPropertyChanged();
            }
        }
    }

}
