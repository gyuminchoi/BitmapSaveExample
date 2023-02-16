using BitmapSaveExample.Model;
using BitmapSaveExample.Service;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Windows;
using System.Windows.Input;

namespace BitmapSaveExample.ViewModels
{
    public class MainViewModel : NotifyPropertyChanged
    {
        const string TestSavePath = "TestImagePath";

        private string _imagePath;
        private TimeSpan _savingTime;
        private string _diskType;
        private string _modelName;
        private string _discription;
        private int _saveCount;
        private bool _isSave;

        public string ImagePath
        {
            get => _imagePath;
            set { _imagePath = value; OnPropertyChanged(nameof(ImagePath)); }
        }

        public TimeSpan SavingTime
        {
            get => _savingTime;
            set { _savingTime = value; OnPropertyChanged(nameof(SavingTime)); }
        }

        public string DiskType
        {
            get => _diskType;
            set { _diskType = value; OnPropertyChanged(nameof(DiskType)); }
        }

        public string ModelName
        {
            get => _modelName;
            set { _modelName = value; OnPropertyChanged(nameof(ModelName)); }
        }

        public string Discription
        {
            get => _discription;
            set { _discription = value; OnPropertyChanged(nameof(Discription)); }
        }

        public bool IsSave
        {
            get => _isSave;
            set { _isSave = value; OnPropertyChanged(nameof(IsSave)); }
        }

        public List<DiskInfo> DiskList { get; set; }

        public ICommand BtnSetImagePathClick { get; set; }
        public ICommand BtnSaveBitmapClick { get; set; }
        public MainViewModel()
        {
            BtnSetImagePathClick = new Command(OnUploadImagePath, CanExecute_Func);
            BtnSaveBitmapClick = new Command(OnSaveBitmap, CanExecute_Func);

            DiskList = new List<DiskInfo>();
            _saveCount = 0;
            Initialize();
        }

        private void OnUploadImagePath(object obj)
        {
            string backup = ImagePath;
            ImagePath = GetFilePath(backup, "bmp", "jpg", "png");

            IsSave = FindImagePath();
            
            DiskInfo info = FindDiskInfo(ImagePath);
            if (info == null) return;

            DiskType = info.DiskType;
            ModelName = info.ModelName;
            Discription = info.Discription;

            
            CreateSaveImagePath(info.RootDirectory);
        }

        private void OnSaveBitmap(object obj)
        {
            IsSave = FindImagePath();
            if (!IsSave) return;

            DiskInfo info =  FindDiskInfo(ImagePath);
            if(info == null) return;
            
            string extention = Path.GetExtension(ImagePath);
            string testPath = SetPath(info.RootDirectory);
            string savePath = Path.Combine(testPath, _saveCount.ToString());

            Stopwatch stopwatch = Stopwatch.StartNew();
            using (Bitmap bmp = new Bitmap(ImagePath))
            {
                bmp.Save(savePath + extention);
                stopwatch.Stop();
                SavingTime = stopwatch.Elapsed;
            }
            _saveCount++;
        }

        public string GetFilePath(string backup = "", string filter1 = "*", string filter2 = "*", string fiter3 = "*")
        {
            var fileDialog = new OpenFileDialog()
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Filter = $"확장자 (*.{filter1})|*.{filter1}|확장자 (*.{filter2})|*.{filter2}|확장자 (*.{fiter3})|*.{fiter3}"
            };

            if (fileDialog.ShowDialog() == true) return fileDialog.FileName;
            else return backup;
        }

        private string SetPath(string localPath)
        {
            string path = Path.Combine(localPath, TestSavePath);
            return path;
        }

        private DiskInfo FindDiskInfo(string path)
        {
            try
            {
                char rootName = path.FirstOrDefault();
                var info = DiskList.Single(item => item.RootDirectory.First() == rootName);

                return info;
            }
            catch (ArgumentNullException) { return null; }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
                return null;
            }
            
        }

        private bool FindImagePath()
        {
            if (File.Exists(ImagePath)) return true;
            else return false;
        }

        private void Initialize()
        {
            try
            {
                DriveInfo[] driveInfo = DriveInfo.GetDrives();
                int length = driveInfo.Length;
                for (int i = 0; i < length; i++)
                {
                    var diskInfo = new DiskInfo() { RootDirectory = driveInfo[i].RootDirectory.ToString() };

                    DiskList.Add(diskInfo);
                }

                ManagementScope scope = new ManagementScope(@"\\.\root\microsoft\windows\storage");
                scope.Connect();
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from MSFT_PhysicalDisk"))
                {
                    searcher.Scope = scope;
                    int index = length;
                    foreach (ManagementObject item in searcher.Get())
                    {
                        string diskType = "";
                        index--;

                        switch (Convert.ToInt16(item["MediaType"]))
                        {
                            case 1:
                                diskType = "Unspecified";
                                break;

                            case 3:
                                diskType = "HDD";
                                break;

                            case 4:
                                diskType = "SSD";
                                break;

                            case 5:
                                diskType = "SCM";
                                break;

                            default:
                                diskType = "Unspecified";
                                break;
                        }
                        DiskList[index].DiskType = diskType;
                    }
                }

                WqlObjectQuery q = new WqlObjectQuery("SELECT * FROM Win32_DiskDrive");
                using (ManagementObjectSearcher res = new ManagementObjectSearcher(q))
                {
                    int index = 0;
                    foreach (ManagementObject o in res.Get())
                    {
                        DiskList[index].ModelName = (string)o["Model"];
                        DiskList[index].Discription = (string)o["Description"];
                        index++;
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());  
            }
            
        }

        private void CreateSaveImagePath(string localPath)
        {
            string testPath = SetPath(localPath);

            if (!Directory.Exists(testPath)) Directory.CreateDirectory(testPath);
        }

        private bool CanExecute_Func(object arg) => true;
    }
}
