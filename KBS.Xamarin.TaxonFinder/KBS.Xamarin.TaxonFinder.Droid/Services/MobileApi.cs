﻿using KBS.App.TaxonFinder.Data;
using KBS.App.TaxonFinder.Models;
using KBS.App.TaxonFinder.Services;
using KBS.App.TaxonFinder.Droid.Services;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.Generic;
using static KBS.App.TaxonFinder.ViewModels.RecordListViewModel;
using Android.Views;
using static Android.Graphics.ImageDecoder;

[assembly: Xamarin.Forms.Dependency(typeof(MobileApi))]
namespace KBS.App.TaxonFinder.Droid.Services
{
    public class MobileApi : IMobileApi
    {

        private HttpClient _client = new HttpClient() { MaxResponseContentBufferSize = 256000 };

        private static readonly string _serviceUrl = "https://www.idoweb.bodentierhochvier.de/api/";
        private static readonly string _serviceUrl_dev = "https://192.168.2.108:45455/api/";
        private static readonly string _loginUrl = $@"{_serviceUrl}ApplicationUser/Login/MobileV2";
        //previous versions use below
        //private static readonly string _loginUrl = $@"{_serviceUrl}ApplicationUser/Login/Mobile";
        private static readonly string _registerUrl = $@"{_serviceUrl}ApplicationUser/Register/Mobile";
        private static readonly string _deleteUrl = $@"{_serviceUrl}ApplicationUser/Delete/Mobile";
        private static readonly string _adviceServiceUrl_mobile = $@"{_serviceUrl}Advice/SaveAdvice/Mobile";
        private static readonly string _adviceServiceUrl_mobileSync = $@"{_serviceUrl}Advice/SyncAdviceList/Mobile";
        private static readonly string _adviceServiceUrl_mobileSync_dev = $@"{_serviceUrl_dev}Advice/SyncAdviceList/Mobile";
        private static readonly string _changesServiceUrl = $@"{_serviceUrl}Advice/SyncAdvices";
        private static readonly string _mailServiceUrl = $@"{_serviceUrl}values/Mail/SendFeedback";
        private static RecordDatabase _database;

        public MobileApi()
        {

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

        public async Task<string> Register(string userName, string password)
        {
            LoginModel contentJson = new LoginModel { username = userName, password = password, deviceId = DependencyService.Get<IDeviceId>().GetDeviceId() };
            StringContent content = new StringContent(JsonConvert.SerializeObject(contentJson), Encoding.UTF8, "application/json");
            try
            {
                /**TODO: replace SSL Validation **/
                var handler = new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                    MaxRequestContentBufferSize = 256000
                };
                _client = new HttpClient(handler);
                var response = await _client.PostAsync(_loginUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var response_content = await response.Content.ReadAsStringAsync();
                    if (response_content.Contains('"'))
                    {
                        string response_content_string = JsonConvert.SerializeObject(JsonConvert.DeserializeObject<ViewModels.RegisterDeviceViewModel.DeviceUserInfo>(response_content));
                        return response_content_string;
                    }
                    return response_content;
                }
                throw new Exception("Anmeldung fehlgeschlagen.");
            }
            catch (Exception e)
            {
                var exc = e.InnerException;
                throw new Exception("Anmeldung fehlgeschlagen.");
            }
        }

        public async Task<string> DeleteUser(UserDeleteRequest uDRequest)
        {
            try
            {
                var handler = new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                    MaxRequestContentBufferSize = 256000
                };

                _client = new HttpClient(handler);
                StringContent content = new StringContent(JsonConvert.SerializeObject(uDRequest), Encoding.UTF8, "application/json");

                var response = await _client.PostAsync(_deleteUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var response_content = await response.Content.ReadAsStringAsync();
                    return response_content;
                }
                return "Löschen fehlgeschlagen.";
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<string> AddNewUser(string givenname, string surname, string mail, string password, string comment, string source)
        {
            try
            {
                RegisterModel contentJson = new RegisterModel { givenname = givenname, surname = surname, mail = mail, password = password, comment = comment, source = source };
                /**TODO: replace SSL Validation **/
                var handler = new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                    MaxRequestContentBufferSize = 256000
                };

                _client = new HttpClient(handler);
                StringContent content = new StringContent(JsonConvert.SerializeObject(contentJson), Encoding.UTF8, "application/json");

                var response = await _client.PostAsync(_registerUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var response_content = await response.Content.ReadAsStringAsync();
                    return response_content;
                }
                return "Registrierung fehlgeschlagen.";
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public async Task<string> GetChangesByDevice(AuthorizationJson auth)
        {
            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                MaxRequestContentBufferSize = 256000,
            };
            var httpClient = new HttpClient(handler);
            var stringContent = new StringContent(JsonConvert.SerializeObject(auth), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(_changesServiceUrl, stringContent);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            throw new Exception("Übermittlung fehlgeschlagen.");
        }

        public async Task<string> SaveAdvicesByDevice(AdviceJsonItem[] adviceJsonItem)
        {
            var userName = Database.GetUserName();
            adviceJsonItem[0].UserName = userName;

            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                MaxRequestContentBufferSize = 256000,
            };
            _client = new HttpClient(handler);
            var stringContent = new StringContent(JsonConvert.SerializeObject(adviceJsonItem[0]), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(_adviceServiceUrl_mobile, stringContent);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            throw new Exception("Übermittlung fehlgeschlagen.");

        }

        public async Task<string> SendFeedback(string text, string mail)
        {
            if (mail == null) { mail = ""; }
            Uri feedbackUri = new Uri(string.Format("{0}?text={1}&adress={2}", _mailServiceUrl, Uri.EscapeDataString(text), Uri.EscapeDataString(mail)));

            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                MaxRequestContentBufferSize = 256000,
            };
            _client = new HttpClient(handler);
            var response = await _client.GetAsync(feedbackUri);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }

            throw new Exception("Übermittlung fehlgeschlagen.");
        }

        public async Task<string> SyncAdvices(SyncRequest syncRequest)
        {
            try
            {

                var handler = new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                    MaxRequestContentBufferSize = 256000
                };

                _client = new HttpClient(handler);
                var dbg = JsonConvert.SerializeObject(syncRequest);

                var stringContent = new StringContent(JsonConvert.SerializeObject(syncRequest), Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(_adviceServiceUrl_mobileSync, stringContent);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return content;
                }
                return null;
            }
            catch (Exception ex)
            {
                var db1 = ex;
                throw new Exception("Übermittlung fehlgeschlagen.");
            }


        }
    }

    public class RegisterModel
    {
        public string givenname { get; set; }
        public string surname { get; set; }
        public string mail { get; set; }
        public string password { get; set; }
        public string comment { get; set; }
        public string source { get; set; }
    }
    public class LoginModel
    {
        public string username { get; set; }
        public string password { get; set; }
        public string deviceId { get; set; }
    }

    public class TokenResponse
    {
        public string token { get; set; }
    }

}