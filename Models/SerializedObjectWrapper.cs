using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BankomatServer.Models
{
    public class SerializedObjectWrapper : IXmlSerializable
    {
        /// <summary>
        /// The underlying Object reference that is being returned
        /// </summary>
        public object Object { get; set; }

        /// <summary>
        /// This is used because creating XmlSerializers are expensive
        /// </summary>
        private static readonly ConcurrentDictionary<Type, XmlSerializer> TypeSerializers
            = new ConcurrentDictionary<Type, XmlSerializer>();

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();

            //Get the Item type attribute
            var itemType = reader.GetAttribute("ItemType");
            if (itemType == null) throw new InvalidOperationException("ItemType attribute cannot be null");

            //Ensure the type is found in the app domain
            var itemTypeType = Type.GetType(itemType);
            if (itemTypeType == null) throw new InvalidOperationException("Could not find the type " + itemType);

            var isEmptyElement = reader.IsEmptyElement;

            reader.ReadStartElement();

            if (isEmptyElement == false)
            {
                var serializer = TypeSerializers.GetOrAdd(itemTypeType, t => new XmlSerializer(t));
                Object = serializer.Deserialize(reader);
                reader.ReadEndElement();
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            //var itemType = Object.GetType();
            //var serializer = TypeSerializers.GetOrAdd(itemType, t => new XmlSerializer(t));

            ////writes the object type so we can use that to deserialize later
            //writer.WriteAttributeString("ItemType",
            //    itemType.AssemblyQualifiedName ?? Object.GetType().ToString());

            //serializer.Serialize(writer, Object);


            Type t = Object.GetType();

            Type[] extraTypes = t.GetProperties()
                .Where(p => p.PropertyType.IsInterface)
                .Select(p => p.GetValue(Object, null).GetType())
                .ToArray();

            DataContractSerializer serializer = new DataContractSerializer(t, extraTypes);
            StringWriter sw = new StringWriter();
            XmlTextWriter xw = new XmlTextWriter(sw);
            serializer.WriteObject(xw, Object);

            //if (Object == null)
            //{
            //    writer.WriteAttributeString("type", "null");
            //    return;
            //}
            //Type type = this.Object.GetType();
            //XmlSerializer serializer = new XmlSerializer(type);
            //writer.WriteAttributeString("type", type.AssemblyQualifiedName);
            //serializer.Serialize(writer, this.Object);
        }
    }
}