using KBS.App.TaxonFinder.ViewModels;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace KBS.App.TaxonFinder.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaxonMediaInfo : ContentPage
    {

        double currentScale = 1;
        double startScale = 1;
        double xOffset = 0;
        double yOffset = 0;
        double x, y;
        double deviceHeight, deviceWidth;

        private const double MIN_SCALE = 1;
        private const double MAX_SCALE = 2;
        private const double OVERSHOOT = 0.15;
        private double StartX, StartY;
        private double StartScale;


        private static Image TaxonImageShadow { get; set; }

        public TaxonMediaInfo()
        {
            InitializeComponent();
            TaxonImageShadow = TaxonImage;
            deviceHeight = DeviceDisplay.MainDisplayInfo.Height;// / DeviceDisplay.MainDisplayInfo.Density;
            deviceWidth = DeviceDisplay.MainDisplayInfo.Width; // DeviceDisplay.MainDisplayInfo.Density;
        }

        public TaxonMediaInfo(string mediaTitle)
        {
            InitializeComponent();
            if (mediaTitle != null)
            {
                if (Int32.TryParse(mediaTitle, out var mediaId))
                {
                    TaxonMediaInfoViewModel.SelectedMediaId = mediaId;
                }
                else if (Guid.TryParse(mediaTitle, out var mediaGuid))
                {
                    TaxonMediaInfoViewModel.SelectedMediaGuid = mediaGuid;
                }
                else
                {
                    //string title for slider images
                    TaxonMediaInfoViewModel.SelectedMediaTitle = mediaTitle;
                }
            }
            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            var screenWidth = mainDisplayInfo.Width / mainDisplayInfo.Density;
            TaxonImage.HeightRequest = screenWidth / 3.43;
            TaxonImageShadow = TaxonImage;
            deviceHeight = DeviceDisplay.MainDisplayInfo.Height;// / DeviceDisplay.MainDisplayInfo.Density;
            deviceWidth = DeviceDisplay.MainDisplayInfo.Width; // DeviceDisplay.MainDisplayInfo.Density;
        }

        public TaxonMediaInfo(int mediaId)
        {
            InitializeComponent();
            if (mediaId != 0)
            {
                TaxonMediaInfoViewModel.SelectedMediaId = mediaId;
            }
            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            var screenWidth = mainDisplayInfo.Width / mainDisplayInfo.Density;
            TaxonImage.HeightRequest = screenWidth / 3.43;
            TaxonImageShadow = TaxonImage;
            deviceHeight = DeviceDisplay.MainDisplayInfo.Height;// / DeviceDisplay.MainDisplayInfo.Density;
            deviceWidth = DeviceDisplay.MainDisplayInfo.Width; // DeviceDisplay.MainDisplayInfo.Density;
        }

        public TaxonMediaInfo(string imagePath, int taxonId)
        {
            InitializeComponent();
            String imgTitle = imagePath.Split('/').Last();
            Regex rgx = new Regex(@"^.+_2\d{7}_\d{6}\.[a-z]{3}$");
            TaxonImage.Source = imagePath;
            //CopyrightLabel.Text = imgTitle;
            var taxon = ((App)App.Current).Taxa.FirstOrDefault(i => i.TaxonId == (int)(taxonId));
            TitleLabel.Text = (taxon != null) ? taxon.LocalName : "Unbekannte Art";
            //SubTitleLabel.Text = (taxon != null) ? taxon.TaxonName : "Unbekannte Art";

            if (rgx.IsMatch(imgTitle))
            {
                DescriptionLabel.Text = "Aufgenommen am " + imgTitle.Split('_')[1].Substring(6, 2) + "." + imgTitle.Split('_')[1].Substring(4, 2) + "." + imgTitle.Split('_')[1].Substring(0, 4) + " um " + imgTitle.Split('_')[2].Substring(0, 2) + ":" + imgTitle.Split('_')[2].Substring(2, 2) + " Uhr.";
            }
            else
            {
                DescriptionLabel.Text = "";
            }
            TaxonImageShadow = TaxonImage;
            deviceHeight = DeviceDisplay.MainDisplayInfo.Height;// / DeviceDisplay.MainDisplayInfo.Density;
            deviceWidth = DeviceDisplay.MainDisplayInfo.Width; // DeviceDisplay.MainDisplayInfo.Density;
        }

        private TaxonMediaInfoViewModel TaxonMediaInfoViewModel
        {
            get
            {
                return (TaxonMediaInfoViewModel)BindingContext;
            }
        }


        void OnTapped(object sender, EventArgs e)
        {
            if (Scale > MIN_SCALE)
            {
                this.ScaleTo(MIN_SCALE, 250, Easing.CubicInOut);
                this.TranslateTo(0, 0, 250, Easing.CubicInOut);
            }
            else
            {
                AnchorX = AnchorY = 0.5;
                this.ScaleTo(MAX_SCALE, 250, Easing.CubicInOut);
            }
        }

        void OnPinchUpdated(object sender, EventArgs e)
        {
            PinchGestureUpdatedEventArgs ePinch = (PinchGestureUpdatedEventArgs)e;
            switch (ePinch.Status)
            {
                case GestureStatus.Started:
                    StartScale = (double)Scale;
                    AnchorX = ePinch.ScaleOrigin.X;
                    AnchorY = ePinch.ScaleOrigin.Y;
                    break;

                case GestureStatus.Running:
                    double current = Scale + (ePinch.Scale - 1) * StartScale;
                    Scale = HelperTools.Clamp(current, MIN_SCALE * (1 - OVERSHOOT), MAX_SCALE * (1 + OVERSHOOT));
                    break;

                case GestureStatus.Completed:
                    if (Scale > MAX_SCALE)
                        this.ScaleTo(MAX_SCALE, 250, Easing.SpringOut);
                    else if (Scale < MIN_SCALE)
                        this.ScaleTo(MIN_SCALE, 250, Easing.SpringOut);
                    break;
            }
        }

        void OnPanUpdated(object sender, EventArgs e)
        {
            PanUpdatedEventArgs ePan = (PanUpdatedEventArgs)e;
            switch (ePan.StatusType)
            {
                case GestureStatus.Started:
                    StartX = (1 - AnchorX) * Width;
                    StartY = (1 - AnchorY) * Height;
                    break;

                case GestureStatus.Running:
                    AnchorX = HelperTools.Clamp(1 - (StartX + ePan.TotalX) / Width, 0, 1);
                    AnchorY = HelperTools.Clamp(1 - (StartY + ePan.TotalY) / Height, 0, 1);
                    break;
            }

            /*
			if(TaxonImage.Scale > 1 && ((TaxonImage.Height * TaxonImage.Scale) > Content.Height))
            {
				var dbg1 = absLayout.Height;
				var dbg2 = absLayout.Width;
				var dbg3 = TaxonImage.Height * TaxonImage.Scale;
				var dbg4 = TaxonImage.Width * TaxonImage.Scale;

				PanUpdatedEventArgs ePan = (PanUpdatedEventArgs)e;
				switch (ePan.StatusType)
				{
					case GestureStatus.Running:
						// Translate and ensure we don't pan beyond the wrapped user interface element bounds.
						TaxonImage.TranslationX =
						  Math.Max(Math.Min(0, x + ePan.TotalX), -Math.Abs((TaxonImage.Width * TaxonImage.Scale) - Content.Width));
						TaxonImage.TranslationY =
						  Math.Max(Math.Min(0, y + ePan.TotalY), -Math.Abs((TaxonImage.Height * TaxonImage.Scale) - Content.Height));
						Trace.WriteLine("X: " + TaxonImage.X);
						Trace.WriteLine("Y: " + TaxonImage.Y);
						Trace.WriteLine("TX: " + TaxonImage.TranslationX);
						Trace.WriteLine("TY: " + TaxonImage.TranslationY);
						break;

					case GestureStatus.Completed:
						// Store the translation applied during the pan
						x = TaxonImage.TranslationX;
						y = TaxonImage.TranslationY;
						break;
				}
			}
			*/
        }
    }

    public static class HelperTools
    {
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }
    }

}