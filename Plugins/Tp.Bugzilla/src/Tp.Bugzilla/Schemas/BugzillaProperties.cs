// Copyright 2004, Microsoft Corporation
// Sample Code - Use restricted to terms of use defined in the accompanying license agreement (EULA.doc)

//--------------------------------------------------------------
// Autogenerated by XSDObjectGen version 1.4.4.1
// Schema file: BugzillaProperties.xsd
// Creation Date: 9/16/2009 8:19:22 PM
//--------------------------------------------------------------

using System;
using System.Xml.Serialization;
using System.Collections;
using System.Xml.Schema;
using System.ComponentModel;

namespace Tp.Bugzilla.Schemas
{
	[Serializable]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public class nameCollection : ArrayList
	{
		public string Add(string obj)
		{
			base.Add(obj);
			return obj;
		}

		public void Insert(int index, string obj)
		{
			base.Insert(index, obj);
		}

		public void Remove(string obj)
		{
			base.Remove(obj);
		}

		new public string this[int index]
		{
			get { return (string) base[index]; }
			set { base[index] = value; }
		}
	}



	[XmlRoot(ElementName="bugzilla_properties",IsNullable=false),Serializable]
	public class bugzilla_properties
	{

		[XmlAttribute(AttributeName="version",DataType="string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __version;
		
		[XmlIgnore]
		public string version
		{ 
			get { return __version; }
			set { __version = value; }
		}

		[XmlElement(ElementName="script_version",IsNullable=false,Form=XmlSchemaForm.Qualified,DataType="string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __script_version;
		
		[XmlIgnore]
		public string script_version
		{ 
			get { return __script_version; }
			set { __script_version = value; }
		}

		[XmlElement(Type=typeof(custom_fields),ElementName="custom_fields",IsNullable=false,Form=XmlSchemaForm.Qualified)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public custom_fields __custom_fields;
		
		[XmlIgnore]
		public custom_fields custom_fields
		{
			get
			{
				if (__custom_fields == null) __custom_fields = new custom_fields();		
				return __custom_fields;
			}
			set {__custom_fields = value;}
		}

		[XmlElement(Type=typeof(severities),ElementName="severities",IsNullable=false,Form=XmlSchemaForm.Qualified)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public severities __severities;
		
		[XmlIgnore]
		public severities severities
		{
			get
			{
				if (__severities == null) __severities = new severities();		
				return __severities;
			}
			set {__severities = value;}
		}

		[XmlElement(Type=typeof(priorities),ElementName="priorities",IsNullable=false,Form=XmlSchemaForm.Qualified)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public priorities __priorities;
		
		[XmlIgnore]
		public priorities priorities
		{
			get
			{
				if (__priorities == null) __priorities = new priorities();		
				return __priorities;
			}
			set {__priorities = value;}
		}

		[XmlElement(Type=typeof(statuses),ElementName="statuses",IsNullable=false,Form=XmlSchemaForm.Qualified)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public statuses __statuses;
		
		[XmlIgnore]
		public statuses statuses
		{
			get
			{
				if (__statuses == null) __statuses = new statuses();		
				return __statuses;
			}
			set {__statuses = value;}
		}

		[XmlElement(Type=typeof(resolutions),ElementName="resolutions",IsNullable=false,Form=XmlSchemaForm.Qualified)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public resolutions __resolutions;
		
		[XmlIgnore]
		public resolutions resolutions
		{
			get
			{
				if (__resolutions == null) __resolutions = new resolutions();		
				return __resolutions;
			}
			set {__resolutions = value;}
		}

		[XmlElement(ElementName="timezone",IsNullable=false,Form=XmlSchemaForm.Qualified,DataType="string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string __timezone;
		
		[XmlIgnore]
		public string timezone
		{ 
			get { return __timezone; }
			set { __timezone = value; }
		}

		public bugzilla_properties()
		{
		}
	}


	[XmlRoot(ElementName="custom_fields",IsNullable=false),Serializable]
	public class custom_fields
	{
		[System.Runtime.InteropServices.DispIdAttribute(-4)]
		public IEnumerator GetEnumerator() 
		{
            return nameCollection.GetEnumerator();
		}

		public string Add(string obj)
		{
			return nameCollection.Add(obj);
		}

		[XmlIgnore]
		public string this[int index]
		{
			get { return (string) nameCollection[index]; }
		}

		[XmlIgnore]
        public int Count 
		{
            get { return nameCollection.Count; }
        }

        public void Clear()
		{
			nameCollection.Clear();
        }

		public string Remove(int index) 
		{ 
            string obj = nameCollection[index];
            nameCollection.Remove(obj);
			return obj;
        }

        public void Remove(object obj)
		{
            nameCollection.Remove(obj);
        }

		[XmlElement(Type=typeof(string),ElementName="name",IsNullable=false,Form=XmlSchemaForm.Qualified,DataType="string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public nameCollection __nameCollection;
		
		[XmlIgnore]
		public nameCollection nameCollection
		{
			get
			{
				if (__nameCollection == null) __nameCollection = new nameCollection();
				return __nameCollection;
			}
			set {__nameCollection = value;}
		}

		public custom_fields()
		{
		}
	}


	[XmlRoot(ElementName="severities",IsNullable=false),Serializable]
	public class severities
	{
		[System.Runtime.InteropServices.DispIdAttribute(-4)]
		public IEnumerator GetEnumerator() 
		{
            return nameCollection.GetEnumerator();
		}

		public string Add(string obj)
		{
			return nameCollection.Add(obj);
		}

		[XmlIgnore]
		public string this[int index]
		{
			get { return (string) nameCollection[index]; }
		}

		[XmlIgnore]
        public int Count 
		{
            get { return nameCollection.Count; }
        }

        public void Clear()
		{
			nameCollection.Clear();
        }

		public string Remove(int index) 
		{ 
            string obj = nameCollection[index];
            nameCollection.Remove(obj);
			return obj;
        }

        public void Remove(object obj)
		{
            nameCollection.Remove(obj);
        }

		[XmlElement(Type=typeof(string),ElementName="name",IsNullable=false,Form=XmlSchemaForm.Qualified,DataType="string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public nameCollection __nameCollection;
		
		[XmlIgnore]
		public nameCollection nameCollection
		{
			get
			{
				if (__nameCollection == null) __nameCollection = new nameCollection();
				return __nameCollection;
			}
			set {__nameCollection = value;}
		}

		public severities()
		{
		}
	}


	[XmlRoot(ElementName="priorities",IsNullable=false),Serializable]
	public class priorities
	{
		[System.Runtime.InteropServices.DispIdAttribute(-4)]
		public IEnumerator GetEnumerator() 
		{
            return nameCollection.GetEnumerator();
		}

		public string Add(string obj)
		{
			return nameCollection.Add(obj);
		}

		[XmlIgnore]
		public string this[int index]
		{
			get { return (string) nameCollection[index]; }
		}

		[XmlIgnore]
        public int Count 
		{
            get { return nameCollection.Count; }
        }

        public void Clear()
		{
			nameCollection.Clear();
        }

		public string Remove(int index) 
		{ 
            string obj = nameCollection[index];
            nameCollection.Remove(obj);
			return obj;
        }

        public void Remove(object obj)
		{
            nameCollection.Remove(obj);
        }

		[XmlElement(Type=typeof(string),ElementName="name",IsNullable=false,Form=XmlSchemaForm.Qualified,DataType="string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public nameCollection __nameCollection;
		
		[XmlIgnore]
		public nameCollection nameCollection
		{
			get
			{
				if (__nameCollection == null) __nameCollection = new nameCollection();
				return __nameCollection;
			}
			set {__nameCollection = value;}
		}

		public priorities()
		{
		}
	}


	[XmlRoot(ElementName="statuses",IsNullable=false),Serializable]
	public class statuses
	{
		[System.Runtime.InteropServices.DispIdAttribute(-4)]
		public IEnumerator GetEnumerator() 
		{
            return nameCollection.GetEnumerator();
		}

		public string Add(string obj)
		{
			return nameCollection.Add(obj);
		}

		[XmlIgnore]
		public string this[int index]
		{
			get { return (string) nameCollection[index]; }
		}

		[XmlIgnore]
        public int Count 
		{
            get { return nameCollection.Count; }
        }

        public void Clear()
		{
			nameCollection.Clear();
        }

		public string Remove(int index) 
		{ 
            string obj = nameCollection[index];
            nameCollection.Remove(obj);
			return obj;
        }

        public void Remove(object obj)
		{
            nameCollection.Remove(obj);
        }

		[XmlElement(Type=typeof(string),ElementName="name",IsNullable=false,Form=XmlSchemaForm.Qualified,DataType="string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public nameCollection __nameCollection;
		
		[XmlIgnore]
		public nameCollection nameCollection
		{
			get
			{
				if (__nameCollection == null) __nameCollection = new nameCollection();
				return __nameCollection;
			}
			set {__nameCollection = value;}
		}

		public statuses()
		{
		}
	}


	[XmlRoot(ElementName="resolutions",IsNullable=false),Serializable]
	public class resolutions
	{
		[System.Runtime.InteropServices.DispIdAttribute(-4)]
		public IEnumerator GetEnumerator() 
		{
            return nameCollection.GetEnumerator();
		}

		public string Add(string obj)
		{
			return nameCollection.Add(obj);
		}

		[XmlIgnore]
		public string this[int index]
		{
			get { return (string) nameCollection[index]; }
		}

		[XmlIgnore]
        public int Count 
		{
            get { return nameCollection.Count; }
        }

        public void Clear()
		{
			nameCollection.Clear();
        }

		public string Remove(int index) 
		{ 
            string obj = nameCollection[index];
            nameCollection.Remove(obj);
			return obj;
        }

        public void Remove(object obj)
		{
            nameCollection.Remove(obj);
        }

		[XmlElement(Type=typeof(string),ElementName="name",IsNullable=false,Form=XmlSchemaForm.Qualified,DataType="string")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public nameCollection __nameCollection;
		
		[XmlIgnore]
		public nameCollection nameCollection
		{
			get
			{
				if (__nameCollection == null) __nameCollection = new nameCollection();
				return __nameCollection;
			}
			set {__nameCollection = value;}
		}

		public resolutions()
		{
		}
	}
}
