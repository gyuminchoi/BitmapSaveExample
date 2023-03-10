using BitmapSaveExample.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BitmapSaveExample.Model
{
    public class DiskInfo : NotifyPropertyChanged
    {
        private string _modelName;
        private string _discription;
        private string _diskType;
        private string _rootDirectory;
        private string _diskName;
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
        public string DiskType
        {
            get => _diskType;
            set { _diskType = value; OnPropertyChanged(nameof(DiskType)); }
        }
        public string RootDirectory
        {
            get => _rootDirectory;
            set { _rootDirectory = value; OnPropertyChanged(nameof(RootDirectory)); }
        }
        public string DiskName
        {
            get => _diskName;
            set { _diskName = value; OnPropertyChanged(nameof(DiskName)); }
        }

        public DiskInfo()
        {
        }
    }
}
