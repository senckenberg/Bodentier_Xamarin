using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace KBS.App.TaxonFinder
{
    public partial class SmallDevicesStyle : ResourceDictionary
    {
        public static SmallDevicesStyle SharedInstance { get; } = new SmallDevicesStyle();
        public SmallDevicesStyle()
        {
            InitializeComponent();
        }
    }
}