﻿using KBS.App.TaxonFinder.Data;
using KBS.App.TaxonFinder.Services;
using KBS.App.TaxonFinder.Views;
using Newtonsoft.Json.Linq;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using PermissionStatus = Plugin.Permissions.Abstractions.PermissionStatus;

namespace KBS.App.TaxonFinder.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        #region Fields

        private static RecordDatabase _database;
        private static ContentPage _hintPage;
        private static string _hintPageAsString;
        private bool _actionNecessary;
        private string _hintLabelText;
        private string _hintButtonText;

        #endregion

        #region Properties

        public static RecordDatabase Database
        {
            get
            {
                if (_database == null)
                {
                    _database = new RecordDatabase(DependencyService.Get<IFileHelper>().GetLocalFilePath("RecordSQLite.db3"));
                }
                return _database;
            }
        }

        public bool ActionNecessary
        {
            get { return _actionNecessary; }
            set
            {
                _actionNecessary = value;
                OnPropertyChanged(nameof(ActionNecessary));
            }
        }
        public string HintLabelText
        {
            get { return _hintLabelText; }
            set
            {
                _hintLabelText = value;
                OnPropertyChanged(nameof(HintLabelText));
            }
        }
        public string HintButtonText
        {
            get { return _hintButtonText; }
            set
            {
                _hintButtonText = value;
                OnPropertyChanged(nameof(HintButtonText));
            }
        }
        private bool ImagesLoaded
        {
            get
            {
                var imageDate = Preferences.Get("imageDate", string.Empty);
                return !string.IsNullOrEmpty(imageDate);
            }
        }
        private bool ImagesUpdated
        {
            get
            {
                var imageDate = Preferences.Get("imageDate", string.Empty);
                if (String.IsNullOrEmpty(imageDate))
                {
                    return false;
                }
                return true;
                /*
                DateTime imageDateTime = DateTime.Now;
                if (!string.IsNullOrEmpty(imageDate))
                {
                    imageDateTime = DateTime.Parse(imageDate);
                }

                var appVersions = ((App)App.Current).AppVersions;
                var appDateTime = DateTime.Parse(appVersions["TaxonImages.json"].Value<string>());
                return appDateTime <= imageDateTime;
                */
            }
        }

        #endregion

        #region Constructor

        public MainPageViewModel()
        {
            GetToHintCommand = new Command(async () => await GetToHint());
            // Handle when your app starts
            //CheckForTutorial();
        }

        #endregion

        #region GetHint Command

        public ICommand GetToHintCommand { get; set; }
        private async Task GetToHint()
        {
            if (_hintPageAsString == "RegsterDevice")
            {
                await App.Current.MainPage.Navigation.PushAsync(new RegisterDevice());
            }
            else if (_hintPageAsString == "UpdateData")
            {
                await App.Current.MainPage.Navigation.PushAsync(new UpdateData());
            }
            else if (_hintPageAsString == "RecordList")
            {
                await App.Current.MainPage.Navigation.PushAsync(new RecordList());
            }
            else
            {
                //Fallback?
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Shows a hint, what to do next on the app.
        /// </summary>
        /// <returns>Returns a task containing a bool whether the user is logged in.</returns>
        public async Task<bool> GetHint()
        {
            bool registered = false;
            try
            {
                var statusStorage = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
                if (statusStorage != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Storage))
                    {
                        await Application.Current.MainPage.DisplayAlert("Benötige Speicher-Berechtigung", "Zum Speichern von Fundmeldungen, Bildern und der Anmeldung wird die Speicher-Berechtigung benötigt.", "Okay");
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Storage });
                    statusStorage = results[Permission.Storage];
                }
                else if (statusStorage == PermissionStatus.Granted)
                {
                    registered = true;
                    var synced = false;
                    try
                    {
                        registered = (Database.GetRegister() != null);
                    }
                    catch (Exception ex) // if database not set
                    {
                        Debug.WriteLine(ex.Message);
                        registered = false;
                    }
                    try
                    {
                        synced = !(Database.GetRecordsAsync().Result.Any(i => !i.IsSynced));
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        synced = true;
                    }

                    ActionNecessary = !synced;
                    if (!registered && !synced)
                    {
                        //_hintPage = new RegisterDevice();
                        _hintPageAsString = "RegisterDevice";
                        HintLabelText = "Zur Synchronisation von Fundmeldungen anmelden.";
                        HintButtonText = "Anmeldung öffnen";
                    }
                    else if (!synced)
                    {
                        //_hintPage = new RecordList();
                        _hintPageAsString = "RecordList";
                        HintLabelText = "Es gibt derzeit unsynchronisierte Fundmeldungen.";
                        HintButtonText = "Fundliste öffnen";
                    }
                    else
                    {
                        if (!ImagesLoaded)
                        {
                            ActionNecessary = true;
                            //_hintPage = new UpdateData();
                            _hintPageAsString = "UpdateData";
                            HintLabelText = "Aktualisiere die Artinformationen und lade Bilder zur Offline-Nutzung herunter.";
                            HintButtonText = "Aktualisierung öffnen";
                        }
                        else if (!ImagesUpdated)
                        {
                            ActionNecessary = true;
                            //_hintPage = new UpdateData();
                            _hintPageAsString = "UpdateData";
                            HintLabelText = "Neue Daten stehen zur Verfügung. Aktualisiere die Artinformationen und lade Bilder zur Offline-Nutzung herunter.";
                            HintButtonText = "Aktualisierung öffnen";
                        }
                        else
                        {
                            CheckForJsonUpdates();
                        }
                    }
                }
                else if (statusStorage != PermissionStatus.Unknown)
                {
                    await Application.Current.MainPage.DisplayAlert("Berechtigung verweigert", "Ohne diese Berechtigung ist die App nicht funktionsfähig.", "Okay");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return registered;
        }

        public async void CheckForTutorial()
        {
            string dontShowTutorial = Preferences.Get("dontShowTutorialOnStartup", "false");
            if (String.IsNullOrEmpty(dontShowTutorial) || dontShowTutorial == "false")
            {
                await App.Current.MainPage.Navigation.PushAsync(new Tutorial());
            }
        }

        public async void CheckForJsonUpdates()
        {
            try
            {
                var updateResult = await UpdateDataViewModel.CheckForUpdates();
                if(updateResult != "Alle Daten sind aktuell")
                {
                    ActionNecessary = true;
                    _hintPageAsString = "UpdateData";
                    HintButtonText = "Aktualisierung öffnen";
                    HintLabelText = updateResult;
                }
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
