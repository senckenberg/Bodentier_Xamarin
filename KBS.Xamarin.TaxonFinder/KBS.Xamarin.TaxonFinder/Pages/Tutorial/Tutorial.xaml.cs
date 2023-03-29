using KBS.App.TaxonFinder.Data;
using KBS.App.TaxonFinder.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KBS.App.TaxonFinder.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Tutorial : ContentPage
    {

        public ObservableCollection<TutorialItem> Monkeys { get; set; }
        public Tutorial()
        {
            InitializeComponent();
        }

        private void SkipButton_Clicked(object sender, EventArgs e)
        {
            Preferences.Set("dontShowTutorialOnStartup", "true");
            Navigation.PushAsync(new MainPage());
        }

        private void Help_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new HelpPage(this));
        }
        protected override void OnDisappearing()
        {
            //var dontShowTutorial = Preferences.Get("dontShowTutorialOnStartup", "false");
            Preferences.Set("dontShowTutorialOnStartup", "true");
            base.OnDisappearing();
        }
    }
}