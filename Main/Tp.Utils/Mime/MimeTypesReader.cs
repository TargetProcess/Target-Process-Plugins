using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace Tp.Utils.Mime
{
    /// <summary>
    /// Summary description for MimeTypesReader.
    /// </summary>
    public sealed class MimeTypesReader
    {
        public static string GetMimeTypeString(string filePath)
        {
            var mimeTypes = new MimeTypes();

            MimeType mimeType;

            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                mimeType = mimeTypes.GetMimeTypeByContent(fileStream);
            }


            if (mimeType == null)
            {
                mimeType = mimeTypes.GetMimeType(filePath);
            }

            if (mimeType == null)
            {
                return null;
            }

            return mimeType.Name;
        }

        internal MimeType[] Read(String filepath)
        {
            var document = new XmlDocument();
            document.Load(filepath);
            return Visit(document);
        }

        /// <summary>Scan through the document. </summary>
        public MimeType[] Visit(XmlDocument document)
        {
            MimeType[] types = null;
            var element = document.DocumentElement;
            if ((element != null) && element.Name.Equals("mime-types"))
            {
                types = ReadMimeTypes(element);
            }
            return types ?? new MimeType[0];
        }

        private MimeType[] ReadMimeTypes(XmlElement element)
        {
            ArrayList types = new ArrayList();
            XmlNodeList nodes = element.ChildNodes;
            for (int i = 0; i < nodes.Count; i++)
            {
                XmlNode node = nodes.Item(i);
                if (Convert.ToInt16(node.NodeType) == (short) XmlNodeType.Element)
                {
                    XmlElement nodeElement = (XmlElement) node;
                    if (nodeElement.Name.Equals("mime-type"))
                    {
                        MimeType type = ReadMimeType(nodeElement);
                        if (type != null)
                        {
                            types.Add(type);
                        }
                    }
                }
            }
            return (MimeType[]) SupportUtil.ToArray(types, new MimeType[types.Count]);
        }

        /// <summary>Read Element named mime-type. </summary>
        private MimeType ReadMimeType(XmlElement element)
        {
            String name = null;
            String description = null;
            MimeType type;
            XmlNamedNodeMap attrs = element.Attributes;
            for (int i = 0; i < attrs.Count; i++)
            {
                XmlAttribute attr = (XmlAttribute) attrs.Item(i);
                if (attr.Name.Equals("name"))
                {
                    name = attr.Value;
                }
                else if (attr.Name.Equals("description"))
                {
                    description = attr.Value;
                }
            }
            if ((name == null) || (name.Trim().Equals("")))
            {
                return null;
            }

            try
            {
                //Trace.WriteLine("Mime type:" + name);
                type = new MimeType(name);
            }
            catch (MimeTypeException mte)
            {
                // Mime Type not valid... just ignore it
                Trace.WriteLine(mte.ToString() + " ... Ignoring!");
                return null;
            }

            type.Description = description;

            XmlNodeList nodes = element.ChildNodes;
            for (int i = 0; i < nodes.Count; i++)
            {
                XmlNode node = nodes.Item(i);
                if (Convert.ToInt16(node.NodeType) == (short) XmlNodeType.Element)
                {
                    XmlElement nodeElement = (XmlElement) node;
                    if (nodeElement.Name.Equals("ext"))
                    {
                        ReadExt(nodeElement, type);
                    }
                    else if (nodeElement.Name.Equals("magic"))
                    {
                        ReadMagic(nodeElement, type);
                    }
                }
            }
            return type;
        }

        /// <summary>Read Element named ext. </summary>
        private void ReadExt(XmlElement element, MimeType type)
        {
            XmlNodeList nodes = element.ChildNodes;
            for (int i = 0; i < nodes.Count; i++)
            {
                XmlNode node = nodes.Item(i);
                if (Convert.ToInt16(node.NodeType) == (short) XmlNodeType.Text)
                {
                    type.AddExtension(((XmlText) node).Data);
                }
            }
        }

        /// <summary>Read Element named magic. </summary>
        private void ReadMagic(XmlElement element, MimeType mimeType)
        {
            // element.getValue();
            String offset = null;
            String content = null;
            String type = null;
            var attrs = element.Attributes;
            for (int i = 0; i < attrs.Count; i++)
            {
                XmlAttribute attr = (XmlAttribute) attrs.Item(i);
                if (attr.Name.Equals("offset"))
                {
                    offset = attr.Value;
                }
                else if (attr.Name.Equals("type"))
                {
                    type = attr.Value;
                    if (String.Compare(type, "byte", true) == 0)
                    {
                        type = "System.Byte";
                    }
                }
                else if (attr.Name.Equals("value"))
                {
                    content = attr.Value;
                }
            }
            if ((offset != null) && (content != null))
            {
                mimeType.AddMagic(Int32.Parse(offset), type, content);
            }
        }
    }
}
