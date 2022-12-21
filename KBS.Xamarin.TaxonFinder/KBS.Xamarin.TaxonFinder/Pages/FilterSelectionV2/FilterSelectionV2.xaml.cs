using KBS.App.TaxonFinder.Data;
using KBS.App.TaxonFinder.ViewModels;
using NavigationSam;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KBS.App.TaxonFinder.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FilterSelectionV2 : ContentPage, INavigationPopInterceptor, INotifyPropertyChanged
    {
        // Important service property
        public bool IsPopRequest { get; set; }

        public FilterItem ScrollToItem { get; set; }

        public string FilterTagGroupName
        {
            set
            {
                ((FilterSelectionViewModelV2)BindingContext).FilterTagGroupName = value;
            }
        }

        public int FilterTag
        {
            set
            {
                ((FilterSelectionViewModelV2)BindingContext).FilterTag = value;
            }
        }

        public object SliderValInfo { get; private set; }
        public double SliderVal { get; private set; }

        public FilterSelectionV2()
        {
            InitializeComponent();
            GoToParentCommandButton.Clicked += GoToParentCommandButton_Clicked;
        }

        private void GoToParentCommandButton_Clicked(object sender, EventArgs e)
        {
            if (ScrollToItem != null)
            {
                TriggerScroll();
            }
        }

        public FilterSelectionV2(int parentFilterId)
        {
            InitializeComponent();
            FilterTag = parentFilterId;
        }

        public FilterSelectionV2(string filterGroupName)
        {
            InitializeComponent();
            FilterTagGroupName = filterGroupName;
        }


        private void CategoryList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = (FilterItem)e.Item;
            ScrollToItem = (FilterItem)e.Item;
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
                FilterSelectionV2 filterSelection = new FilterSelectionV2();
                filterSelection.FilterTagGroupName = taxon.TaxonName;
                Navigation.PushAsync(filterSelection);
            }
            else if (taxon.TaxonomyStateName == "sp.")
            {
                Navigation.PushAsync(new TaxonInfo(taxon.TaxonId));
            }
        }

        private async void OnImageTapped(object sender, EventArgs e) {
            try
            {
                TappedEventArgs eTap = (TappedEventArgs)e;
                int imageId = Int32.Parse(eTap.Parameter.ToString());
                if (imageId != null)
                {
                    await App.Current.MainPage.Navigation.PushAsync(new TaxonMediaInfo((int)imageId));
                }
            } catch(Exception ex)
            {
                Trace.WriteLine(ex);
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

        protected override bool OnBackButtonPressed()
        {
            var dontShow = Preferences.Get("dontShowBestimmungLeave", "false");

            if (dontShow != "true")
            {
                var result = DisplayActionSheet("Wirklich verlassen?", "Abbrechen", null, "Ja", "Nein", "Nicht mehr anzeigen").Result;

                if (result == "Ja")
                {
                    //Navigation.PushAsync(new OrderSelection());
                    Navigation.PopAsync();
                    return true;
                }
                else if (result == "Nicht mehr anzeigen")
                {
                    Preferences.Set("dontShowBestimmungLeave", "true");
                    //Navigation.PushAsync(new OrderSelection());
                    Navigation.PopAsync();
                    return true;
                }
                else
                {

                }
            }
            else
            {
                Navigation.PopAsync();// PushAsync(new OrderSelection());
                return true;
            }
            return false;
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
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;
        private void _currentItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

            if (e.PropertyName == "FilterTag")
            {
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        #endregion

        public void TriggerScroll()
        {
            CategoryList.ScrollTo(ScrollToItem, ScrollToPosition.Center, true);
        }
    }
}