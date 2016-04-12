using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml;

namespace Tp.Integration.Messages.ServiceBus.Serialization
{
	/// <summary>
	///     Serializes arbitrary objects to XML.
	/// </summary>
	public class XmlSerializer : IDisposable
	{
		private static readonly ConcurrentDictionary<Type, string> _typesToAssemblyNamesCache = new ConcurrentDictionary<Type, string>();

		private static readonly ConcurrentDictionary<PropertyInfo, Func<object, object>> _propertyGettersCache =
			new ConcurrentDictionary<PropertyInfo, Func<object, object>>();

		private static readonly ConcurrentDictionary<Type, Type> _binaryCtorTypesCache = new ConcurrentDictionary<Type, Type>();

		#region Members

		private IXmlSerializationTag taglib = new XmlSerializationTag();

		// All serialized objects are registered here
		private readonly List<object> objlist = new List<object>();

		// If a type dictionary is used, store the Types here
		private readonly Hashtable typedictionary = new Hashtable();
		// Usage of a type dictionary is optional, but it's advised
		private bool usetypedictionary = true;

		public const string TYPE_KEY_PREFIX = "TK";

		private Type serializationIgnoredAttributeType;

		#endregion Members

		#region Properties

		/// <summary>
		///     Gets or sets the attribute that, when applied to a property enable its serialization. If null every property is
		///     serialized.
		///     Even "Type" does not specialize the kind of Type it is obvious that only Attributes can be applied to properties.
		/// </summary>
		[Description("Gets or sets Attribute Type which marks a property to be ignored.")]
		public Type SerializationIgnoredAttributeType
		{
			get { return serializationIgnoredAttributeType; }
			set { serializationIgnoredAttributeType = value; }
		}

		/// <summary>
		///     Gets or sets whether errors during serialisation shall be ignored.
		/// </summary>
		[Description("Gets or sets whether errors during serialisation shall be ignored.")]
		public bool IgnoreSerialisationErrors { get; set; }

		/// <summary>
		///     Gets or sets the dictionary of XML-tags.
		/// </summary>
		[Description("Gets or sets the dictionary of XML-tags.")]
		public IXmlSerializationTag TagLib
		{
			get { return taglib; }
			set { taglib = value; }
		}

		/// <summary>
		///     Gets or sets whether a type dictionary is used to store Type information.
		/// </summary>
		[Description("Gets or sets whether a type dictionary is used to store Type information.")]
		public bool UseTypeDictionary
		{
			get { return usetypedictionary; }
			set { usetypedictionary = value; }
		}

		/// <summary>
		///     Gets or sets whether the ISerializable Attribute is ignored.
		/// </summary>
		/// <remarks>
		///     Set this property only to true if you know about side effects.
		/// </remarks>
		[Description("Gets or sets whether the ISerializable Attribute is ignored.")]
		public bool IgnoreSerializableAttribute { get; set; }

		#endregion Properties

		#region Serialize

		/// <summary>
		///     Serializes an Object to a file.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="filename"></param>
		/// <returns></returns>
		public void Serialize(object obj, string filename)
		{
			XmlDocument doc = Serialize(obj);
			doc.Save(filename);
		}

		/// <summary>
		///     Serializes an Object to a new XmlDocument.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public XmlDocument Serialize(object obj)
		{
			var doc = new XmlDocument();
			XmlDeclaration xd = doc.CreateXmlDeclaration("1.0", "utf-8", null);
			doc.AppendChild(xd);

			Serialize(obj, null, doc);

			return doc;
		}

		/// <summary>
		///     Serializes an Object and appends it to root (DocumentElement) of the specified XmlDocument.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="name"></param>
		/// <param name="doc"></param>
		public void Serialize(object obj, String name, XmlDocument doc)
		{
			// Reset();

			XmlElement root = doc.CreateElement(taglib.OBJECT_TAG);

			XmlComment comment = root.OwnerDocument.CreateComment(" Data section : Don't edit any attributes ! ");
			root.AppendChild(comment);

			SetObjectInfoAttributes(name, obj.GetType(), root);

			if (doc.DocumentElement == null)
				doc.AppendChild(root);
			else
				doc.DocumentElement.AppendChild(root);

			Type ctortype = GetBinaryConstructorType(obj.GetType());

			if (ctortype != null)
			{
				SerializeBinaryObject(obj, ctortype, root);
			}
			else
			{
				SerializeProperties(obj, root);
			}

			WriteTypeDictionary(root);
		}

		/// <summary>
		///     Serializes an Object and appends it to the specified XmlNode.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="name"></param>
		/// <param name="parent"></param>
		public void Serialize(object obj, String name, XmlNode parent)
		{
			//Reset();

			XmlDocument doc = parent.OwnerDocument;

			XmlElement root = doc.CreateElement(taglib.OBJECT_TAG);
			parent.AppendChild(root);

			XmlComment comment = root.OwnerDocument.CreateComment(" Data section : Don't edit any attributes ! ");
			root.AppendChild(comment);

			SetObjectInfoAttributes(name, obj.GetType(), root);

			Type ctortype = GetBinaryConstructorType(obj.GetType());

			if (ctortype != null)
			{
				SerializeBinaryObject(obj, ctortype, root);
			}
			else
			{
				SerializeProperties(obj, root);
			}

			WriteTypeDictionary(root);
		}

		#endregion Serialize

		#region ObjectInfo

		/// <summary>
		///     Returns an ObjectInfo filled with the values of Name, Type, and Assembly.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		private ObjectInfo GetObjectInfo(string name, Type type)
		{
			return new ObjectInfo
			{
				Name = name,
				Type = type.FullName,
				Assembly = _typesToAssemblyNamesCache.GetOrAdd(type, tpe => tpe.Assembly.GetName().Name)
			};
		}

		/// <summary>
		///     Sets the property attributes of a Property to an XmlNode.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="type"></param>
		/// <param name="node"></param>
		private void SetObjectInfoAttributes(String propertyName, Type type, XmlNode node)
		{
			var objinfo = new ObjectInfo();

			GetObjectInfo(propertyName, type);

			if (type != null)
			{
				objinfo = GetObjectInfo(propertyName, type);
			}

			// Use of a TypeDictionary?
			if (usetypedictionary)
			{
				// TypeDictionary
				String typekey = GetTypeKey(type);

				XmlAttribute att = node.OwnerDocument.CreateAttribute(taglib.NAME_TAG);
				att.Value = objinfo.Name;
				node.Attributes.Append(att);

				att = node.OwnerDocument.CreateAttribute(taglib.TYPE_TAG);
				att.Value = typekey;
				node.Attributes.Append(att);

				// The assembly will be set, also, but it's always empty.
				att = node.OwnerDocument.CreateAttribute(taglib.ASSEMBLY_TAG);
				att.Value = "";
				node.Attributes.Append(att);
			}
			else
			{
				// No TypeDictionary
				XmlAttribute att = node.OwnerDocument.CreateAttribute(taglib.NAME_TAG);
				att.Value = objinfo.Name;
				node.Attributes.Append(att);

				att = node.OwnerDocument.CreateAttribute(taglib.TYPE_TAG);
				att.Value = objinfo.Type;
				node.Attributes.Append(att);

				att = node.OwnerDocument.CreateAttribute(taglib.ASSEMBLY_TAG);
				att.Value = objinfo.Assembly;
				node.Attributes.Append(att);
			}
		}

		#endregion ObjectInfo

		#region Properties

		/// <summary>
		///     Returns wether the Property has to be serialized or not (depending on SerializationIgnoredAttributeType).
		/// </summary>
		/// <param name="pi"></param>
		/// <returns></returns>
		protected bool CheckPropertyHasToBeSerialized(PropertyInfo pi)
		{
			if (serializationIgnoredAttributeType != null)
			{
				return pi.GetCustomAttributes(serializationIgnoredAttributeType, true).Length == 0;
			}
			return true;
		}

		/// <summary>
		///     Serializes the properties an Object and appends them to the specified XmlNode.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="parent"></param>
		protected void SerializeProperties(object obj, XmlNode parent)
		{
			if (TypeInfo.IsCollection(obj.GetType()))
			{
				SetCollectionItems(obj, (ICollection) obj, parent);
			}
			else
			{
				XmlElement node = parent.OwnerDocument.CreateElement(taglib.PROPERTIES_TAG);

				SetProperties(obj, node);

				parent.AppendChild(node);
			}
			objlist.Remove(obj);
		}

		protected void SetProperties(object obj, XmlElement node)
		{
			PropertyInfo[] piarr = obj.GetType().GetProperties();
			foreach (PropertyInfo pi in piarr)
			{
				SetProperty(obj, pi, node);
			}
		}

		/// <summary>
		///     Sets a property.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="pi"></param>
		/// <param name="parent"></param>
		protected void SetProperty(object obj, PropertyInfo pi, XmlNode parent)
		{
			objlist.Add(obj);

			var getter = GetGetter(pi);

			var val = getter(obj);

			// If the value there's nothing to do
			if (val == null)
				return;

			// If the the value already exists in the list of processed objects/properties
			// ignore it o avoid circular calls.

			if (objlist.FirstOrDefault(x => x == val) != null)
				return;

			SetProperty(obj, val, pi, parent);

			objlist.Remove(val);
		}

		public delegate object GenericGetter(object target);

		private static Func<object, object> GetGetter(PropertyInfo propertyInfo)
		{
			return _propertyGettersCache.GetOrAdd(propertyInfo, CreateGetter);
		}

		/// Creates a dynamic getter for the property
		private static Func<object, object> CreateGetter(PropertyInfo propertyInfo)
		{
			/*
			* If there's no getter return null
			*/
			var param = Expression.Parameter(typeof(object), "e");
			var convertedParam = Expression.Convert(param, propertyInfo.DeclaringType);
			var prop = Expression.Property(convertedParam, propertyInfo);
			var convertedPropValue = Expression.Convert(prop, typeof(object));
			var lambda = Expression.Lambda<Func<object, object>>(convertedPropValue, param);

			return lambda.Compile();
		}

		private static Type GetBinaryConstructorType(Type tpe)
		{
			return _binaryCtorTypesCache.GetOrAdd(tpe, TypeInfo.GetBinaryConstructorType);
		}

		/// <summary>
		///     Sets a property.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="value"></param>
		/// <param name="pi"></param>
		/// <param name="parent"></param>
		/// <remarks>
		///     This is the central method which is called recursivly!
		/// </remarks>
		protected void SetProperty(object obj, object value, PropertyInfo pi, XmlNode parent)
		{
			//      object val = value; ??

			try
			{
				// Empty values are ignored (no need to restore null references or empty Strings)
				if (value == null || value.Equals(string.Empty))
					return;

				// Get the Type
				//Type pt = pi.PropertyType;
				Type pt = value.GetType();

				// Check whether this property can be serialized and deserialized
				if (CheckPropertyHasToBeSerialized(pi) && (pt.IsSerializable || IgnoreSerializableAttribute) && (pi.CanWrite) &&
					((pt.IsPublic) || (pt.IsEnum)))
				{
					XmlElement prop = parent.OwnerDocument.CreateElement(taglib.PROPERTY_TAG);

					SetObjectInfoAttributes(pi.Name, pt, prop);

					// Try to find a constructor for binary data.
					// If found remember the parameter's Type.

					Type binctortype = GetBinaryConstructorType(pt);

					if (binctortype != null) // a binary contructor was found
					{
						/*
						 * b. Trying to handle binary data
						 */

						var getter = GetGetter(pi);

						SerializeBinaryObject(getter(obj), binctortype, prop);
					}
					else if (TypeInfo.IsCollection(pt))
					{
						/*
						 * a. Collections ask for a specific handling
						 */
						SetCollectionItems(obj, (ICollection) value, prop);
					}
					else
					{
						/*
						 * c. "normal" classes
						 */

						SetXmlElementFromBasicPropertyValue(prop, pt, value, parent);
					}
					// Append the property node to the paren XmlNode
					parent.AppendChild(prop);
				}
			}
			catch (Exception)
			{
				if (!IgnoreSerialisationErrors)
				{
					throw;
				}
			}
		}

		protected void SetXmlElementFromBasicPropertyValue(XmlElement prop, Type pt, object value, XmlNode parent)
		{
			// If possible, convert this property to a string
			if (pt.IsAssignableFrom(typeof(string)))
			{
				prop.InnerText = value.ToString();
				return;
			}

			TypeConverter tc = TypeDescriptor.GetConverter(pt);
			if (tc.CanConvertFrom(typeof(string)) && tc.CanConvertTo(typeof(string)))
			{
				prop.InnerText = tc.ConvertToInvariantString(value);
				return;
			}

			bool complexclass = false;
			// Holds whether the propertys type is an complex type (the properties of objects have to be iterated, either)

			// Get all properties
			PropertyInfo[] piarr2 = pt.GetProperties();
			XmlElement proplist = null;
			//Debug.Assert(piarr2.Length > 0, "No property found to serialize for type " + pt.Name + "! Current implementation of Ellisys.Util.Serialization.XmlSerializer only work on public properties with get and set");
			// Loop all properties
			for (int j = 0; j < piarr2.Length; j++)
			{
				PropertyInfo pi2 = piarr2[j];
				// Check whether this property can be serialized and deserialized
				if (CheckPropertyHasToBeSerialized(pi2) && (pi2.PropertyType.IsSerializable || IgnoreSerializableAttribute) &&
					(pi2.CanWrite) && ((pi2.PropertyType.IsPublic) || (pi2.PropertyType.IsEnum)))
				{
					// Seems to be a complex type
					complexclass = true;

					// Add a properties parent node
					if (proplist == null)
					{
						proplist = parent.OwnerDocument.CreateElement(taglib.PROPERTIES_TAG);
						prop.AppendChild(proplist);
					}

					// Set the property (recursive call of this method!)
					SetProperty(value, pi2, proplist);
				}
			}

			// Ok, that was not a complex class either
			if (!complexclass)
			{
				// Converting to string was not possible, just set the value by ToString()
				prop.InnerText = value.ToString();
			}
		}

		/// <summary>
		///     Serializes binary data to a XmlNode.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="ctorParamType"></param>
		/// <param name="parent"></param>
		protected void SerializeBinaryObject(Object obj, Type ctorParamType, XmlNode parent)
		{
			XmlElement proplist = null;
			String val = null;

			try
			{
				// If the objact is a Stream or can be converted to a byte[]...
				TypeConverter tc = TypeDescriptor.GetConverter(obj.GetType());
				if (tc.CanConvertTo(typeof(byte[])) || typeof(Stream).IsAssignableFrom(obj.GetType()))
				{
					byte[] barr = null;

					// Convert to byte[]
					if (typeof(Stream).IsAssignableFrom(obj.GetType()))
					{
						// Convert a Stream to byte[]
						var bctc = new BinaryContainerTypeConverter();
						barr = bctc.ConvertStreamToByteArray((Stream) obj);
					}
					else
					{
						// Convert the object to a byte[]
						barr = (byte[]) tc.ConvertTo(obj, typeof(byte[]));
					}

					// Create a constructor node
					proplist = parent.OwnerDocument.CreateElement(taglib.CONSTRUCTOR_TAG);
					parent.AppendChild(proplist);

					// Set info about the constructor type as attributes  
					SetObjectInfoAttributes("0", ctorParamType, proplist);

					// Create a node for the binary data
					XmlNode bindata = proplist.OwnerDocument.CreateElement(taglib.BINARY_DATA_TAG);
					proplist.AppendChild(bindata);

					// Set info about the binary data type as attributes (currently it's always byte[]) 
					SetObjectInfoAttributes("0", typeof(byte[]), bindata);

					// Convert the byte array to a string so it's easy to store it in XML
					val = Convert.ToBase64String(barr, 0, barr.Length);

					bindata.InnerText = val;
				}
			}
			catch (Exception exc)
			{
				if (!IgnoreSerialisationErrors)
				{
					throw exc;
				}
			}
		}

		#endregion Properties

		#region SetCollectionItems

		/// <summary>
		///     Sets the items on a collection.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="value"></param>
		/// <param name="parent"></param>
		/// <remarks>
		///     This method could be simplified since it's mainly the same code you can find in SetProperty()
		/// </remarks>
		protected void SetCollectionItems(object obj, ICollection value, XmlNode parent)
		{
			// Validating the parameters
			if (obj == null || value == null || parent == null)
				return;

			try
			{
				ICollection coll = value;

				XmlElement collnode = parent.OwnerDocument.CreateElement(taglib.ITEMS_TAG);
				parent.AppendChild(collnode);

				int cnt = 0;

				// What kind of Collection?
				if (TypeInfo.IsDictionary(coll.GetType()))
				{
					// IDictionary
					var dict = (IDictionary) coll;
					IDictionaryEnumerator de = dict.GetEnumerator();
					while (de.MoveNext())
					{
						object key = de.Key;
						object val = de.Value;

						XmlElement itemnode = parent.OwnerDocument.CreateElement(taglib.ITEM_TAG);
						collnode.AppendChild(itemnode);

						object curr = de.Current;

						XmlElement propsnode = parent.OwnerDocument.CreateElement(taglib.PROPERTIES_TAG);
						itemnode.AppendChild(propsnode);

						SetProperties(curr, propsnode);
					}
				}
				else
				{
					// Everything else
					IEnumerator ie = coll.GetEnumerator();
					while (ie.MoveNext())
					{
						object obj2 = ie.Current;

						XmlElement itemnode = parent.OwnerDocument.CreateElement(taglib.ITEM_TAG);

						if (obj2 != null)
						{
							SetObjectInfoAttributes(null, obj2.GetType(), itemnode);
						}
						else
						{
							SetObjectInfoAttributes(null, null, itemnode);
						}

						itemnode.Attributes[taglib.NAME_TAG].Value = "" + cnt;

						cnt++;

						collnode.AppendChild(itemnode);

						if (obj2 == null)
							continue;

						Type pt = obj2.GetType();

						if (TypeInfo.IsCollection(pt))
						{
							SetCollectionItems(obj, (ICollection) obj2, itemnode);
						}
						else
						{
							SetXmlElementFromBasicPropertyValue(itemnode, pt, obj2, parent);
						} // IsCollection?
					} // Loop collection
				} // IsDictionary?
			}
			catch (Exception exc)
			{
				if (!IgnoreSerialisationErrors)
				{
					throw exc;
				}
				else
				{
					// perhaps logging
				}
			}
		}

		#endregion SetCollectionItems

		#region Misc

		/// <summary>
		///     Builds the Hashtable that will be written to XML as the type dictionary.
		///     TODO: Why Hashtable? Better use a typesafe generic Dictionary. Maybe filesize can be decreased.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		///     While serialization the key of the type dictionary is the Type so it's easy to determine
		///     whether a Type is registered already. For deserialization the order is reverse: find a Type
		///     for a given key.
		///     This methods creates a reversed Hashtable with the Types information stored in TypeInfo instances.
		/// </remarks>
		protected Hashtable BuildSerializeableTypeDictionary()
		{
			var ht = new Hashtable();

			IDictionaryEnumerator de = typedictionary.GetEnumerator();
			while (de.MoveNext())
			{
				var type = (Type) de.Entry.Key;
				var key = (String) de.Value;

				var ti = new TypeInfo(type);
				ht.Add(key, ti);
			}
			return ht;
		}

		/// <summary>
		///     Gets the key of a Type from the type dictionary.
		///     If the Type is not registered, yet, it will be registered here.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>If the given Object is null, null, otherwise the Key of the Objects Type.</returns>
		protected string GetTypeKey(object obj)
		{
			if (obj == null)
				return null;

			return GetTypeKey(obj.GetType());
		}

		/// <summary>
		///     Gets the key of a Type from the type dictionary.
		///     If the Type is not registered, yet, it will be registered here.
		/// </summary>
		/// <param name="type"></param>
		/// <returns>If the given Type is null, null, otherwise the Key of the Type.</returns>
		protected string GetTypeKey(Type type)
		{
			if (type == null)
				return null;

			if (!typedictionary.ContainsKey(type))
			{
				typedictionary.Add(type, TYPE_KEY_PREFIX + typedictionary.Count);
			}

			return (String) typedictionary[type];
		}

		/// <summary>
		///     Writes the TypeDictionary to XML.
		/// </summary>
		/// <param name="parentNode"></param>
		protected void WriteTypeDictionary(XmlNode parentNode)
		{
			bool usedict = UseTypeDictionary;

			try
			{
				if (UseTypeDictionary)
				{
					XmlComment comment =
						parentNode.OwnerDocument.CreateComment(" TypeDictionary : Don't edit anything in this section at all ! ");
					parentNode.AppendChild(comment);

					XmlElement dictelem = parentNode.OwnerDocument.CreateElement(taglib.TYPE_DICTIONARY_TAG);
					parentNode.AppendChild(dictelem);
					Hashtable dict = BuildSerializeableTypeDictionary();

					// Temporary set UseTypeDictionary to false, otherwise TypeKeys instead of the
					// Type information will  be written
					UseTypeDictionary = false;

					SetObjectInfoAttributes(null, dict.GetType(), dictelem);
					SerializeProperties(dict, dictelem);

					// Reset UseTypeDictionary
					UseTypeDictionary = true;
				}
			}
			catch (Exception e)
			{
				throw e;
			}
			finally
			{
				UseTypeDictionary = usedict;
			}
		}

		/// <summary>
		///     Clears the Collections.
		/// </summary>
		public void Reset()
		{
			if (objlist != null)
				objlist.Clear();

			if (typedictionary != null)
				typedictionary.Clear();
		}

		/// <summary>
		///     Dispose, release references.
		/// </summary>
		public void Dispose()
		{
			Reset();
		}

		#endregion Misc
	}
}
