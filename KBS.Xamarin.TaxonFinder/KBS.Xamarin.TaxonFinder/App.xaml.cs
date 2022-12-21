using KBS.App.TaxonFinder.Data;
using KBS.App.TaxonFinder.Models;
using KBS.App.TaxonFinder.Services;
using KBS.App.TaxonFinder.Views;
using NavigationSam;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace KBS.App.TaxonFinder
{
    public partial class App : Application
    {
        public FilterItem Filter { get; set; }
        public List<Taxon> Taxa { get; set; }
        public List<TaxonImage> TaxonImages { get; set; }
        public List<TaxonImageType> TaxonImageTypes { get; set; }
        public List<ProtectionClass> ProtectionClasses { get; set; }
        public List<TaxonFilterItem> TaxonFilterItems { get; set; }
        public List<TaxonProtectionClass> TaxonProtectionClasses { get; set; }
        public List<TaxonOrder> TaxonOrders { get; set; }
        public List<TaxonSynonym> TaxonSynonyms { get; set; }
        public MediaFileModel AudioRecording { get; set; }
        public JObject DownloadedVersions { get; set; }
        public JObject AppVersions { get; set; }

        private static RecordDatabase _database;


        const int smallWightResolution = 768;
        const int smallHeightResolution = 1280;

        public App()
        {
            InitializeComponent();
            LoadVersions();
            LoadData();
            CheckRecordsDatabase();
            LoadStyles();
            //MainPage = new NavigationPage(new MainPage());
            MainPage = new NavigationPageSam(new MainPage());

        }

        void LoadStyles()
        {
            if (IsASmallDevice())
            {
                dictionary.MergedDictionaries.Add(SmallDevicesStyle.SharedInstance);
            }
            else
            {
                dictionary.MergedDictionaries.Add(LargeDevicesStyle.SharedInstance);
            }
        }

        public async void CheckRecordsDatabase()
        {
            List<RecordModel> records = await Database.GetRecordsAsync();
            if (Taxa != null)
            {
                foreach (RecordModel rec in records)
                {
                    if (String.IsNullOrEmpty(rec.TaxonName))
                    {
                        Taxon tax = Taxa.FirstOrDefault(i => i.TaxonId == rec.TaxonId);
                        if (tax != null)
                        {
                            rec.TaxonName = tax.TaxonName;
                            if (rec.TaxonGuid == null)
                            {
                                rec.TaxonGuid = tax.Identifier.ToString();
                            }
                            await Database.UpdateRecord(rec);
                        }
                    }
                }
            }
        }

        public static RecordDatabase Database
        {
            get
            {
                return _database ?? (_database = new RecordDatabase(DependencyService.Get<IFileHelper>().GetLocalFilePath("RecordSQLite.db3")));
            }
        }


        public static bool IsASmallDevice()
        {
            // Get Metrics
            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;

            // Width (in pixels)
            var width = mainDisplayInfo.Width / mainDisplayInfo.Density;

            // Height (in pixels)
            var height = mainDisplayInfo.Height / mainDisplayInfo.Density;
            return (width <= smallWightResolution && height <= smallHeightResolution);
        }

        private void LoadVersions()
        {
            var versionsFile = "Versions.json";
            Assembly assembly = typeof(App).GetTypeInfo().Assembly;
            string assemblyName = assembly.GetName().Name;
            var fileHelper = DependencyService.Get<IFileHelper>();

            //get backup json
            string pathName = $"{assemblyName}.Resources.{versionsFile}";
            Stream stream = assembly.GetManifestResourceStream(pathName);
            using (var reader = new StreamReader(stream))
            {
                var result = reader.ReadToEnd();
                AppVersions = JObject.Parse(result);
            }

            //get downloaded json
            if (fileHelper.FileExists(versionsFile))
            {
                var result = fileHelper.GetAllText(versionsFile);
                DownloadedVersions = JObject.Parse(result);
            }
        }

        public void LoadData()
        {
            Filter = FilterItem.InitFilters();
            ProtectionClasses = ProtectionClass.FromDatabase();
            List<OrderSelectionDictionary> taxonDeterminationList = Load.FromFile<OrderSelectionDictionary>("OrderSelectionTaxa.json");
            TaxonFilterItems = Load.FromFile<TaxonFilterItem>("TaxonFilterItems.json");
            Taxa = Load.FromFile<Taxon>("Taxa.json").OrderBy(i => i.LocalName).ThenBy(i => i.TaxonName).ToList();
            TaxonImages = Load.FromFile<TaxonImage>("TaxonImages.json");
            TaxonImageTypes = Load.FromFile<TaxonImageType>("TaxonImageTypes.json");
            TaxonProtectionClasses = Load.FromFile<TaxonProtectionClass>("TaxonProtectionClasses.json");
            TaxonSynonyms = Load.FromFile<TaxonSynonym>("TaxonSynonyms.json");


            foreach (var taxon in Taxa)
            {
                taxon.Images = TaxonImages.Where(i => i.TaxonId == taxon.TaxonId && i.Index != null).OrderBy(i => i.Index).ToList();
                taxon.ImageTypes = TaxonImageTypes.Where(i => i.TaxonId == taxon.TaxonId && i.TaxonTypeId == taxon.TaxonTypeId).ToList();
                taxon.TotalCriteria = TaxonFilterItems.Where(i => i.TaxonId == taxon.TaxonId && i.TaxonTypeId == taxon.TaxonTypeId).Count();
                if (taxonDeterminationList.Select(i => i.OrderName).ToList().Contains(taxon.TaxonName))
                {
                    taxon.navigateToWebSlug = taxonDeterminationList.First(i => i.OrderName == taxon.TaxonName).OrderSlug;
                    taxon.hasDeterminationLink = true;
                    taxon.DeterminationNavLink = taxonDeterminationList.First(i => i.OrderName == taxon.TaxonName).OrderName;
                }
            }

            foreach (var fi in TaxonFilterItems)
            {
                if (fi.ListSourceJson != null)
                {
                    foreach (string lsj in fi.ListSourceJson)
                    {
                        if (!TaxonImages.Select(ti => ti.Title).ToList().Contains(lsj.Trim()))
                        {
                            TaxonImages.Add(new TaxonImage { Title = lsj.Trim(), ImageId = Guid.NewGuid().ToString() });
                        }
                    }
                }
            }
            TaxonOrders = Taxa.Where(i => i.HasDiagnosis).Select(i => new { i.OrderName, i.OrderLocalName }).Distinct().Select(i => new TaxonOrder() { OrderName = i.OrderName, OrderLocalName = i.OrderLocalName }).ToList();


        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        public class AlternateColorDataTemplateSelector : DataTemplateSelector
        {
            public DataTemplate EvenTemplate { get; set; }
            public DataTemplate UnevenTemplate { get; set; }

            protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
            {
                // TODO: Maybe some more error handling here
                return ((List<string>)((ListView)container).ItemsSource).IndexOf(item as string) % 2 == 0 ? EvenTemplate : UnevenTemplate;
            }
        }

        /**Helper for Dictionary OrderSelection**/
        public partial class OrderSelectionDictionary
        {
            public string OrderName { get; set; }
            public string OrderSlug { get; set; }
        }

    }
}
