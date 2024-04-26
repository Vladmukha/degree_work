using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestAndroidClear.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UserPage : ContentPage
	{
		public UserPage ()
		{
			InitializeComponent ();
		}

        private async void About_Clicked(object sender, EventArgs e)
        {
            if (InfoCard.IsVisible == false)
            {
                InfoCard.Opacity = 0;
                InfoCard.IsVisible = true;
                InfoCard.FadeTo(1, 200);
                await InfoCard.TranslateTo(0, 10, 200);
            }
            else
            {
                InfoCard.Opacity = 1;
                InfoCard.TranslateTo(0, -10, 200);
                await InfoCard.FadeTo(0, 100);
                InfoCard.IsVisible = false;
            }
        }

        private void ChangeLogin_Clicked(object sender, EventArgs e)
        {
            string newLogin = "";
            login.Text = newLogin;
        }

        private void ChangePass_Clicked(object sender, EventArgs e)
        {

        }

        private void ChangeMail_Clicked(object sender, EventArgs e)
        {

        }

        private void Exit_Clicked(object sender, EventArgs e)
        {

        }
    }
}