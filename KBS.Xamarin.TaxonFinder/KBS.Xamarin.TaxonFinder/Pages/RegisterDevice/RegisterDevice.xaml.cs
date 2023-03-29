using KBS.App.TaxonFinder.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KBS.App.TaxonFinder.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RegisterDevice : ContentPage
	{
		public bool TryingToRegister
		{
			set
			{
				((RegisterDeviceViewModel)BindingContext).TryingToRegister = value;
			}
		}
		public RegisterDevice()
		{
			InitializeComponent();

			TryingToRegister = false;
		}

		void AddUserButton_Clicked(object sender, EventArgs e)
		{
			TryingToRegister = true;
			//LogoutButton.IsVisible = false;
			RegisterStack.IsVisible = true;
		}

		public async void DeleteUserButton_Clicked (object sender, EventArgs e)
		{
            var result = await DisplayActionSheet("Wirklich löschen?", "Abbrechen", null, "Ja", "Nein");

            if (result == "Ja")
            {
				//Navigation.PushAsync(new OrderSelection());
				IsBusy = true;
                await ((RegisterDeviceViewModel)BindingContext).DeleteUser();
            }
		}

		private void Help_Clicked(object sender, EventArgs e)
		{
			Navigation.PushAsync(new HelpPage(this));
		}

	}

}