using KBS.App.TaxonFinder.Data;
using KBS.App.TaxonFinder.Models;
using KBS.App.TaxonFinder.Services;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Newtonsoft.Json;
using static KBS.App.TaxonFinder.ViewModels.RecordListViewModel;
using System.Diagnostics;

namespace KBS.App.TaxonFinder.ViewModels
{
    public class RegisterDeviceViewModel : RegisterModel, INotifyPropertyChanged
    {
        #region Fields

        private readonly IMobileApi _mobileApi;
        private bool _isBusy;
        private bool _tryingToRegister;
        private bool _isLoggedIn;
        private string _result;
        private static RecordDatabase _database;

        #endregion

        #region Properties

        public string Username { get; set; }
        public string Password { get; set; }
        public string PasswordSecond { get; set; }
        public string Surname { get; set; }
        public string Givenname { get; set; }
        public string TopResult { get; set; }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        public bool TryingToRegister
        {

            get { return _tryingToRegister; }
            set
            {
                _tryingToRegister = value;
                OnPropertyChanged(nameof(TryingToRegister));

            }
        }

        public string Result
        {
            get { return _result; }
            set
            {
                _result = value;
                OnPropertyChanged(nameof(Result));
            }
        }
        public bool IsLoggedIn
        {
            get { return _isLoggedIn; }
            set
            {
                _isLoggedIn = value;
                OnPropertyChanged(nameof(IsLoggedIn));

            }
        }
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

        #endregion

        #region Constructor

        public RegisterDeviceViewModel() : base()
        {
            LoginCommand = new Command(async () => await Login(), () => !IsBusy);
            AddUserCommand = new Command(async () => await AddUser(), () => !IsBusy);
            LogoutCommand = new Command(async () => await Logout());
            _mobileApi = DependencyService.Get<IMobileApi>();

            IsLoggedIn = (Database.GetRegister() != null);
            if (IsLoggedIn)
            {
                TopResult = "Angemeldet als " + Database.GetRegister().Result.UserName;
                OnPropertyChanged(nameof(TopResult));
            }
        }

        #endregion

        #region Login Command

        public Command LoginCommand { get; set; }
        private async Task Login()
        {
            try
            {
                if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                    throw new Exception("Zur Anmeldung Internetverbindung herstellen.");

                if (Username == null || Password == null || Username == "" || Password == "")
                    throw new Exception("Bitte Eingaben überprüfen.");
                IsBusy = true;

                var result = await _mobileApi.Register(Username, Password);

                if (result == "invalid user")
                {
                    throw new Exception("Anmeldung fehlgeschlagen.");
                }
                else
                {
                    DeviceUserInfo dui = JsonConvert.DeserializeObject<DeviceUserInfo>(result);
                    await Database.Register(dui.DeviceHash, Username, dui.FirstName, dui.LastName);
                    IsBusy = false;
                    IsLoggedIn = true;
                    Result = "Erfolgreich angemeldet.";
                    TopResult = "Angemeldet als " + Username;
                    OnPropertyChanged(nameof(TopResult));
                }

            }
            catch (Exception e)
            {
                IsBusy = false;
                Result = e.Message;
            }

        }

        #endregion

        #region AddUser Command

        public Command AddUserCommand { get; set; }
        private async Task AddUser()
        {
            if (TryingToRegister)
            {
                try
                {
                    if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                        throw new Exception("Zur Registrierung Internetverbindung herstellen.");

                    if (String.IsNullOrEmpty(Username) || String.IsNullOrEmpty(Password) || String.IsNullOrEmpty(PasswordSecond) || String.IsNullOrEmpty(Surname) || String.IsNullOrEmpty(Givenname))
                        throw new Exception("Bitte Eingaben überprüfen.");

                    if (PasswordSecond != Password)
                        throw new Exception("Passwort stimmt nicht überein.");

                    //cf. Kbs.IdoWeb.Api -> Startup.cs
                    if (!Password.Any(char.IsDigit))
                    {
                        throw new Exception("Passwort muss eine Zahl enthalten.");
                    }

                    if (Password.Length < 8)
                    {
                        throw new Exception("Passwort muss aus mindestens 8 Zeichen bestehen.");
                    }


                    IsBusy = true;
                    Result = await _mobileApi.AddNewUser(Givenname, Surname, Username, Password, $"Registrierung über App {DateTime.Now.ToString("dd.MM.yyyy")}", "app");
                    IsBusy = false;
                    
                    //if result positive
                    //IsLoggedIn = true;
                    
                    if (Result == "success")
                    {
                        Result = "Registrierung als " + Username + " wurde beantragt und wird zeitnah bearbeitet. Es erfolgt eine Benachrichtigung per E-Mail.";
                    }
                    else
                    {
                        TopResult = Result;
                    }
                    OnPropertyChanged(nameof(TopResult));
                }
                catch (Exception e)
                {
                    IsBusy = false;
                    Result = e.Message;
                };
            }
            else
            {
                Result = "";
            }

        }

        public partial class DeviceUserInfo
        {
            public string DeviceHash { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        #endregion

        public async Task DeleteUser()
        {
            
            try
            {
                string DeviceId = DependencyService.Get<IDeviceId>().GetDeviceId();
                RegisterModel regModel = await Database.GetRegister();
                string DeviceHash = regModel.DeviceHash;
                string UserName = regModel.UserName;

                if (!String.IsNullOrEmpty(DeviceId) && !String.IsNullOrEmpty(DeviceHash) && !String.IsNullOrEmpty(UserName))
                {
                    UserDeleteRequest uDRequest = new UserDeleteRequest(DeviceId = DeviceId, DeviceHash = DeviceHash, UserName = UserName);
                    string Result = await _mobileApi.DeleteUser(uDRequest);
                    IsBusy = false;
                    //if result positive
                    if (Result.Contains("success"))
                    {
                        await Logout();
                        Result = "Der Account wurde erfolgreich gelöscht und Du wurdest ausgeloggt.";
                        await Application.Current.MainPage.DisplayAlert("Account gelöscht", "Der Account wurde erfolgreich gelöscht und Du wurdest ausgeloggt.", "Okay");
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Informationen fehlerhaft", "Wir konnten die benötigten Informationen nicht auslesen, bitte nutzen Sie die Feedback-Funktion der App um eine*n Admin zu kontaktieren", "Okay");
                    }
                    OnPropertyChanged(nameof(TopResult));
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Informationen fehlerhaft", "Wir konnten die benötigten Informationen nicht auslesen, bitte nutzen Sie die Feedback-Funktion der App um eine*n Admin zu kontaktieren", "Okay");
                }
            }
            catch (Exception e)
            {
                IsBusy = false;
                Result = e.Message;
            }
        }

        #region Logout Command

        public Command LogoutCommand { get; set; }
        private async Task Logout()
        {
            await Database.Logout();
            IsLoggedIn = false;
            Result = "";
            Username = "";
            Password = "";
            OnPropertyChanged(nameof(Username));
            OnPropertyChanged(nameof(Password));
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
