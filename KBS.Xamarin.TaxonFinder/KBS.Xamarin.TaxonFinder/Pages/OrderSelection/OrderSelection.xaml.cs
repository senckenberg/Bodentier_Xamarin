using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace KBS.App.TaxonFinder.Views
{
    public partial class OrderSelection : ContentPage
    {

        public List<string> groupList = new List<string> { "Bodentiere", "Doppelfüßer (Diplopoda)", "Samenfüßer (Chordeumatida)", "Bandfüßer (Polydesmida)", "Schnurfüßer (Julida)", "Saftkugler (Glomerida)", "Pinselfüßer (Polyxenida)", "Bohrfüßer (Polyzoniida)", "Hundertfüßer (Chilopoda)", "Steinläufer (Lithobiomorpha)", "Skolopender(Scolopendromorpha)", "Erdkriecher (Geophilomorpha)", "Spinnenläufer (Scutigeromorpha)", "Asseln (Isopoda)" };
        private FilterSelectionV2 filterSelection;

        public OrderSelection()
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

        private void DetailedSelectionButton_Clicked(object sender, EventArgs e)
        {
            //filterSelection.FilterTagGroupName = "Bodentiere";
            Navigation.PushAsync(new OrderSelectionDetail());
        }

        private void Help_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new HelpPage(this));
        }

    }
}
