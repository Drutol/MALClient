using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight;
using MALClient.Models;

namespace MALClient.ViewModels
{
    public class PinTileDialogViewModel : ViewModelBase
    {
        private Visibility _urlInputVisibility = Visibility.Visible;

        public Visibility UrlInputVisibility
        {
            get
            {
                return _urlInputVisibility;
            }
            set
            {
                _urlInputVisibility = value;
                RaisePropertyChanged(() => UrlInputVisibility);
            }
        }

        private Visibility _imagePreviewVisibility = Visibility.Visible;

        public Visibility ImagePreviewVisibility
        {
            get
            {
                return _imagePreviewVisibility;
            }
            set
            {
                _imagePreviewVisibility = value;
                RaisePropertyChanged(() => ImagePreviewVisibility);
            }
        }

        private int _selectedActionIndex = 0;

        public int SelectedActionIndex
        {
            get { return _selectedActionIndex; }
            set
            {
                _selectedActionIndex = value;
                UrlInputVisibility = value == 0 ? Visibility.Visible : Visibility.Collapsed;
                RaisePropertyChanged(() => SelectedActionIndex);
            }
        }

        private int _selectedImageOptionIndex = 0;

        public int SelectedImageOptionIndex
        {
            get { return _selectedImageOptionIndex; }
            set
            {
                _selectedImageOptionIndex = value;
                ImagePreviewVisibility = value != 2 ? Visibility.Visible : Visibility.Collapsed;
                RaisePropertyChanged(() => SelectedImageOptionIndex);
            }
        }

        public void Load(ILibraryData data)
        {
            
        }
    }
}
