using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;

namespace KBS.App.TaxonFinder.Models
{
    [Serializable]
    public class AdviceJsonItemSync : ISerializable
    {
        [DataMember]
        public int? GlobalAdviceId { get; set; }
        [DataMember]
        public Guid Identifier { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public int TaxonId { get; set; }
        [DataMember]
        public string TaxonFullName { get; set; }
        [DataMember]
        public Guid? TaxonGuid { get; set; }
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
        public string Lat { get; set; }
        [DataMember]
        public string Lon { get; set; }
        [DataMember]
        public int? AccuracyTypeId { get; set; }
        [DataMember]
        public int? LocalityTemplateId { get; set; }
        [DataMember]
        public string Md5Checksum { get; set; }
        [DataMember]
        public DateTime? LastEditDate { get; set; }
        [DataMember]
        public DateTime? DeletionDate { get; set; }
        [DataMember]
        public bool IsSynced { get; set; }
        [DataMember]
        public int ApprovalStateId { get; set; }
        [DataMember]
        public int? DiagnosisTypeId { get; set; }
        [DataMember]
        public List<AdviceImageJsonItem> Images { get; set; }

        public AdviceJsonItemSync() { }
        protected AdviceJsonItemSync(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new System.ArgumentNullException("info");
            }
            GlobalAdviceId = (int?)info.GetValue("GlobalAdviceId", typeof(int?));
            Identifier = (Guid)info.GetValue("Identifier", typeof(Guid));
            UserName = (string)info.GetValue("UserName", typeof(string));
            LastEditDate = (DateTime)info.GetValue("LastEditDate", typeof(DateTime));
            DeletionDate = (DateTime?)info.GetValue("DeletionDate", typeof(DateTime?));
            TaxonId = (int)info.GetValue("TaxonId", typeof(int));
            TaxonFullName = (string)info.GetValue("TaxonFullName", typeof(string));
            TaxonGuid = (Guid?)info.GetValue("TaxonGuid", typeof(Guid?));
            AdviceDate = (DateTime)info.GetValue("AdviceDate", typeof(DateTime));
            AdviceCount = (int)info.GetValue("AdviceCount", typeof(int));
            AdviceCity = (string)info.GetValue("AdviceCity", typeof(string));
            MaleCount = (int?)info.GetValue("MaleCount", typeof(int?));
            FemaleCount = (int?)info.GetValue("FemaleCount", typeof(int?));
            StateEgg = (bool)info.GetValue("StateEgg", typeof(bool));
            StateLarva = (bool)info.GetValue("StateLarva", typeof(bool));
            StateImago = (bool)info.GetValue("StateImago", typeof(bool));
            StateNymph = (bool)info.GetValue("StateNymph", typeof(bool));
            StatePupa = (bool)info.GetValue("StatePupa", typeof(bool));
            StateDead = (bool)info.GetValue("StateDead", typeof(bool));
            Comment = (string)info.GetValue("Comment", typeof(string));
            HabitatDescriptionForEvent = (string)info.GetValue("HabitatDescriptionForEvent", typeof(string));
            DiagnosisTypeId = (int?)info.GetValue("DiagnosisTypeId", typeof(int?));
            ReportedByName = (string)info.GetValue("ReportedByName", typeof(string));
            Lat = (string)info.GetValue("Lat", typeof(string));
            Lon = (string)info.GetValue("Lon", typeof(string));
            AccuracyTypeId = (int?)info.GetValue("AccuracyTypeId", typeof(int?));
            ApprovalStateId= (int)info.GetValue("ApprovalStateId", typeof(int));
            LocalityTemplateId = (int?)info.GetValue("LocalityTemplateId", typeof(int?));
            Md5Checksum = (string)info.GetValue("Md5Checksum", typeof(string));
            Images = (List<AdviceImageJsonItem>)info.GetValue("Images", typeof(List<AdviceImageJsonItem>));
            IsSynced = (bool)info.GetValue("IsSynced", typeof(bool));
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("GlobalAdviceId", GlobalAdviceId);
            info.AddValue("Identifier", Identifier);
            info.AddValue("UserName", UserName);
            info.AddValue("LastEditDate", LastEditDate);
            info.AddValue("DeletionDate", DeletionDate);
            info.AddValue("TaxonId", TaxonId);
            info.AddValue("TaxonFullName", TaxonFullName);
            info.AddValue("TaxonGuid", TaxonGuid);
            info.AddValue("AdviceDate", AdviceDate);
            info.AddValue("AdviceCount", AdviceCount);
            info.AddValue("AdviceCity", AdviceCity);
            info.AddValue("MaleCount", MaleCount);
            info.AddValue("FemaleCount", FemaleCount);
            info.AddValue("StateEgg", StateEgg);
            info.AddValue("StateLarva", StateLarva);
            info.AddValue("StateImago", StateImago);
            info.AddValue("StateNymph", StateNymph);
            info.AddValue("StatePupa", StatePupa);
            info.AddValue("StateDead", StateDead);
            info.AddValue("Comment", Comment);
            info.AddValue("HabitatDescriptionForEvent", HabitatDescriptionForEvent);
            info.AddValue("ReportedByName", ReportedByName);
            info.AddValue("ImageCopyright", ImageCopyright);
            info.AddValue("DiagnosisTypeId", DiagnosisTypeId);
            info.AddValue("ImageLegend", ImageLegend);
            info.AddValue("Lat", Lat);
            info.AddValue("Lon", Lon);
            info.AddValue("AccuracyTypeId", AccuracyTypeId);
            info.AddValue("ApprovalStateId", ApprovalStateId);
            info.AddValue("LocalityTemplateId", LocalityTemplateId);
            info.AddValue("Md5Checksum", Md5Checksum);
            info.AddValue("IsSynced", IsSynced);
            info.AddValue("Images", Images);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            GetObjectData(info, context);
        }

        public void GenerateItemHashV2 ()
        {
            AdviceJsonItemSync adviceCopyForCleanHash = this;
            //adviceCopyForCleanHash.Lat = adviceCopyForCleanHash.Lat.ToString("N6");


            string itemJson = JsonConvert.SerializeObject(this);
            Md5Checksum = GetHashString(itemJson);
        }

        public static byte[] GetHash(string inputString)
        {
            using (HashAlgorithm algorithm = SHA256.Create())
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            if(!String.IsNullOrEmpty(inputString))
            {
                foreach (byte b in GetHash(inputString))
                    sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }

        public void GenerateItemHash()
        {
            string hash = ComputeHash(ObjectToByteArray(this));
            Md5Checksum = hash;
        }

        private static readonly Object locker = new Object();

        private static byte[] ObjectToByteArray(Object objectToSerialize)
        {
            MemoryStream fs = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                //Here's the core functionality! One Line!
                //To be thread-safe we lock the object
                lock (locker)
                {
                    formatter.Serialize(fs, objectToSerialize);
                }
                return fs.ToArray();
            }
            catch (SerializationException se)
            {
                Console.WriteLine("Error occurred during serialization. Message: " +
                se.Message);
                return null;
            }
            finally
            {
                fs.Close();
            }
        }

        private static string ComputeHash(byte[] objectAsBytes)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            try
            {
                byte[] result = md5.ComputeHash(objectAsBytes);

                // Build the final string by converting each byte
                // into hex and appending it to a StringBuilder
                StringBuilder sb = new StringBuilder();
                if(objectAsBytes.Length > 0)
                {
                    for (int i = 0; i < result.Length; i++)
                    {
                        sb.Append(result[i].ToString("X2"));
                    }
                }

                // And return it
                return sb.ToString();
            }
            catch (ArgumentNullException ane)
            {
                //If something occurred during serialization, 
                //this method is called with a null argument. 
                Console.WriteLine("Hash has not been generated.");
                return null;
            }
        }

    }
}