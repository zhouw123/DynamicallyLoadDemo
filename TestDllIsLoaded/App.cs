using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows;

namespace TestDllIsLoaded
{
    public class App : Application
    {

   
        public App()
        {
            var appType = typeof(Application);
            var field = appType.GetField("_resourceAssembly", BindingFlags.NonPublic | BindingFlags.Static);
            field.SetValue(null, Assembly.GetExecutingAssembly());

            InitializeComponent();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
        }

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);
        }
        private void InitializeComponent()
        {
            //设置主窗口启动项。
            this.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
        }
        //声明一个Window类型的集合,用于放置子窗口。
        private List<Window> listWindow = new List<Window>();

        public List<Window> ListWindow
        {
            get { return listWindow; }
            set { this.listWindow = value; }
        }
    }
}
