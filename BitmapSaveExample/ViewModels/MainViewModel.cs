using BitmapSaveExample.Service;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Windows.Input;

namespace BitmapSaveExample.ViewModels
{
    public class MainViewModel : NotifyPropertyChanged
    {
        private string _imagePath;
        private string _saveTime;
        private string _diskType;

        public string ImagePath
        {
            get => _imagePath;
            set { _imagePath = value; OnPropertyChanged(nameof(ImagePath)); }
        }

        public string SaveTime
        {
            get => _saveTime;
            set { _saveTime = value; OnPropertyChanged(nameof(SaveTime)); }
        }

        public string DiskType
        {
            get => _diskType;
            set { _diskType = value; OnPropertyChanged(nameof(DiskType)); }
        }

        public List<string> DiskList { get; set; }

        public ICommand BtnSetImagePathClick { get; set; }

        public MainViewModel()
        {
            BtnSetImagePathClick = new Command(OnUploadImagePath, CanExecute_Func);
            DiskList = new List<string>();
            Init();
        }

        private void OnUploadImagePath(object obj)
        {
            ImagePath = GetFilePath("bmp");
            if (ImagePath.Contains("C:")) DiskType = DiskList[1];
            if (ImagePath.Contains("D:")) DiskType = DiskList[0];

        }

        public string GetFilePath(string filter)
        {
            var fileDialog = new OpenFileDialog()
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Filter = $"파일 확장자 (*.{filter})|*.{filter}"
            };

            if (fileDialog.ShowDialog() == true) return fileDialog.FileName;
            else return string.Empty;
        }

        private bool CanExecute_Func(object arg) => true;
        private void Init()
        {
            ManagementScope scope = new ManagementScope(@"\\.\root\microsoft\windows\storage");
            scope.Connect();
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from MSFT_PhysicalDisk");
            searcher.Scope = scope;
            foreach (ManagementObject item in searcher.Get())
            {
                switch (Convert.ToInt16(item["MediaType"]))
                {
                    case 1:
                        DiskList.Add("Unspecified");
                        break;

                    case 3:
                        DiskList.Add("HDD");
                        break;

                    case 4:
                        DiskList.Add("SSD");
                        break;

                    case 5:
                        DiskList.Add("SCM");
                        break;

                    default:
                        DiskList.Add("Unspecified");
                        break;
                }
            }
            searcher.Dispose();
        }
    }
}
