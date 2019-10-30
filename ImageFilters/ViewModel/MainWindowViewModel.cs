using ImageFilters.Enums;
using ImageFilters.Filters;
using ImageFilters.Helpers;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using UserApplication.ViewModels;

namespace ImageFilters.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        private FilterTypeEnum filterType;
        private AdaptiveTreshHoldeParamViewModel adaptiveTreshHoldeParamViewModel;
        private GaussianBlurParamViewModel gaussianBlurParamViewModel;
        private BitmapSource originalBitmapSource;
        private BitmapSource filteredBitmapSource;
        private int paramHeight;

        public ICommand OpenCommand { get { return new DelegateCommand(OnOpenCommand, CanExecuteOpenCommand); } }
        public ICommand ProcessCommand { get { return new DelegateCommand(OnProcessCommand, CanExecuteProcessCommand); } }
        public ICommand SaveCommand { get { return new DelegateCommand(OnSaveCommand, CanExecuteSaveCommand); } }


        public FilterTypeEnum FilterType
        {
            get => filterType;
            set
            {
                filterType = value;
                FilteredBitmapSource = null;
                _ = (FilterTypeEnum.AdaptiveTreshHold == filterType) ? ParamHeight = 100 : ParamHeight = 330;
                RaisePropertyChanged(() => FilterType);

            }
        }
        public int ParamHeight
        {
            get => paramHeight;
            set
            {
                paramHeight = value;
                RaisePropertyChanged(() => ParamHeight);
            }
        }

        public AdaptiveTreshHoldeParamViewModel AdaptiveTreshHoldParam
        {
            get => adaptiveTreshHoldeParamViewModel;
            set
            {
                adaptiveTreshHoldeParamViewModel = value;
                RaisePropertyChanged(() => AdaptiveTreshHoldParam);
            }
        }
        public GaussianBlurParamViewModel GaussianBlurParam
        {
            get => gaussianBlurParamViewModel;
            set
            {
                gaussianBlurParamViewModel = value;
                RaisePropertyChanged(() => GaussianBlurParam);
            }
        }


        public BitmapSource OriginalBitmapSource
        {
            get => originalBitmapSource;
            set
            {
                originalBitmapSource = value;
                RaisePropertyChanged(() => OriginalBitmapSource);
            }
        }
        public BitmapSource FilteredBitmapSource
        {
            get => filteredBitmapSource;
            set
            {
                filteredBitmapSource = value;
                RaisePropertyChanged(() => FilteredBitmapSource);
            }
        }

        public MainWindowViewModel()
        {
            FilterType = FilterTypeEnum.AdaptiveTreshHold;
            AdaptiveTreshHoldParam = new AdaptiveTreshHoldeParamViewModel();
            GaussianBlurParam = new GaussianBlurParamViewModel();
        }


        private void OnOpenCommand()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image files (*.jpg *.bmp *.png)|*.jpg;*.bmp;*.png|All Files (*.*)|*.*";

            if ((bool)dlg.ShowDialog())
            {
                string selectedFileName = dlg.FileName;

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(selectedFileName);
                bitmap.EndInit();
                OriginalBitmapSource = bitmap;
            }
        }
        private bool CanExecuteOpenCommand()
        {
            return true;
        }

        private void OnProcessCommand()
        {
            if (FilterTypeEnum.GaussianBlur == FilterType)
            {
                var gaussianBlurFilter = new GaussianBlurFilter(GaussianBlurParam.Kernel);
                FilteredBitmapSource = ToBitmapSource(gaussianBlurFilter.Applay(BitmapFromSource(OriginalBitmapSource)));
            }
            if (FilterTypeEnum.AdaptiveTreshHold == FilterType)
            {
                var adaptiveTresHoldFilter = new AdaptiveTreshHoldFilter(int.Parse(AdaptiveTreshHoldParam.ParamS), float.Parse(AdaptiveTreshHoldParam.ParamT));
                FilteredBitmapSource = ToBitmapSource(adaptiveTresHoldFilter.Applay(BitmapFromSource(OriginalBitmapSource)));
            }


        }
        private bool CanExecuteProcessCommand()
        {
            return OriginalBitmapSource != null;
        }

        private void OnSaveCommand()
        {
            SaveFileDialog dlg = new SaveFileDialog
            {
                Filter = "Image files(*.jpg *.bmp *.png) | *.jpg; *.bmp; *.png | All Files(*.*) | *.* ",
                FileName = "filtered image"
            };
            if ((bool)dlg.ShowDialog())
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(FilteredBitmapSource));
                using (FileStream stream = new FileStream(dlg.FileName, FileMode.Create))
                {
                    encoder.Save(stream);
                }

            }
        }
        private bool CanExecuteSaveCommand()
        {
            return FilteredBitmapSource != null;
        }


        private BitmapSource ToBitmapSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage as BitmapSource;
            }

        }
        private Bitmap BitmapFromSource(BitmapSource bitmapsource)
        {
            Bitmap bitmap;
            var outStream = new MemoryStream();
            BitmapEncoder enc = new BmpBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(bitmapsource));
            enc.Save(outStream);
            bitmap = new Bitmap(outStream);
            return bitmap;
        }

    }
}
