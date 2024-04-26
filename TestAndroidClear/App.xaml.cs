using System;
using System.Text;
using TestAndroidClear.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestAndroidClear
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
