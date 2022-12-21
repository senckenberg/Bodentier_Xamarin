using KBS.App.TaxonFinder.Data;
using KBS.App.TaxonFinder.Services;
using Plugin.SimpleAudioPlayer;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace KBS.App.TaxonFinder.ViewModels
{
    public class TaxonMediaInfoViewModel : INotifyPropertyChanged
    {
        #region Fields

        private TaxonImage _selectedMedia;

        double currentScale = 1;
        double startScale = 1;
        double xOffset = 0;
        double yOffset = 0;

        #endregion


        #region Properties
        public int SelectedMediaId
        {
            set
            {
                SelectedMedia = ((App)App.Current).TaxonImages.FirstOrDefault(i => i.ImageId == value.ToString());
                OnPropertyChanged(nameof(SelectedMedia));
            }
        }

        public Guid SelectedMediaGuid
        {
            set
            {
                SelectedMedia = ((App)App.Current).TaxonImages.FirstOrDefault(i => i.ImageId == value.ToString());
                OnPropertyChanged(nameof(SelectedMediaGuid));
            }
        }

        public String SelectedMediaTitle
        {
            set
            {
                SelectedMedia = ((App)App.Current).TaxonImages.FirstOrDefault(i => i.ImageId == value.ToString());
                OnPropertyChanged(nameof(SelectedMediaTitle));
            }
        }


        public TaxonImage SelectedMedia
        {
            get
            {
                return _selectedMedia == null ? new TaxonImage() : _selectedMedia;
            }

            set
            {
                _selectedMedia = value;
                OnPropertyChanged(nameof(SelectedMedia));
            }
        }
        #endregion

        #region constructor
        public TaxonMediaInfoViewModel()
        {
            TapAddCommand = new Command<string>(async arg => await TapAdd(arg));
            OnPinchUpdatedCommand = new Command<Xamarin.Forms.PinchGestureUpdatedEventArgs>(async arg => await OnPinchUpdatedC(arg));
        }
        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region TapAdd Command
        public ICommand TapAddCommand { get; set; }
        private async Task TapAdd(string localName)
        {
            Trace.WriteLine(localName);
            //await Xamarin.Essentials.Launcher.OpenAsync(new Uri($"https://bodentierhochvier.de/erfassen/funde"));
        }
        #endregion


        #region TapAdd Command
        public ICommand OnPinchUpdatedCommand { get; set; }
        private async Task OnPinchUpdatedC(Xamarin.Forms.PinchGestureUpdatedEventArgs e)
        {
            Trace.WriteLine(e.ToString());
            //await Xamarin.Essentials.Launcher.OpenAsync(new Uri($"https://bodentierhochvier.de/erfassen/funde"));
        }
        #endregion

        private void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            /*
            switch (e.Status)
            {
                case GestureStatus.Started:
                    StartScale = Scale;
                    AnchorX = e.ScaleOrigin.X;
                    AnchorY = e.ScaleOrigin.Y;
                    break;

                case GestureStatus.Running:
                    double current = Scale + (e.Scale - 1) * StartScale;
                    Scale = Clamp(current, MIN_SCALE * (1 - OVERSHOOT), MAX_SCALE * (1 + OVERSHOOT));
                    break;

                case GestureStatus.Completed:
                    if (Scale > MAX_SCALE)
                        this.ScaleTo(MAX_SCALE, 250, Easing.SpringOut);
                    else if (Scale < MIN_SCALE)
                        this.ScaleTo(MIN_SCALE, 250, Easing.SpringOut);
                    break;
            }
            */
        }

        #endregion
    }
}
