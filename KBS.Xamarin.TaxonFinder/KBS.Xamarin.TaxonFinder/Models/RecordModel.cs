using SQLite;
using System;
using System.ComponentModel;
using System.Linq;

namespace KBS.App.TaxonFinder.Models
{
    /**SQLite library supports the following column types by default: string, int, double, byte[], DateTime.**/

    public class RecordModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private double _longitude;
        private double _latitude;
        private double? _height;
        private double? _accuracy;
        private int _taxonId;
        private string _taxonName;
        private string _taxonGuid;
        private bool _isEditable;
        private bool _isSynced;
        private bool _autoPosition;
        private int? _localityTemplateId;
        private string _userName;
        private string _reportedByName;
        private DateTime? _lastEdit;
        private int? _accuracyTypeId;
        private int _approvalStateId;
        private int? _diagnosisTypeId;

        [PrimaryKey]
        [AutoIncrement]
        public int LocalRecordId { get; set; }
        public string Identifier { get; set; }
        public bool IsSynced { get { return _isSynced; } set { _isSynced = value; OnPropertyChanged(nameof(IsSynced)); } }
        public bool IsEditable { get { return _isEditable; } set { _isEditable = value; OnPropertyChanged(nameof(IsEditable)); } }
        public string HabitatName { get; set; }
        public string HabitatDescription { get; set; }
        public string HabitatDescriptionForEvent { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime RecordDate { get; set; }
        public DateTime? LastEdit { get { return _lastEdit; } set { _lastEdit = value; } }
        public int? AccuracyTypeId { get { return _accuracyTypeId; } set { _accuracyTypeId = value; } }
        public DateTime? DeletionDate { get; set; }

        public int TaxonId { get { return _taxonId; } set { _taxonId = value; OnPropertyChanged(nameof(TaxonId)); OnPropertyChanged(nameof(TaxonName)); } }
        public string TaxonGuid { get { return _taxonGuid; } set { _taxonGuid = value; OnPropertyChanged(nameof(TaxonGuid)); OnPropertyChanged(nameof(TaxonName)); } }
        public string TaxonName { get { return _taxonName; } set { _taxonName = value; } }
        public double Latitude { get { return Math.Round(_latitude, 6); ; } set { _latitude = value; OnPropertyChanged(nameof(Latitude)); } }
        public double Longitude { get { return Math.Round(_longitude, 6); } set { _longitude = value; OnPropertyChanged(nameof(Longitude)); } }
        public double? Height { get { return _height; } set { _height = value; OnPropertyChanged(nameof(Height)); } }
        public double? Accuracy { get { return _accuracy; } set { _accuracy = value; OnPropertyChanged(nameof(Accuracy)); } }
        public int ApprovalStateId { get { return _approvalStateId; } set { _approvalStateId = value; OnPropertyChanged(nameof(ApprovalStateId)); } }
        public string UserName { get { return _userName; } set { _userName = value; } }
        public int? MaleCount { get; set; }
        public int? FemaleCount { get; set; }
        public string ReportedByName { get { return _reportedByName; } set { _reportedByName = value; } }
        public string ReportedByUserId { get; set; }
        public int TotalCount { get; set; }
        public bool AutoPosition { get { return _autoPosition; } set { _autoPosition = value; OnPropertyChanged(nameof(AutoPosition)); } }
        public Data.PositionInfo.PositionOption Position { get; set; }
        public int? LocalityTemplateId { get { return _localityTemplateId; } set { _localityTemplateId = value; } }
        public string ImageLegend { get; internal set; }
        public string ImageCopyright { get; set; }
        public bool StateEgg { get; set; }
        public bool StateLarva { get; set; }
        public bool StateNymph { get; set; }
        public bool StatePupa { get; set; }
        public bool StateDead { get; set; }
        public bool StateImago { get; set; }
        public int? DiagnosisTypeId { get { return _diagnosisTypeId; } set { _diagnosisTypeId = value; } }
        public int? GlobalAdviceId { get; set; }

        public RecordModel()
        {
        }
        public string SearchString(int taxonId)
        {

            var tempTaxonInfo = ((App)App.Current).Taxa.FirstOrDefault(i => i.TaxonId == taxonId);
            if (tempTaxonInfo == null)
                return "";
            return (string.IsNullOrEmpty(tempTaxonInfo.TaxonName) ? "" : tempTaxonInfo.TaxonName) + (string.IsNullOrEmpty(tempTaxonInfo.LocalName) ? "" : tempTaxonInfo.LocalName) + (string.IsNullOrEmpty(tempTaxonInfo.FamilyName) ? "" : tempTaxonInfo.FamilyName) + (string.IsNullOrEmpty(tempTaxonInfo.FamilyLocalName) ? "" : tempTaxonInfo.FamilyLocalName) + (string.IsNullOrEmpty(HabitatName) ? "" : HabitatName) + (string.IsNullOrEmpty(HabitatDescription) ? "" : HabitatDescription);

        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
