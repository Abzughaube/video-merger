using System.Collections.ObjectModel;

namespace VideoMerger
{
    public class ViewModel : ViewModelBase
    {
        private ObservableCollection<FileItem> fileItems = new ObservableCollection<FileItem>();
        public ObservableCollection<FileItem> FileItems
        {
            get => fileItems;
            set
            {
                fileItems = value;
                OnPropertyChanged();
            }
        }

        private FileItem selectedItem;
        public FileItem SelectedItem
        {
            get => selectedItem;
            set
            {
                selectedItem = value;
                OnPropertyChanged();
            }
        }
    }
}
