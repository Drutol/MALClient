using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MalClient.Shared.Comm;
using MalClient.Shared.Utils;
using MalClient.Shared.Utils.Managers;
using WinRTXamlToolkit.Imaging;

namespace MalClient.Shared.ViewModels
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
                if (value == Visibility.Visible)
                {
                    ViewModelLocator.NavMgr.RegisterOneTimeOverride(new RelayCommand(() => GeneralVisibility = Visibility.Collapsed));
                    ViewModelLocator.GeneralMain.View.PinDialogStoryboard.Begin();
                    RaisePropertyChanged(() => GeneralVisibility);
                }
                else
                {
                    HidePinDialog();
                }             
            }
        }

        public async void HidePinDialog()
        {
            var sb = ViewModelLocator.GeneralMain.View.HidePinDialogStoryboard;
            sb.Completed += SbOnCompleted;
            sb.Begin();
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
                PinSettings.AddImage = value != Visibility.Collapsed;
                RaisePropertyChanged(() => PinSettings);
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

        public string OpenWebsiteText => Settings.SelectedApiType == ApiType.Mal ? "Open in Mal" : "Open in hummingbird";
            

        private int _selectedImageOptionIndex = 0;

        public int SelectedImageOptionIndex
        {
            get { return _selectedImageOptionIndex; }
            set
            {    
                _selectedImageOptionIndex = value;
                ImagePreviewVisibility = value == 0 ? Visibility.Visible : Visibility.Collapsed;
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

        public PinTileSettings PinSettings { get; set; } = new PinTileSettings();

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

        public bool _isCropEnabled = true;
        public bool IsCropEnabled
        {
            get { return _isCropEnabled; }
            set
            {
                _isCropEnabled = value;
                RaisePropertyChanged(() => IsCropEnabled);
            }
        }

        public bool _isPinEnabled = true;
        public bool IsPinEnabled
        {
            get { return _isPinEnabled; }
            set
            {
                _isPinEnabled = value;
                RaisePropertyChanged(() => IsPinEnabled);
            }
        }

        //private StorageFile _originaPickedStorageFile;
        //private async void LoadSelectedImage()
        //{
        //    var fp = new FileOpenPicker();
        //    fp.ViewMode = PickerViewMode.Thumbnail;
        //    fp.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
        //    fp.FileTypeFilter.Add(".png");
        //    fp.FileTypeFilter.Add(".jpg");
        //    var file = await fp.PickSingleFileAsync();
        //    if (file != null)
        //    {
        //        _selectedImageOptionIndex = 1;
        //        ImagePreviewVisibility = Visibility.Visible;
        //        RaisePropertyChanged(() => SelectedImageOptionIndex);
        //    }
        //    _originaPickedStorageFile = file;
        //    var bmp = new BitmapImage();
        //    var bmp1 = new BitmapImage();
        //    using (var fs = (await file.OpenStreamForReadAsync()).AsRandomAccessStream())
        //    {
        //        bmp.SetSource(fs);
        //        fs.Seek(0);
        //        bmp1.SetSource(fs);
        //    }
        //    PreviewImageWide = bmp1;
        //    PreviewImageNormal = bmp;
        //}

        //public async void HidePinDialog()
        //{
        //    var sb = ViewModelLocator.Main.View.HidePinDialogStoryboard;
        //    sb.Completed += SbOnCompleted;
        //    sb.Begin();
        //}

        private void SbOnCompleted(object sender, object o)
        {
            (sender as Storyboard).Completed -= SbOnCompleted;
            RaisePropertyChanged(() => GeneralVisibility);
        }

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
            IsCropEnabled = false;
            try
            { 
                var img = wide ? _previewImageWide : _previewImageNormal;
                WriteableBitmap resizedBitmap = new WriteableBitmap(CropWidth, CropHeight);
                //if (img.UriSource == null)
                //await resizedBitmap.LoadAsync(_originaPickedStorageFile);
                /*else*/
                if (!img.UriSource.ToString().Contains("ms-appdata"))
                {
                    var imgFile = await SaveImage(img, wide);
                    await resizedBitmap.LoadAsync(imgFile);
                }
                else
                    await resizedBitmap.LoadAsync(await StorageFile.GetFileFromApplicationUriAsync(img.UriSource));

                if (wide)
                    resizedBitmap = resizedBitmap.Crop(CropLeftWide, CropTopWide, CropWidthWide + CropLeftWide, CropTopWide + CropHeightWide);
                else
                    resizedBitmap = resizedBitmap.Crop(CropLeft, CropTop, CropWidth + CropLeft, CropTop + CropHeight);

                var file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync($"_cropTemp{(wide ? "Wide" : "")}.png", CreationCollisionOption.GenerateUniqueName);

                if (wide)
                    _lastCroppedFileNameWide = file.Name;
                else
                    _lastCroppedFileName = file.Name;

                await resizedBitmap.SaveAsync(file, BitmapEncoder.PngEncoderId);

                if (wide)
                {
                    PreviewImageWide = new BitmapImage(new Uri($"ms-appdata:///temp/{file.Name}"));
                }
                else
                {
                    PreviewImageNormal = new BitmapImage(new Uri($"ms-appdata:///temp/{file.Name}"));
                    UndoCropVisibility = Visibility.Visible;
                }
                
            }
            catch (Exception)
            {
                Utilities.GiveStatusBarFeedback("An error occured...");
            }
            IsCropEnabled = true;
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

        private async Task<StorageFile> SaveImage(BitmapImage img,bool wide)
        {
            try
            {
                var uri = img.UriSource;

                var http = new HttpClient();
                byte[] response = { };
                var file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync($"_cropTemp{(wide ? "Wide" : "")}.png", CreationCollisionOption.GenerateUniqueName);
                //get bytes
                await Task.Run(async () => response = await http.GetByteArrayAsync(uri));


                var fs = await file.OpenStreamForWriteAsync(); //get stream
                var writer = new DataWriter(fs.AsOutputStream());

                writer.WriteBytes(response); //write
                await writer.StoreAsync();
                await writer.FlushAsync();

                writer.Dispose();
                return file;
            }
            catch (Exception)
            {
                Utilities.GiveStatusBarFeedback("An error occured...");
                return null;
            }


            //var bmp = new WriteableBitmap(img.PixelWidth, img.PixelHeight);
            //bmp = await bmp.LoadFromBitmapImageSourceAsync(img);

            //await bmp.SaveAsync(file, BitmapEncoder.PngEncoderId);
            //return file;
        }

        private async void PinThing()
        {
            IsPinEnabled = false;
            try
            {
                if (SelectedImageOptionIndex == 0)
                {
                    //if we didn't crop
                    if (string.IsNullOrEmpty(_lastCroppedFileName))
                    {
                        var file = await SaveImage(PreviewImageNormal, false);
                        _lastCroppedFileName = file.Name;
                        //if we din't crop wide either
                        if (string.IsNullOrEmpty(_lastCroppedFileNameWide))
                            _lastCroppedFileNameWide = file.Name; //set source to this
                    }
                    //if we didn't crop wide... you get the idea
                    if (string.IsNullOrEmpty(_lastCroppedFileNameWide))
                    {
                        //we may have not even opened wide pivot image -> no img loaded -> no width -> assume normal picture
                        if (PreviewImageWide.PixelWidth == 0)
                            _lastCroppedFileNameWide = _lastCroppedFileName;
                        else
                        {
                            var file = await SaveImage(PreviewImageWide, true);
                            _lastCroppedFileNameWide = file.Name;
                            if (string.IsNullOrEmpty(_lastCroppedFileName))
                                _lastCroppedFileName = file.Name;
                        }
                    }
                }
                var action = new PinTileActionSetting();
                switch (SelectedActionIndex)
                {
                    case 0:
                        action.Action = TileActions.OpenUrl;
                        action.Param = TargetUrl ?? "";
                        break;
                    case 1:
                        action.Action = TileActions.OpenUrl;
                        action.Param = Settings.SelectedApiType == ApiType.Mal
                            ? $"https://myanimelist.net/{(EntryData.ParentAbstraction.RepresentsAnime ? "anime" : "manga")}/{EntryData.Id}"
                            : $"https://hummingbird.me/{(EntryData.ParentAbstraction.RepresentsAnime ? "anime" : "manga")}/{EntryData.Id}";
                        break;
                    default:
                        action.Action = TileActions.OpenDetails;
                        action.Param = EntryData.Id + "|" + EntryData.Title;
                        break;
                }
                await LiveTilesManager.PinTile(EntryData, (SelectedImageOptionIndex == 0 ? new Uri($"ms-appdata:///temp/{_lastCroppedFileName}") : null), (SelectedImageOptionIndex == 0 ? new Uri($"ms-appdata:///temp/{_lastCroppedFileNameWide}") : null), PinSettings, action);
                GeneralVisibility = Visibility.Collapsed;
            }
            catch (Exception)
            {
                Utilities.GiveStatusBarFeedback("An error occured...");
            }           
            IsPinEnabled = true;
        }
    }
}
