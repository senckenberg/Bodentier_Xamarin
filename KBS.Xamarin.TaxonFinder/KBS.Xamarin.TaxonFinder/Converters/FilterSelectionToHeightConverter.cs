using System;
using System.Globalization;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KBS.App.TaxonFinder.Converters
{
    public class FilterSelectionToHeightConverter : IMarkupExtension, IValueConverter
    {
        public object Convert(object value,
                                Type targetType,
                                object parameter,
                                CultureInfo culture)
        {
            DisplayInfo di = DeviceDisplay.MainDisplayInfo;
            if (value != null)
            {
               
                if ((bool)value == false)
                {
                    return (double)0.9 * (di.Height/di.Density);
                }

            }
            return (double)0.35 * (di.Height / di.Density);
        }

        public object ConvertBack(object value,
                                    Type targetType,
                                    object parameter,
                                    CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}