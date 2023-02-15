using BitmapSaveExample.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BitmapSaveExample
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MessageBox.Show("이미지 저장시 업로드한 경로의 드라이브에서 TestImagePath폴더에 저장됩니다.", "확인하기", MessageBoxButton.OK);
            var window = new MainWindow();
            this.MainWindow = window;
            window.Show();
        }
    }
}
