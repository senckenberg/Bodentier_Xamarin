using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace KBS.App.TaxonFinder
{
    public partial class LargeDevicesStyle : ResourceDictionary
    {
        public static LargeDevicesStyle SharedInstance { get; } = new LargeDevicesStyle();
        public LargeDevicesStyle()
        {
            InitializeComponent();
        }
    }
}