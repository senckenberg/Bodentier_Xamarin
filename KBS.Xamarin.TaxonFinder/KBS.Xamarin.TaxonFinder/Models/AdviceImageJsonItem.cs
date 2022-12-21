using System;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace KBS.App.TaxonFinder.Models
{
    [Serializable]
    public class AdviceImageJsonItem : ISerializable
    {
        [DataMember]
        public string ImageName { get; set; }

        [DataMember]
        public string ImageBase64 { get; set; }

        public AdviceImageJsonItem(string imageBase, string imageCompleteName)
        {
            ImageBase64 = imageBase;
            ImageName = Path.GetFileName(imageCompleteName);
        }

        public AdviceImageJsonItem() { }
        protected AdviceImageJsonItem(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new System.ArgumentNullException("info");
            }
            ImageBase64 = (string)info.GetValue("ImageBase64", typeof(string));
            ImageName = (string)info.GetValue("ImageName", typeof(string));
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ImageBase64", ImageBase64);
            info.AddValue("ImageName", ImageName);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            GetObjectData(info, context);
        }
    }

}
