using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace KBS.App.TaxonFinder.Views
{
    public partial class OrderSelectionDetail : ContentPage
    {

        public List<string> groupList = new List<string> { "Bodentiere", "Doppelfüßer (Diplopoda)", "Samenfüßer (Chordeumatida)", "Bandfüßer (Polydesmida)", "Schnurfüßer (Julida)", "Saftkugler (Glomerida)", "Pinselfüßer (Polyxenida)", "Bohrfüßer (Polyzoniida)", "Hundertfüßer (Chilopoda)", "Steinläufer (Lithobiomorpha)", "Skolopender(Scolopendromorpha)", "Erdkriecher (Geophilomorpha)", "Spinnenläufer (Scutigeromorpha)", "Asseln (Isopoda)" };
        private FilterSelectionV2 filterSelection;

        public OrderSelectionDetail()
        {
            InitializeComponent();
            filterSelection = new FilterSelectionV2();
        }

        private void BodentiereSelectionButton_Clicked(object sender, EventArgs e)
        {
            filterSelection = new FilterSelectionV2();
            filterSelection.FilterTagGroupName = "Bodentiere";
            Navigation.PushAsync(filterSelection);
        }

        private void DoppelfuesserSelectionButton_Clicked(object sender, EventArgs e)
        {
            filterSelection = new FilterSelectionV2();
            filterSelection.FilterTagGroupName = "Doppelfüßer (Diplopoda)";
            Navigation.PushAsync(filterSelection);
        }
        private void SamenfuesserSelectionButton_Clicked(object sender, EventArgs e)
        {
            filterSelection = new FilterSelectionV2();
            filterSelection.FilterTagGroupName = "Samenfüßer (Chordeumatida)";
            Navigation.PushAsync(filterSelection);
        }

        private void BandfuesserSelectionButton_Clicked(object sender, EventArgs e)
        {
            filterSelection = new FilterSelectionV2();
            filterSelection.FilterTagGroupName = "Bandfüßer (Polydesmida)";
            Navigation.PushAsync(filterSelection);
        }

        private void SchnurfuesserSelectionButton_Clicked(object sender, EventArgs e)
        {
            filterSelection = new FilterSelectionV2();
            filterSelection.FilterTagGroupName = "Schnurfüßer (Julida)";
            Navigation.PushAsync(filterSelection);
        }

        private void SaftkuglerSelectionButton_Clicked(object sender, EventArgs e)
        {
            filterSelection = new FilterSelectionV2();
            filterSelection.FilterTagGroupName = "Saftkugler (Glomerida)";
            Navigation.PushAsync(filterSelection);
        }

        private void PinselfuesserSelectionButton_Clicked(object sender, EventArgs e)
        {
            filterSelection = new FilterSelectionV2();
            filterSelection.FilterTagGroupName = "Pinselfüßer (Polyxenida)";
            Navigation.PushAsync(filterSelection);
        }

        private void BohrfuesserSelectionButton_Clicked(object sender, EventArgs e)
        {
            filterSelection = new FilterSelectionV2();
            filterSelection.FilterTagGroupName = "Bohrfüßer (Polyzoniida)";
            Navigation.PushAsync(filterSelection);
        }

        private void HundertfuesserSelectionButton_Clicked(object sender, EventArgs e)
        {
            filterSelection = new FilterSelectionV2();
            filterSelection.FilterTagGroupName = "Hundertfüßer (Chilopoda)";
            Navigation.PushAsync(filterSelection);
        }

        private void SteinlaeuferSelectionButton_Clicked(object sender, EventArgs e)
        {
            filterSelection = new FilterSelectionV2();
            filterSelection.FilterTagGroupName = "Steinläufer (Lithobiomorpha)";
            Navigation.PushAsync(filterSelection);
        }

        private void SkolopenderSelectionButton_Clicked(object sender, EventArgs e)
        {
            filterSelection = new FilterSelectionV2();
            filterSelection.FilterTagGroupName = "Skolopender (Scolopendromorpha)";
            Navigation.PushAsync(filterSelection);
        }

        private void ErdkriecherSelectionButton_Clicked(object sender, EventArgs e)
        {
            filterSelection = new FilterSelectionV2();
            filterSelection.FilterTagGroupName = "Erdkriecher (Geophilomorpha)";
            Navigation.PushAsync(filterSelection);
        }

        private void SpinnenlaeuferSelectionButton_Clicked(object sender, EventArgs e)
        {
            filterSelection = new FilterSelectionV2();
            filterSelection.FilterTagGroupName = "Spinnenläufer (Scutigeromorpha)";
            Navigation.PushAsync(filterSelection);
        }

        private void LandasselnSelectionButton_Clicked(object sender, EventArgs e)
        {
            filterSelection = new FilterSelectionV2();
            filterSelection.FilterTagGroupName = "Asseln (Isopoda)";
            Navigation.PushAsync(filterSelection);
        }

        private void Help_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new HelpPage(this));
        }

    }
}
