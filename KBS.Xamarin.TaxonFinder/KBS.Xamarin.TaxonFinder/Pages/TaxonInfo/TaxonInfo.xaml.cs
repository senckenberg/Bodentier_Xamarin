using KBS.App.TaxonFinder.Data;
using KBS.App.TaxonFinder.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KBS.App.TaxonFinder.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaxonInfo : ContentPage
    {
        public TaxonInfo()
        {
            InitializeComponent();
        }

        public TaxonInfo(int taxonId)
        {
            InitializeComponent();
            ((TaxonInfoViewModel)BindingContext).SelectedTaxonId = taxonId;
            SetupCarousel();
        }

        private void SetupCarousel()
        {
            ObservableCollection<TaxonImage> CarouselImageCollection = new ObservableCollection<TaxonImage>();
            if (((TaxonInfoViewModel)BindingContext).SelectedTaxon != null)
            {
                if (((TaxonInfoViewModel)BindingContext).SelectedTaxon.Images != null && ((TaxonInfoViewModel)BindingContext).SelectedTaxon.HasImages)
                {
                    foreach (TaxonImage taxonImage in ((TaxonInfoViewModel)BindingContext).SelectedTaxon.Images)
                    {
                        if (taxonImage != null)
                        {
                            if (taxonImage.ImageId != null && taxonImage.LoRes != null)
                            {
                                CarouselImageCollection.Add(taxonImage);
                            }
                        }
                    }
                }
            }
            if (CarouselImageCollection.Count > 0)
            {
                ImageCarousel.ItemsSource = CarouselImageCollection;
            }
        }


        private void HtmlLabel_NavigateToTaxon(object sender, CustomRenderers.TaxonLinkClickedEventArgs e)
        {
            var rNavigation = App.Current.MainPage.Navigation;

            var page = rNavigation.NavigationStack.Last();
            rNavigation.PushAsync(new TaxonInfo(e.TaxonId));
            rNavigation.RemovePage(page);

        }

        private void Help_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new HelpPage(this));
        }
        protected override void OnDisappearing()
        {
            //((TaxonInfoViewModel)BindingContext).Player.Stop();
            //((TaxonInfoViewModel)BindingContext).Player.Dispose();
            base.OnDisappearing();
        }
    }
}