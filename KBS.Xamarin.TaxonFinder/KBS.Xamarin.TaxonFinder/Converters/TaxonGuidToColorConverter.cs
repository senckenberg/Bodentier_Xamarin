using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KBS.App.TaxonFinder.Converters
{
    public class TaxonGuidToColorConverter : IMarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                if (!Guid.TryParse((string)value, out var taxonGuid) && Application.Current.Resources.TryGetValue("danger", out var colourDanger))
                {
                    return (Color)colourDanger;
                }
            }
            
            if (Application.Current.Resources.TryGetValue("font_dark", out var colourFontDark))
            {
                return (Color)colourFontDark;
            }
            return Color.Gray;
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