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

        private Visibility _undoWideCropVisibility = Visibility.Collapsed;

        public Visibility UndoWideCropVisibility
        {
            get { return _undoWideCropVisibility; }
            set
            {
                _undoWideCropVisibility = value;
                RaisePropertyChanged(() => UndoWideCropVisibility);
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

        public string TargetUrl { get; set; }
        

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

        private BitmapImage _previewImageWide;
        private BitmapImage _previewImageNormal;

        public BitmapImage PreviewImageNormal
        {
            get { return _previewImageNormal; }
            set
            {
                _previewImageNormal = value;
                RaisePropertyChanged(() => PreviewImageNormal);
            }
        }

        public BitmapImage PreviewImageWide
        {
            get { return _previewImageWide; }
            set
            {
                _previewImageWide = value;
                RaisePropertyChanged(() => PreviewImageWide);
            }
        }

        private ICommand _closeDialogCommand;

        public ICommand CloseDialogCommand => _closeDialogCommand ?? (_closeDialogCommand = new RelayCommand(() =>
        {
            GeneralVisibility = Visibility.Collapsed;
            EntryData = null;
            PreviewImageNormal = PreviewImageWide = null;
        }));

        private ICommand _cropImageCommand;

        public ICommand CropImageCommand => _cropImageCommand ?? (_cropImageCommand = new RelayCommand(() => CropImage()));

        private ICommand _undoCropCommand;
        public ICommand UndoCropCommand => _undoCropCommand ?? (_undoCropCommand = new RelayCommand(() => ResetCrop()));

        private ICommand _pinThingCommand;
        public ICommand PinThingCommand => _pinThingCommand ?? (_pinThingCommand = new RelayCommand(PinThing));

        private ICommand _cropWideImageCommand;

        public ICommand CropWideImageCommand => _cropWideImageCommand ?? (_cropWideImageCommand = new RelayCommand(() => CropImage(true)));

        private ICommand _undoWideCropCommand;
        public ICommand UndoWideCropCommand => _undoWideCropCommand ?? (_undoWideCropCommand = new RelayCommand(() => ResetCrop(true)));


        public int CropTop { get; set; }
        public int CropLeft { get; set; }
        public int CropWidth { get; set; }
        public int CropHeight { get; set; }

        public int CropTopWide { get; set; }
        public int CropLeftWide { get; set; }
        public int CropWidthWide { get; set; }
        public int CropHeightWide { get; set; }


        public void Load(AnimeItemViewModel data)
        {
            GeneralVisibility = Visibility.Visible;
            _lastCroppedFileName = null;
            _lastCroppedFileNameWide = null;
            EntryData = data;
            ResetCrop();
            ResetCrop(true);
            RaisePropertyChanged(() => EntryData);
        }

        private string _lastCroppedFileName;
        private string _lastCroppedFileNameWide;
        private async void CropImage(bool wide = false)
        {
            var img = wide ? _previewImageWide : _previewImageNormal;
            WriteableBitmap resizedBitmap = new WriteableBitmap(CropWidth,CropHeight);
            if (!img.UriSource.ToString().Contains("ms-appdata"))
                await resizedBitmap.LoadFromBitmapImageSourceAsync(img);
            else               
                await resizedBitmap.LoadAsync((await ApplicationData.Current.LocalFolder.GetFilesAsync(CommonFileQuery.DefaultQuery)).First(storageFile => storageFile.Name == _lastCroppedFileName));

            if(wide)
                resizedBitmap = resizedBitmap.Crop(CropLeftWide, CropTopWide, CropWidthWide + CropLeftWide, CropTopWide + CropHeightWide);
            else
                resizedBitmap = resizedBitmap.Crop(CropLeft, CropTop, CropWidth + CropLeft, CropTop + CropHeight);

            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync($"_cropTemp{(wide ? "Wide" : "")}.png", CreationCollisionOption.GenerateUniqueName);

            if (wide)
                _lastCroppedFileNameWide = file.Name;
            else
                _lastCroppedFileName = file.Name;

            await resizedBitmap.SaveAsync(file, BitmapEncoder.PngEncoderId);

            if (wide)
                PreviewImageWide = new BitmapImage(new Uri($"ms-appdata:///local/{file.Name}"));
            else
                PreviewImageNormal = new BitmapImage(new Uri($"ms-appdata:///local/{file.Name}"));
            UndoCropVisibility = Visibility.Visible;
        }

        private void ResetCrop(bool wide = false)
        {
            if (wide)
            {
                UndoWideCropVisibility = Visibility.Collapsed;
                PreviewImageWide = new BitmapImage(new Uri(EntryData.ImgUrl));
            }
            else
            {
                UndoCropVisibility = Visibility.Collapsed;
                PreviewImageNormal = new BitmapImage(new Uri(EntryData.ImgUrl));
            }          
        }

        private async void PinThing()
        {
            if (string.IsNullOrEmpty(_lastCroppedFileName))
            {
                var bmp = new WriteableBitmap(PreviewImageNormal.PixelWidth, PreviewImageNormal.PixelHeight);
                bmp = await bmp.LoadFromBitmapImageSourceAsync(PreviewImageNormal);
                var file = await ApplicationData.Current.LocalFolder.CreateFileAsync("_cropTemp.png", CreationCollisionOption.GenerateUniqueName);
                await bmp.SaveAsync(file, BitmapEncoder.PngEncoderId);
                _lastCroppedFileName = file.Name;
                if (string.IsNullOrEmpty(_lastCroppedFileNameWide))
                    _lastCroppedFileNameWide = file.Name;
            }
            if (string.IsNullOrEmpty(_lastCroppedFileNameWide))
            {
                var bmp = new WriteableBitmap(PreviewImageWide.PixelWidth, PreviewImageWide.PixelHeight);
                bmp = await bmp.LoadFromBitmapImageSourceAsync(PreviewImageWide);
                var file = await ApplicationData.Current.LocalFolder.CreateFileAsync("_cropTempWide.png", CreationCollisionOption.GenerateUniqueName);
                await bmp.SaveAsync(file, BitmapEncoder.PngEncoderId);
                _lastCroppedFileNameWide = file.Name;
                if (string.IsNullOrEmpty(_lastCroppedFileName))
                    _lastCroppedFileName = file.Name;
            }

            await LiveTilesManager.PinTile(TargetUrl ?? "", EntryData, new Uri($"ms-appdata:///local/{_lastCroppedFileName}"), new Uri($"ms-appdata:///local/{_lastCroppedFileNameWide}"));

            foreach (var file in await ApplicationData.Current.TemporaryFolder.GetFilesAsync(CommonFileQuery.DefaultQuery))
                if(file.Name.Contains("_cropTemp"))
                    await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
            GeneralVisibility = Visibility.Collapsed;
        }
    }
}
