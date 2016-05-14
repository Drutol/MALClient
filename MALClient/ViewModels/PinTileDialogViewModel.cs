using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Models;
using WinRTXamlToolkit.AwaitableUI;
using WinRTXamlToolkit.Imaging;

namespace MALClient.ViewModels
{
    public class PinTileDialogViewModel : ViewModelBase
    {
        private Visibility _generalVisibility = Visibility.Collapsed;

        public Visibility GeneralVisibility
        {
            get { return _generalVisibility; }
            set
            {
                _generalVisibility = value;
                RaisePropertyChanged(() => GeneralVisibility);
            }
        }

        private Visibility _urlInputVisibility = Visibility.Visible;

        public Visibility UrlInputVisibility
        {
            get { return _urlInputVisibility; }
            set
            {
                _urlInputVisibility = value;
                RaisePropertyChanged(() => UrlInputVisibility);
            }
        }

        private Visibility _imagePreviewVisibility = Visibility.Visible;

        public Visibility ImagePreviewVisibility
        {
            get { return _imagePreviewVisibility; }
            set
            {
                _imagePreviewVisibility = value;
                RaisePropertyChanged(() => ImagePreviewVisibility);
            }
        }

        private Visibility _undoCropVisibility = Visibility.Collapsed;

        public Visibility UndoCropVisibility
        {
            get { return _undoCropVisibility; }
            set
            {
                _undoCropVisibility = value;
                RaisePropertyChanged(() => UndoCropVisibility);
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

        public AnimeItemViewModel EntryData { get; set; }

        private BitmapImage _previewImage;

        public BitmapImage PreviewImage
        {
            get { return _previewImage; }
            set
            {
                _previewImage = value;
                RaisePropertyChanged(() => PreviewImage);
            }
        }

        private ICommand _closeDialogCommand;

        public ICommand CloseDialogCommand => _closeDialogCommand ?? (_closeDialogCommand = new RelayCommand(() =>
        {
            GeneralVisibility = Visibility.Collapsed;
            EntryData = null;
            PreviewImage = null;
        }));

        private ICommand _cropImageCommand;

        public ICommand CropImageCommand => _cropImageCommand ?? (_cropImageCommand = new RelayCommand(CropImage));

        public ICommand _undoCropCommand;
        public ICommand UndoCropCommand => _undoCropCommand ?? (_undoCropCommand = new RelayCommand(ResetCrop));

        public int CropTop { get; set; }
        public int CropLeft { get; set; }
        public int CropWidth { get; set; }
        public int CropHeight { get; set; }



        public void Load(AnimeItemViewModel data)
        {
            GeneralVisibility = Visibility.Visible;

            EntryData = data;
            ResetCrop();
            RaisePropertyChanged(() => EntryData);
        }

        private string _lastCroppedFileName;
        private async void CropImage()
        {
            var img = PreviewImage;
            WriteableBitmap resizedBitmap = new WriteableBitmap(CropWidth,CropHeight);
            if (!img.UriSource.ToString().Contains("ms-appdata"))
                await resizedBitmap.LoadFromBitmapImageSourceAsync(img);
            else               
                await resizedBitmap.LoadAsync((await ApplicationData.Current.TemporaryFolder.GetFilesAsync(CommonFileQuery.DefaultQuery)).First(storageFile => storageFile.Name == _lastCroppedFileName));
            resizedBitmap = resizedBitmap.Crop(CropLeft, CropTop, CropWidth + CropLeft, CropTop + CropHeight);
            var file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("_cropTemp.png", CreationCollisionOption.GenerateUniqueName);
            _lastCroppedFileName = file.Name;
            await resizedBitmap.SaveAsync(file, BitmapEncoder.PngEncoderId);
            PreviewImage = new BitmapImage(new Uri($"ms-appdata:///temp/{_lastCroppedFileName}"));
            UndoCropVisibility = Visibility.Visible;
        }

        private void ResetCrop()
        {
            PreviewImage = new BitmapImage(new Uri(EntryData.ImgUrl));
            UndoCropVisibility = Visibility.Collapsed;;
        }
    }
}
