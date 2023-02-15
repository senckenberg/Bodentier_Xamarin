﻿using KBS.App.TaxonFinder.Data;
using KBS.App.TaxonFinder.Models;
using KBS.App.TaxonFinder.Services;
using KBS.App.TaxonFinder.ViewModels;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;
using PermissionStatus = Plugin.Permissions.Abstractions.PermissionStatus;
using Position = Xamarin.Forms.Maps.Position;
using Application = Xamarin.Forms.Application;
using ListView = Xamarin.Forms.ListView;
using Xamarin.Forms.Internals;

namespace KBS.App.TaxonFinder.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecordEdit : ContentPage
    {
        private IGeolocator _location;
        private RecordEditViewModel RecordEditViewModel
        {
            get
            {
                return (RecordEditViewModel)BindingContext;
            }
        }
        private bool _initFieldSetupDone { get; set; }

        public List<DiagnosisTypeModel> _diagnosisTypeModels = new List<DiagnosisTypeModel>
        {
            new DiagnosisTypeModel { Id=-1, Name="keine Auswahl"},
            new DiagnosisTypeModel { Id=0,  Name="Beobachtung lebend (ohne optische Hilfmittel)" },
            new DiagnosisTypeModel { Id=1,  Name="Beobachtung lebend (Foto/Lupe)" },
            new DiagnosisTypeModel { Id=2,  Name="Bestimmung lebend (Mikroskop)" },
            new DiagnosisTypeModel { Id=3,  Name="Bestimmung tot, äußere Merkmale (Mikroskop)"  },
            new DiagnosisTypeModel { Id=4,  Name="Bestimmung tot, Präparation (Mikroskop)" },
            new DiagnosisTypeModel { Id=5,  Name="DNA-Untersuchung"  }
        };


        public RecordEdit(int taxonId)
        {
            _initFieldSetupDone = false;
            InitializeComponent();
            LoadTaxonPicker();
            //LoadDiagnosisTypePicker();

            RecordDate.Date = DateTime.Now;
            TotalCount.Text = "1";

            if (taxonId > 0)
            {
                RecordEditViewModel.TaxonId = taxonId;
                var taxon = ((App)App.Current).Taxa.FirstOrDefault(i => i.TaxonId == taxonId);
                RecordEditViewModel.TaxonGuid = taxon.Identifier.ToString();
                //TaxonNameLabel.Text = taxon != null ? taxon.LocalName : "Unbekannte Art";

            }
            else
            {
                RecordEditViewModel.TaxonId = taxonId;
                //TaxonNameLabel.Text = "Unbekannte Art";
            }

            RecordEditViewModel.ExistingRecord = false;
            TemplateSwitch.IsToggled = true;
            RecordEditViewModel.IsEditable = true;
            RecordEditViewModel.Position = PositionInfo.PositionOption.Pin;
            RecordEditViewModel.Latitude = 51.160;
            RecordEditViewModel.Longitude = 10.442;

            /*
            if (RecordEditViewModel.DiagnosisTypeId != null)
            {
                DiagnosisTypePicker.SelectedItem = RecordEditViewModel.DiagnosisTypeId;
            }
            else
            {
                DiagnosisTypePicker.SelectedIndex = -1;
            }
            */

            LoadMap();
            UpdateTaxonPicker();
            SetDiagnosisTypePicker();
            UpdateReportedByNameLabel();
            _initFieldSetupDone = true;

        }

        public RecordEdit(int localRecordId, int taxonId)
        {
            InitializeComponent();
            LoadTaxonPicker();
            //LoadDiagnosisTypePicker();

            RecordEditViewModel.SelectedRecordId = localRecordId;
            var taxon = ((App)App.Current).Taxa.FirstOrDefault(i => i.Identifier == Guid.Parse(RecordEditViewModel.TaxonGuid));
            OnPropertyChanged(nameof(RecordEditViewModel.TaxonId));
            OnPropertyChanged(nameof(RecordEditViewModel.TaxonName));
            //TaxonNameLabel.Text = taxon != null ? taxon.LocalName : "Unbekannte Art";
            RecordAreaButton.Text = "Fundort verändern";
            RecordEditViewModel.ExistingRecord = true;
            TemplateSwitch.IsToggled = true;
            DeleteButton.Text = "Löschen";
            Title = "Details des Fundes";

            /*
            if (RecordEditViewModel.DiagnosisTypeId != null)
            {
                DiagnosisTypePicker.SelectedItem = RecordEditViewModel.DiagnosisTypeId;

            }
            else
            {
                DiagnosisTypePicker.SelectedIndex = -1;
            }
            */
            SetPosition();
            SetDiagnosisTypePicker();
            UpdateTaxonPicker();
            _initFieldSetupDone = true;
        }

        public void UpdateTaxonPicker()
        {
            if (RecordEditViewModel.TaxonGuid != null)
            {
                var ix = RecordEditViewModel.TaxonPickerItemsSource.Select(tx => tx.Identifier).ToList().IndexOf(Guid.Parse(RecordEditViewModel.TaxonGuid));
                TaxonPicker.SelectedIndex = ix;
            }
        }

        public void UpdateReportedByNameLabel()
        {
            try
            {
                RecordEditViewModel.UpdateReportedByName();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        public RecordEdit(int localRecordId, int taxonId, bool copyRecord)
        {
            InitializeComponent();
            LoadTaxonPicker();

            RecordEditViewModel.TaxonId = taxonId;
            OnPropertyChanged(nameof(RecordEditViewModel.TaxonId));
            OnPropertyChanged(nameof(RecordEditViewModel.TaxonName));
            var taxon = ((App)App.Current).Taxa.FirstOrDefault(i => i.TaxonId == taxonId);
            //TaxonNameLabel.Text = taxon != null ? taxon.LocalName : "Unbekannte Art";
            RecordAreaButton.Text = "Fundort verändern";
            RecordEditViewModel.CopyRecordId = localRecordId;
            RecordEditViewModel.ExistingRecord = false;
            TemplateSwitch.IsToggled = true;

            if (RecordEditViewModel.DiagnosisTypeId != null)
            {
                DiagnosisTypePicker.SelectedItem = RecordEditViewModel.DiagnosisTypeId;
            }
            else
            {
                DiagnosisTypePicker.SelectedIndex = -1;
            }

            SetPosition();
            UpdateTaxonPicker();
        }
        private void LocationService_PositionChanged(object sender, Services.PositionEventArgs e)
        {
            MapSpan span = MapSpan.FromCenterAndRadius(new Position(e.Position.Latitude, e.Position.Longitude), new Distance(1000));
            map.MoveToRegion(span);

            RecordEditViewModel.Accuracy = e.Position.Accuracy;
            RecordEditViewModel.Latitude = e.Position.Latitude;
            RecordEditViewModel.Longitude = e.Position.Longitude;
            RecordEditViewModel.Height = e.Position.Altitude;
            UpdatePin();
        }

        public async void LoadTaxonPicker()
        {
            List<Taxon> ItemsSource = await RecordEditViewModel.SetTaxonPickerItemsSource();
            ItemsSource.Add(new Taxon { TaxonId = 0, TaxonName = "Unbekannte Art" });
            RecordEditViewModel.TaxonPickerItemsSource = ItemsSource;
            TaxonPicker.ItemsSource = ItemsSource;
            TaxonPicker.ItemDisplayBinding = new Binding("TaxonName");
            SetTaxonPicker();
        }

        /*
        public async void LoadDiagnosisTypePicker()
        {
            try
            {
                //xam sucks
                DiagnosisTypePicker = new Xamarin.Forms.Picker();
                DiagnosisTypePicker.ItemsSource = new List<DiagnosisTypeModel>();
                DiagnosisTypePicker.SetBinding(Xamarin.Forms.Picker.ItemsSourceProperty, "DiagnosisTypeModel");
                DiagnosisTypePicker.SetBinding(Xamarin.Forms.Picker.SelectedItemProperty, "DiagnosisTypeModel");
                DiagnosisTypePicker.ItemDisplayBinding = new Binding("DiagnosisTypeModel.Name");
                foreach (DiagnosisTypeModel diagnosisTypeModel in _diagnosisTypeModels)
                {
                    DiagnosisTypePicker.ItemsSource.Add(new DiagnosisTypeModel { Id = diagnosisTypeModel.Id, Name = diagnosisTypeModel.Name });
                }

            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }
        */

        public async void SetDiagnosisTypePicker()
        {
            try
            {
                if (DiagnosisTypePicker.ItemsSource != null)
                {
                    var dbg = _diagnosisTypeModels.FindIndex(i => i.Id == RecordEditViewModel.DiagnosisTypeId);
                    DiagnosisTypePicker.SelectedIndex = _diagnosisTypeModels.FindIndex(i => i.Id == RecordEditViewModel.DiagnosisTypeId);

                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                DiagnosisTypePicker.SelectedIndex = -1;
            }
            OnPropertyChanged(nameof(DiagnosisTypePicker));
        }


        public async void SetTaxonPicker()
        {
            if (RecordEditViewModel.TaxonGuid != null)
            {

                var ix = RecordEditViewModel.TaxonPickerItemsSource.Select(taxonPickerI => taxonPickerI.Identifier).ToList().IndexOf(Guid.Parse(RecordEditViewModel.TaxonGuid.ToString()));
                TaxonPicker.SelectedIndex = ix;
            }
        }


        void OnTaxonPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Xamarin.Forms.Picker)sender;
            int selectedIndex = picker.SelectedIndex;

            if (selectedIndex > 0)
            {
                Taxon result = (Taxon)picker.ItemsSource[selectedIndex];
                if (result.TaxonId != 0)
                {
                    RecordEditViewModel.TaxonId = result.TaxonId;
                    RecordEditViewModel.TaxonGuid = result.Identifier.ToString();
                }
                else
                {
                    RecordEditViewModel.TaxonId = 0;
                }
                //SetLocalityTemplatePicker();
            }
            else
            {
            }
        }

        void OnDiagnosisTypePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            Xamarin.Forms.Picker picker = (Xamarin.Forms.Picker)sender;
            if (_initFieldSetupDone)
            {
                int selectedIndex = picker.SelectedIndex;
                if (selectedIndex > 0)
                {
                    DiagnosisTypeModel result = (DiagnosisTypeModel)picker.ItemsSource[selectedIndex];
                    RecordEditViewModel.DiagnosisTypeId = result.Id;
                }
                else
                {
                    RecordEditViewModel.DiagnosisTypeId = -1;
                }
            }
        }

        private async void LoadMap()
        {
            //map.Pins.Clear();
            //map.RouteCoordinates.Clear();
            //map.ShapeCoordinates.Clear();
            //map.IsVisible = true;

            //map.MapPositionOption = CurrentRecordEdit.Position;
            //switch (CurrentRecordEdit.Position)
            //{
            //	case PositionInfo.PositionOption.Pin:
            //		map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(CurrentRecordEdit.Latitude, CurrentRecordEdit.Longitude), new Distance(100)));
            //		UpdatePin();
            //		break;
            //	case PositionInfo.PositionOption.Line:
            //		foreach (var position in CurrentRecordEdit.PositionList)
            //		{
            //			map.RouteCoordinates.Add(new Position(position.Latitude, position.Longitude));
            //		}
            //		map.MoveToRegion(MapSpan.FromCenterAndRadius(GetCenterOfPositions(CurrentRecordEdit.PositionList), new Distance(GetDistanceFromCenter(CurrentRecordEdit.PositionList))));
            //		break;
            //	case PositionInfo.PositionOption.Area:
            //		foreach (var position in CurrentRecordEdit.PositionList)
            //		{
            //			map.ShapeCoordinates.Add(new Position(position.Latitude, position.Longitude));
            //		}
            //		map.MoveToRegion(MapSpan.FromCenterAndRadius(GetCenterOfPositions(CurrentRecordEdit.PositionList), new Distance(GetDistanceFromCenter(CurrentRecordEdit.PositionList))));
            //		break;
            //	default:
            //		break;
            //}

            MapSpan span = MapSpan.FromCenterAndRadius(new Position(RecordEditViewModel.Latitude, RecordEditViewModel.Longitude), new Distance(75000));
            map.MoveToRegion(span);

            try
            {
                var statusLocation = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                if (statusLocation != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                    {
                        await Application.Current.MainPage.DisplayAlert("Benötige Standort-Berechtigung", "Zum automatischen Feststellen des Fundorts wird die Standort-Berechtigung benötigt.", "Okay");
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Location });
                    statusLocation = results[Permission.Location];
                }
                if (statusLocation == PermissionStatus.Granted)
                {
                    _location = DependencyService.Get<IGeolocator>();
                }
                else if (statusLocation != PermissionStatus.Unknown)
                {
                    await Application.Current.MainPage.DisplayAlert("Berechtigung verweigert", "Ohne Berechtigung kann der Standort nicht automatisch festgestellt werden.", "Okay");
                }
            }
            catch
            {
            }

            _location = DependencyService.Get<IGeolocator>();

            // LoadMap();

            if (!RecordEditViewModel.ExistingRecord && _location != null && _location.IsGeolocationAvailable && _location.IsGeolocationEnabled)
            {
                try
                {
                    _location.PositionChanged += LocationService_PositionChanged;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

                if (_location.IsListening)
                {
                    try
                    {
                        _location.StopListening();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
                try
                {
                    _location.StartListening(10000, 30);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                RecordEditViewModel.AutoPosition = true;
                AutoPositionSwitch.IsEnabled = true;
            }
            else
            {
                RecordEditViewModel.AutoPosition = false;
                AutoPositionSwitch.IsEnabled = false;
            }

            if (Device.RuntimePlatform == Device.Android)
            {
                MapHolder.Scrolled += OnScrolled;
            }

        }
        private void SetPosition()
        {
            MapSpan span = MapSpan.FromCenterAndRadius(new Position(RecordEditViewModel.Latitude, RecordEditViewModel.Longitude), new Distance(1000));
            map.MoveToRegion(span);
            UpdatePin();
            /*
            _location = DependencyService.Get<IGeolocator>();

            if (_location != null && _location.IsGeolocationAvailable && _location.IsGeolocationEnabled)
            {
                _location.PositionChanged += LocationService_PositionChanged;

                if (_location.IsListening)
                {
                    try
                    {
                        _location.StopListening();
                    }
                    catch { }
                }
                CurrentRecordEdit.AutoPosition = false;
            }
            else
            {
                CurrentRecordEdit.AutoPosition = false;
                AutoPositionSwitch.IsEnabled = false;
            }

            if (Device.RuntimePlatform == Device.Android)
            {
                MapHolder.Scrolled += OnScrolled;
            }
            */
        }

        public void CountChanged(object sender, TextChangedEventArgs e)
        {
            if (_initFieldSetupDone)
            {
                if (string.IsNullOrEmpty(FemaleCount.Text))
                {
                    RecordEditViewModel.FemaleCount = null;
                }
                if (string.IsNullOrEmpty(MaleCount.Text))
                {
                    RecordEditViewModel.MaleCount = null;
                }

                if (RecordEditViewModel.FemaleCount != null || RecordEditViewModel.MaleCount != null)
                {
                    var fCount_temp = 0;
                    var mCount_temp = 0;
                    if (RecordEditViewModel.FemaleCount != null)
                    {
                        fCount_temp = RecordEditViewModel.FemaleCount.Value;
                    }
                    if (RecordEditViewModel.MaleCount != null)
                    {
                        mCount_temp = RecordEditViewModel.MaleCount.Value;
                    }
                    int total_temp = fCount_temp + mCount_temp;
                    if (total_temp != 0)
                    {
                        RecordEditViewModel.TotalCount = total_temp;
                        TotalCount.Text = RecordEditViewModel.TotalCount.ToString();
                        //TotalCount.Text = total_temp.ToString();
                    }
                }
                else if (String.IsNullOrEmpty(TotalCount.Text))
                {
                    RecordEditViewModel.TotalCount = -1;
                }
            }
        }

        private void OnScrolled(object sender, ScrolledEventArgs e)
        {
            if (MapHolder.ScrollY > 210 && MapHolder.ScrollY < 375)
            {
                MapHolder.InputTransparent = true;
            }
            else
            {
                MapHolder.InputTransparent = false;
            }
        }
        private void UpdatePin()
        {
            var pin = new Pin()
            {
                Position = new Position(RecordEditViewModel.Latitude, RecordEditViewModel.Longitude),
                Label = "Fundort",
                Type = PinType.SearchResult
            };
            map.Pins.Clear();
            map.Pins.Add(pin);
        }

        private void RecordAreaButton_Clicked(object _sender, EventArgs e)
        {
            MessagingCenter.Subscribe<RecordArea, Position>(this, "Pin", (sender, arg) =>
            {
                RecordEditViewModel.Position = PositionInfo.PositionOption.Pin;
                RecordEditViewModel.Latitude = arg.Latitude;
                RecordEditViewModel.Longitude = arg.Longitude;
                RecordEditViewModel.PositionList.Clear();
                Unsubscribe();
            });
            MessagingCenter.Subscribe<RecordArea, ObservableCollection<Position>>(this, "Line", (sender, arg) =>
            {
                RecordEditViewModel.Position = PositionInfo.PositionOption.Line;
                RecordEditViewModel.Latitude = 0;
                RecordEditViewModel.Longitude = 0;
                RecordEditViewModel.PositionList.Clear();
                RecordEditViewModel.PositionList = arg;
                Unsubscribe();
            });
            MessagingCenter.Subscribe<RecordArea, ObservableCollection<Position>>(this, "Area", (sender, arg) =>
            {
                RecordEditViewModel.Position = PositionInfo.PositionOption.Area;
                RecordEditViewModel.Latitude = 0;
                RecordEditViewModel.Longitude = 0;
                RecordEditViewModel.PositionList.Clear();
                RecordEditViewModel.PositionList = arg;
                Unsubscribe();

            });

            if (RecordEditViewModel.Position == PositionInfo.PositionOption.Line || RecordEditViewModel.Position == PositionInfo.PositionOption.Area)
                Navigation.PushAsync(new RecordArea(GetCenterOfPositions(RecordEditViewModel.PositionList).Latitude, GetCenterOfPositions(RecordEditViewModel.PositionList).Longitude, GetDistanceFromCenter(RecordEditViewModel.PositionList)));
            else if (RecordEditViewModel.Longitude != 0 && RecordEditViewModel.Latitude != 0)
                Navigation.PushAsync(new RecordArea(RecordEditViewModel.Latitude, RecordEditViewModel.Longitude, 100));
            else
                Navigation.PushAsync(new RecordArea());
        }

        private void Unsubscribe()
        {
            MessagingCenter.Unsubscribe<RecordArea, Position>(this, "Pin");
            MessagingCenter.Unsubscribe<RecordArea, ObservableCollection<Position>>(this, "Line");
            MessagingCenter.Unsubscribe<RecordArea, ObservableCollection<Position>>(this, "Area");

            if ((RecordEditViewModel.Position == PositionInfo.PositionOption.Pin && RecordEditViewModel.Latitude != 0 && RecordEditViewModel.Longitude != 0) ||
            (RecordEditViewModel.Position != PositionInfo.PositionOption.Pin && RecordEditViewModel.PositionList.Count != 0))
            {
                RecordAreaButton.Text = "Fundort verändern";
            }
            LoadMap();
        }
        public static double GetDistanceFromCenter(ObservableCollection<Position> positionList)
        {
            var center = GetCenterOfPositions(positionList);
            var distance = 0.0d;
            foreach (var pos in positionList)
            {
                distance = Math.Max(distance, Location.CalculateDistance(pos.Latitude, pos.Longitude, center.Latitude, center.Longitude, DistanceUnits.Kilometers));
            };
            return 1000 * distance;
        }
        public static Position GetCenterOfPositions(ObservableCollection<Position> positionList)
        {
            return new Position(positionList.Average(i => i.Latitude), positionList.Average(i => i.Longitude));
        }
        private void TemplateSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            SaveButton.Text = (e.Value) ? "Speichern" : "Absenden";
        }
        private void MediaListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var media = ((ListView)sender).SelectedItem as MediaFileModel;
            if (media.Media == MediaFileModel.MediaType.Image)
                Navigation.PushAsync(new TaxonMediaInfo(media.Path, RecordEditViewModel.TaxonId));
            if (media.Media == MediaFileModel.MediaType.Audio)
                Navigation.PushAsync(new TakeAudio(media.Path));
        }

        private void SaveButton_Clicked(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(PlaceEntry.Text))
            {
                PlaceEntry.Focus();
            }
            else if (String.IsNullOrEmpty(TotalCount.Text) || int.Parse(TotalCount.Text) < 1)
            {
                TotalCount.Focus();
            }
            else if ((RecordEditViewModel.Position == PositionInfo.PositionOption.Pin && RecordEditViewModel.Latitude == 0 && RecordEditViewModel.Longitude == 0) ||
            (RecordEditViewModel.Position != PositionInfo.PositionOption.Pin && RecordEditViewModel.PositionList.Count == 0))
            {
                Application.Current.MainPage.DisplayAlert("Fundort festlegen", "Lege zunächste einen Fundort fest.", "Okay");
                RecordAreaButton_Clicked(null, null);
            }
        }

        private void Help_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new HelpPage(this));
        }
        private void RecordMap_Tap(object sender, CustomRenderers.TapEventArgs e)
        {
            if (RecordEditViewModel.IsEditable)
            {
                if (RecordEditViewModel.AutoPosition)
                {
                    RecordEditViewModel.AutoPosition = false;
                }
            }

            RecordEditViewModel.Latitude = e.Position.Latitude;
            RecordEditViewModel.Longitude = e.Position.Longitude;

            UpdatePin();
        }

        private void AutoPositionSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                if (!_location.IsListening)
                {
                    try
                    {
                        _location.StartListening(10000, 30);
                    }
                    catch { }
                }
            }
            else
            {
                try
                {
                    _location.StopListening();
                }
                catch { }
            }
        }
        protected override void OnAppearing()
        {
            ((RecordEditViewModel)BindingContext).AddAudio();
            base.OnAppearing();
        }

    }
}
