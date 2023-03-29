using System;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KBS.App.TaxonFinder.Converters
{
    public class TaxonGuidToTaxonNameConverter : IMarkupExtension, IValueConverter
    {
        public object Convert(object value,
                                Type targetType,
                                object parameter,
                                System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value != null)
                {
                    if (Guid.TryParse(value.ToString(), out var taxonGuid))
                    {
                        var taxon = ((App)App.Current).Taxa.FirstOrDefault(i => i.Identifier == taxonGuid);
                        if (taxon != null)
                        {
                            return taxon.LocalName;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
            return "Unbekannte Art";
        }

        public object ConvertBack(object value,
                                    Type targetTypes,
                                    object parameter,
                                    System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}