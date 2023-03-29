using System;
using System.Runtime.Serialization;

namespace KBS.App.TaxonFinder.Models
{
	public class AdviceJsonItem
	{
		[DataMember]
		public int ObservationId { get; set; }
		[DataMember]
		public string Id { get; set; }
		[DataMember]
		public string UserName { get; set; }
		[DataMember]
		public int AdviceId { get; set; }
		[DataMember]
		public Guid Identifier { get; set; }
		[DataMember]
		public int TaxonId { get; set; }
		[DataMember]
		public string TaxonFullName { get; set; }
		[DataMember]
		public DateTime? AdviceDate { get; set; }
		[DataMember]
		public int? AdviceCount { get; set; }
		[DataMember]
		public string AdviceCity { get; set; }
		[DataMember]
		public int? MaleCount { get; set; }
		[DataMember]
		public int? FemaleCount { get; set; }
		[DataMember]
		public bool StateEgg { get; set; }
		[DataMember]
		public bool StateLarva { get; set; }
		[DataMember]
		public bool StateImago { get; set; }
		[DataMember]
		public bool StateNymph { get; set; }
		[DataMember]
		public bool StatePupa { get; set; }
		[DataMember]
		public bool StateDead { get; set; }
		[DataMember]
		public string Comment { get; set; }
		[DataMember]
		public string HabitatDescriptionForEvent { get; set; }
		[DataMember]
		public string ReportedByName { get; set; }
		[DataMember]
		public string ImageCopyright { get; set; }
		[DataMember]
		public string ImageLegend { get; set; }
		[DataMember]
		public int UploadCode { get; set; }
		[DataMember]
		public decimal? Lat { get; set; }
		[DataMember]
		public decimal? Lon { get; set; }
		[DataMember]
		public string AreaWkt { get; set; }
		[DataMember]
		public int? Zoom { get; set; }
		[DataMember]
		public int? AccuracyType { get; set; }
		[DataMember]
		public string DeviceId { get; set; }
		[DataMember]
		public string DeviceHash { get; set; }
		[DataMember]
		public int? LocalityTemplateId { get; set; }
		[DataMember]
		public AdviceImageJsonItem[] Images { get; set; }
		[DataMember]
		public int? DiagnosisTypeId { get; set; }

	}

    public class UserDeleteRequest
    {
        public string DeviceHash { get; set; }
        public string DeviceId { get; set; }
        public string UserName { get; set; }

        public UserDeleteRequest(string v1, string v2, string v3)
        {
            this.DeviceId = v1;
            this.DeviceHash = v2;
            this.UserName = v3;
        }
    }

}
