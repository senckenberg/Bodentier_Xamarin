using KBS.App.TaxonFinder.Data;
using KBS.App.TaxonFinder.ViewModels;
using NavigationSam;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KBS.App.TaxonFinder.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FilterSelection : ContentPage, INavigationPopInterceptor
    {
        // Important service property
        public bool IsPopRequest { get; set; }

        public string FilterTagGroupName
        {
            set
            {
                ((FilterSelectionViewModel)BindingContext).FilterTagGroupName = value;
            }
        }

        public int FilterTag
        {
            set
            {
                ((FilterSelectionViewModel)BindingContext).FilterTag = value;
            }
        }

        public object SliderValInfo { get; private set; }
        public double SliderVal { get; private set; }

        public FilterSelection()
        {
            InitializeComponent();
        }

        public FilterSelection(int parentFilterId)
        {
            InitializeComponent();
            FilterTag = parentFilterId;
        }

        public FilterSelection(string filterGroupName)
        {
            InitializeComponent();
            FilterTagGroupName = filterGroupName;
        }


        private void CategoryList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = (FilterItem)e.Item;
            FilterTag = item.TagId;
        }

        private void SelectionList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = (FilterItem)e.Item;
            item.Selected = !item.Selected;
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var taxon = (Taxon)e.Item;
            if (taxon.hasDeterminationLink)
            {
                FilterSelection filterSelection = new FilterSelection();
                filterSelection.FilterTagGroupName = taxon.TaxonName;
                Navigation.PushAsync(filterSelection);
            }
            else if (taxon.TaxonomyStateName == "sp.")
            {
                Navigation.PushAsync(new TaxonInfo(taxon.TaxonId));
            }
        }

        private void NavigateToDetermination(object sender, EventArgs e)
        {
            var dbg1 = e;
            var dbg2 = sender;

            /*
            var taxon = (Taxon)e.Item;
            if (taxon.TaxonName!= null)
            {
                FilterSelection filterSelection = new FilterSelection();
                filterSelection.FilterTagGroupName = taxon.TaxonName;
                Navigation.PushAsync(filterSelection);
            }
            */
        }

        private void Help_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new HelpPage(this));
        }

        public async Task<bool> RequestPop()
        {
            var dontShow = Preferences.Get("dontShowBestimmungLeave", "false");

            if (dontShow != "true")
            {
                var result = await DisplayActionSheet("Wirklich verlassen?", "Abbrechen", null, "Ja", "Nein", "Nicht mehr anzeigen");

                if (result == "Ja")
                {
                    //Navigation.PushAsync(new OrderSelection());
                    Navigation.PopAsync();
                }
                else if (result == "Nicht mehr anzeigen")
                {
                    Preferences.Set("dontShowBestimmungLeave", "true");
                    //Navigation.PushAsync(new OrderSelection());
                    Navigation.PopAsync();
                }
                else
                {

                }
            }
            else
            {
                Navigation.PopAsync();// PushAsync(new OrderSelection());
            }

            return false;
        }
    }
}