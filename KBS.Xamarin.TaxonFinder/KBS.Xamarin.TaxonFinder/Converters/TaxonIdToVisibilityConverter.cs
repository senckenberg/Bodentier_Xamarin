using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KBS.App.TaxonFinder.Converters
{
    public class TaxonIdToVisibilityConverter : IMarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                if ((int)value > 0)
                {
                    var taxon = ((App)App.Current).Taxa.FirstOrDefault(i => i.TaxonId == (int)value);
                    return taxon != null ? true : false;
                }
            }
            return false;
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}