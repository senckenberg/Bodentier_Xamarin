using KBS.App.TaxonFinder.Data;
using KBS.App.TaxonFinder.Services;
using KBS.App.TaxonFinder.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace KBS.App.TaxonFinder.ViewModels
{
    public class TutorialViewModel : INotifyPropertyChanged
    {
        #region Fields
        #endregion
        #region Properties
        public ObservableCollection<TutorialItem> Monkeys { get; set; }

        #endregion

        #region Constructor

        public TutorialViewModel()
        {
            try
            {
                Monkeys = new ObservableCollection<TutorialItem>(Load.FromFile<TutorialItem>("TutorialItems.json").OrderBy(i => i.Index).ToList());
                OnPropertyChanged(nameof(Monkeys));

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

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
    }
}
