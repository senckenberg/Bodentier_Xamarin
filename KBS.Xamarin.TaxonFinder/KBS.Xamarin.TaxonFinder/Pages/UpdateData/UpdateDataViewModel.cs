using KBS.App.TaxonFinder.Data;
using KBS.App.TaxonFinder.Models;
using KBS.App.TaxonFinder.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace KBS.App.TaxonFinder.ViewModels
{

    public class UpdateDataViewModel : INotifyPropertyChanged
    {
        #region Fields

        private const string ServiceUrl = "https://www.bodentierhochvier.de/wp-content/uploads/bodentier_app_json/";
        private const string VersionsFile = "Versions.json";
        private static string _oldVersionDate;
        private static JObject _oldVersionJson;
        private readonly IFileHelper _fileHelper;
        private static IFileHelper _fileHelperStatic;
        private bool _isBusy;
        private string _dataStatus;
        private string _result;
        private const string hires_ending = "-225x300.jpg";
        private const string lores_ending = "-113x150.jpg";

        private static RecordDatabase _database;

        private Dictionary<char, string> sonderzeichenMap = new Dictionary<char, string>() {
          { 'ä', "ae" },
          { 'ö', "oe" },
          { 'ü', "ue" },
          { 'Ä', "Ae" },
          { 'Ö', "Oe" },
          { 'Ü', "Ue" },
          { 'ß', "ss" }
        };

        #endregion

        #region Properties

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }
        public string DataStatus
        {
            get { return _dataStatus; }
            set
            {
                _dataStatus = value;
                OnPropertyChanged(nameof(DataStatus));
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

        #endregion

        #region Constructor

        public UpdateDataViewModel()
        {
            LoadDataCommand = new Command(async () => await LoadData(false), () => !IsBusy);
            LoadDataCommand_lores = new Command(async () => await LoadData(true), () => !IsBusy);

            _fileHelper = DependencyService.Get<IFileHelper>();
            //check version file, display state
            if (_fileHelper.FileExists(VersionsFile))
            {
                var oldVersions = _fileHelper.GetAllText(VersionsFile);
                try
                {
                    _oldVersionJson = JObject.Parse(oldVersions);
                    _oldVersionDate = _oldVersionJson["Versions.json"].Value<string>();
                    var imageDate = Preferences.Get("imageDate", "");
                    if (imageDate != "")
                    {
                        DataStatus = $"Daten vom Stand {DateTime.Parse(_oldVersionDate).ToString("dd.MM.yyyy")}. Alles auf dem neusten Stand! Momentan sind keine Aktualisierungen erforderlich.";
                    }
                    else
                    {
                        DataStatus = "Daten bisher nur unvollständig aktualisiert.";
                    }
                }
                catch (Exception)
                {
                }
            }
            else
            {
                DataStatus = "Daten bisher noch nicht aktualisiert.";
            }
        }

        #endregion

        #region LoadData Command

        public Command LoadDataCommand { get; set; }
        public Command LoadDataCommand_lores { get; set; }
        private async Task LoadData(bool lores = false)
        {
            Result = "";

            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Application.Current.MainPage.DisplayAlert("Benötige Internetverbindung", "Zum Laden der Dateien wird eine Internetverbindung benötigt.", "Okay");
            }
            else if (await Application.Current.MainPage.DisplayAlert("Große Dateimengen herunterladen?", "Beim Fortfahren werden mehrere Megabyte (bis zu 500MB) Bilddaten heruntergeladen. Am besten sollte dies mit WLAN heruntergeladen werden.", "Fortfahren", "Abbrechen"))
            {
                IsBusy = true;
                await LoadJson();
                try
                {
                    Dictionary<string, string> filesList = new Dictionary<string, string>();
                    //var count = ((App)App.Current).TaxonImages.Count;
                    //count += tFilterItems.Count;
                    var tFilterItems = Load.FromFile<FilterItem>("TaxonFilterItems.json");
                    var i = 0;

                    foreach (var image in ((App)App.Current).TaxonImages)
                    {
                        string[] fileNames;
                        if (!lores)
                        {
                            fileNames = new string[] { $"{image.Title}{hires_ending}", $"{image.Title}.jpg" };
                        }
                        else
                        {
                            fileNames = new string[] { $"{image.Title}{lores_ending}", $"{image.Title}.jpg" };

                        }
                        if (!filesList.ContainsKey(fileNames[0]))
                        {
                            filesList.Add(fileNames[0], fileNames[1]);
                        }
                    }

                    foreach (FilterItem tFilter in tFilterItems)
                    {
                        string[] fileNames;
                        if (tFilter.ListSourceJson != null)
                        {
                            foreach (string img in tFilter.ListSourceJson)
                            {
                                if (img != null)
                                {
                                    if (!lores)
                                    {
                                        fileNames = new string[] { $"{img.Trim()}{hires_ending}", $"{img.Trim()}.jpg" };
                                    }
                                    else
                                    {
                                        fileNames = new string[] { $"{img.Trim()}{lores_ending}", $"{img.Trim()}.jpg" };
                                    }
                                    if (!filesList.ContainsKey(fileNames[0]))
                                    {
                                        filesList.Add(fileNames[0], fileNames[1]);
                                    }

                                }
                            }
                        }
                    }
                    //filesList = (List<string[]>)filesList.DistinctBy(x => x.).ToList();
                    var count = filesList.Count;

                    foreach (var fileName in filesList)
                    {
                        i++;
                        string localFilePath = _fileHelper.GetLocalAppPath($"{fileName.Value}");
                        if (!_fileHelper.FileExists(localFilePath))
                        {
                            try
                            {
                                string targetFileName = getRemoteFileName(fileName.Key, fileName.Value);
                                if (!String.IsNullOrEmpty(targetFileName))
                                {
                                    var downloadFile = _fileHelper.DownloadFileAsync($"https://www.bodentiere.de/wp-content/uploads/{targetFileName}");
                                    if (_fileHelper.RemoteFileExists($"https://www.bodentiere.de/wp-content/uploads/{targetFileName}"))
                                    {
                                        if (downloadFile != null)
                                        {

                                            await _fileHelper.CopyFileToLocalAsync(downloadFile, fileName.Value);
                                        }
                                    }

                                }
                                else
                                {
                                    Debug.WriteLine($"{fileName.Value} does not exist");
                                }


                            }
                            catch (Exception e)
                            {
                                throw e;
                            }
                        }
                        else
                        {

                        }
                        string percent = ((double)(i * 1000 / count) / 10).ToString("0.0");
                        DataStatus = $"Lade Bilder {i} von {count} ({percent} %)";
                    }

                    var versions = _fileHelper.GetAllText(VersionsFile);
                    var versionsJson = JObject.Parse(versions);
                    var versionDate = versionsJson["TaxonImages.json"].Value<string>();
                    var imageDate = versionsJson["Versions.json"].Value<string>();
                    Preferences.Set("imageDate", imageDate);
                    DataStatus = $"Daten vom Stand {DateTime.Parse(versionDate).ToString("dd.MM.yyyy")}";

                    //Update Taxon-References in Records Database
                    UpdateTaxonRefInRecords();

                    Result = "Daten erfolgreich aktualisiert.";
                }
                catch (Exception e)
                {
                    var msg = e.Message;
                    Result = "Beim Laden der Bilder ist ein Problem aufgetreten.";
                }
                finally
                {
                    IsBusy = false;
                }

            }
        }

        public async void UpdateTaxonRefInRecords()
        {
            try
            {
                List<RecordModel> records = await Database.GetRecordsAsync();
                if (((App)App.Current).Taxa != null)
                {
                    foreach (RecordModel rec in records)
                    {
                        //check if taxonid from record still in Taxa.json
                        Taxon tax = ((App)App.Current).Taxa.FirstOrDefault(i => i.TaxonId == rec.TaxonId);
                        if (tax != null)
                        {
                            if (tax.TaxonName != rec.TaxonName)
                            {
                                Taxon taxByName = ((App)App.Current).Taxa.FirstOrDefault(i => i.TaxonName == rec.TaxonName);
                                if (taxByName != null)
                                {
                                    rec.TaxonGuid = taxByName.Identifier.ToString();
                                    rec.TaxonId = taxByName.TaxonId;
                                    await Database.UpdateRecord(rec);
                                }
                                else
                                {
                                    Trace.WriteLine("Error mapping TaxonId values. Please review data");
                                    Trace.WriteLine($@"rec.LocalRecordId: {rec.LocalRecordId}, rec.TaxonName: {rec.TaxonName}, rec.TaxonId: {rec.TaxonId}");
                                }
                            }
                        }
                        else
                        {
                            //map by name
                            tax = ((App)App.Current).Taxa.FirstOrDefault(i => i.TaxonName == rec.TaxonName);
                            if (tax != null)
                            {
                                rec.TaxonId = tax.TaxonId;
                                rec.TaxonGuid = tax.Identifier.ToString();
                                await Database.UpdateRecord(rec);
                            }
                        }
                    }
                }
            } catch (Exception ex)
            {
                Trace.WriteLine("Fatal error updating recordmodel");
                Trace.Write(ex);
            }

        }

        public static RecordDatabase Database
        {
            get
            {
                return _database ?? (_database = new RecordDatabase(DependencyService.Get<IFileHelper>().GetLocalFilePath("RecordSQLite.db3")));
            }
        }


        public static async Task<string> CheckForUpdates()
        {
            string result = "Alle Daten sind aktuell";
            try
            {
                _fileHelperStatic = DependencyService.Get<IFileHelper>();
                var versionsUrl = ServiceUrl + VersionsFile;
                var remoteVersionsFile = await _fileHelperStatic.DownloadFileAsync(versionsUrl);

                //get last time updated
                var remoteVersionJsonString = System.Text.Encoding.UTF8.GetString(remoteVersionsFile, 0, remoteVersionsFile.Length);
                JObject remoteVersionJson = JObject.Parse(remoteVersionJsonString);

                var oldVersions = _fileHelperStatic.GetAllText(VersionsFile);
                _oldVersionJson = JObject.Parse(oldVersions); _oldVersionDate = _oldVersionJson["Versions.json"].Value<string>();

                var remoteVersionDate = remoteVersionJson["Versions.json"].Value<string>();
                if (_fileHelperStatic.FileExists(VersionsFile))
                {
                    //if internet files not already loaded (== _oldVersionDate not set) and more up to date
                    if (!String.IsNullOrEmpty(_oldVersionDate) && remoteVersionDate != _oldVersionDate)
                    {
                        //update all files out of date
                        int cnt = 0;
                        foreach (var file in remoteVersionJson)
                        {
                            var fileName = file.Key;
                            if (remoteVersionJson[fileName].Value<string>() != _oldVersionJson[fileName].Value<string>())
                            {
                                result = "Updates sind verfügbar";
                            }
                        }
                    }
                    else if (String.IsNullOrEmpty(_oldVersionDate))
                    {
                        result = "Updates sind verfügbar";
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                result = "Konnte nicht auf Updates prüfen.";
            }

            return result;
        }

        private string getRemoteFileName(string filePathRes, string filePathNoRes)
        {
            string filePathRes_clean = filePathRes.Aggregate(
              new StringBuilder(),
              (sb, c) => sonderzeichenMap.TryGetValue(c, out var r) ? sb.Append(r) : sb.Append(c)
              ).ToString();
            string filePathNoRes_clean = filePathNoRes.Aggregate(
              new StringBuilder(),
              (sb, c) => sonderzeichenMap.TryGetValue(c, out var r) ? sb.Append(r) : sb.Append(c)
              ).ToString();

            //Test without umlaute + resolution
            if (_fileHelper.RemoteFileExists($"https://www.bodentiere.de/wp-content/uploads/{filePathRes}"))
            {
                return filePathRes;
            }
            //Test with umlaute + resolution
            if (_fileHelper.RemoteFileExists($"https://www.bodentiere.de/wp-content/uploads/{filePathRes_clean}"))
            {
                return filePathRes_clean;
            }
            //Test without umlaute + without resolution
            if (_fileHelper.RemoteFileExists($"https://www.bodentiere.de/wp-content/uploads/{filePathNoRes}"))
            {
                return filePathNoRes;
            }
            //Test with umlaute + without resolution
            if (_fileHelper.RemoteFileExists($"https://www.bodentiere.de/wp-content/uploads/{filePathNoRes_clean}"))
            {
                return filePathNoRes_clean;
            }
            return null;
        }

        private async Task LoadJson()
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Application.Current.MainPage.DisplayAlert("Benötige Internetverbindung", "Zum Laden der Dateien wird eine Internetverbindung benötigt.", "Okay");
            }
            else
            {
                try
                {
                    DataStatus = "Lade Artinformationen …";
                    //load version file from internet
                    //json / bodentier_app_json
                    var versionsUrl = ServiceUrl + VersionsFile;
                    var versions = await _fileHelper.DownloadFileAsync(versionsUrl);

                    //get last time updated
                    var versionJsonString = System.Text.Encoding.UTF8.GetString(versions, 0, versions.Length);

                    JObject versionJson = JObject.Parse(versionJsonString);

                    var versionDate = versionJson["Versions.json"].Value<string>();
                    if (_fileHelper.FileExists(VersionsFile))
                    {
                        //if internet files not already loaded (== _oldVersionDate not set) and more up to date
                        if (!String.IsNullOrEmpty(_oldVersionDate) && versionDate != _oldVersionDate)
                        {
                            //update all files out of date
                            int cnt = 0;
                            foreach (var file in versionJson)
                            {
                                var fileName = file.Key;
                                if (versionJson[fileName].Value<string>() != _oldVersionJson[fileName].Value<string>())
                                {
                                    var jsonFile = await _fileHelper.DownloadFileAsync(ServiceUrl + fileName);
                                    _fileHelper.CopyFileToLocal(jsonFile, fileName);
                                    cnt++;
                                    if(versionJson != null && versionJson.Count != null)
                                    {
                                        Result = String.Format("Download der Update-Informationen {0} / {1} komplett.", cnt, versionJson.Count);
                                    } else
                                    {
                                        Result = String.Format("Lade der Update-Informationen .. ");
                                    }
                                }
                            }

                        }
                    }
                    //load all files for the first time
                    else
                    {
                        foreach (var firstFile in versionJson)
                        {
                            var firstFileName = firstFile.Key;
                            var firstJsonFile = await _fileHelper.DownloadFileAsync(ServiceUrl + firstFileName);
                            _fileHelper.CopyFileToLocal(firstJsonFile, firstFileName);
                        }
                    }

                    Result = "Download der Update-Info vollständig. Lade Daten in App ..";
                    //reload the Data in App
                    ((App)App.Current).LoadData();
                }
                catch (Exception e)
                {
                    var msg = e.InnerException;
                    Result = "Beim Laden der Artinformationen ist ein Problem aufgetreten.";
                    ((App)App.Current).LoadData();
                }
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
