using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Xml;

namespace Tp.Utils.Mime
{
	/// <summary>
	/// Summary description for MimeTypes.
	/// </summary>
	public sealed class MimeTypes
	{
		#region Class Members

		/// <summary>The default <code>application/octet-stream</code> MimeType </summary>
		public const string DEFAULT = MediaTypeNames.Application.Octet;

		/// <summary>All the registered MimeTypes </summary>
		private readonly ArrayList _types = new ArrayList();

		/// <summary>All the registered MimeType indexed by name </summary>
		private readonly Hashtable _typesIdx = new Hashtable();

		/// <summary>MimeTypes indexed on the file extension </summary>
		private readonly IDictionary _extIdx = new Hashtable();

		/// <summary>List of MimeTypes containing a magic char sequence </summary>
		private readonly List<MimeType> _mimeTypes = new List<MimeType>();

		/// <summary>The minimum length of data to provide to check all MimeTypes </summary>
		private int _minLength;

		/// <summary> My registered instances
		/// There is one instance associated for each specified file while
		/// calling the {@link #get(String)} method.
		/// Key is the specified file path in the {@link #get(String)} method.
		/// Value is the associated MimeType instance.
		/// </summary>
		private static readonly IDictionary Instances = new Hashtable();

		#endregion

		/// <summary>Should never be instanciated from outside </summary>
		public MimeTypes(string strFilepath)
		{
			var reader = new MimeTypesReader();
			Add(reader.Read(strFilepath));
		}

		public MimeTypes()
		{
			var reader = new MimeTypesReader();

			Assembly assembly = Assembly.GetExecutingAssembly();

			using (var stream = assembly.GetManifestResourceStream("Tp.Utils.Mime.mime-types.xml"))
			{
				if (stream == null)
				{
					throw new InvalidOperationException("Could not read resource");
				}
				var document = new XmlDocument();
				document.Load(stream);
				Add(reader.Visit(document));
			}
		}

		/// <summary> Return the minimum length of data to provide to analyzing methods
		/// based on the document's content in order to check all the known
		/// MimeTypes.
		/// </summary>
		/// <returns> the minimum length of data to provide.
		/// </returns>
		public int MinLength => _minLength;

		/// <summary> Return a MimeTypes instance.</summary>
		/// <param name="filepath">is the mime-types definitions xml file.
		/// </param>
		/// <returns> A MimeTypes instance for the specified filepath xml file.
		/// </returns>
		public static MimeTypes Get(string filepath)
		{
			MimeTypes instance;
			lock (Instances.SyncRoot)
			{
				instance = (MimeTypes) Instances[filepath];
				if (instance == null)
				{
					//instance = new MimeTypes(filepath, null);
					instance = new MimeTypes(filepath);
					Instances[filepath] = instance;
				}
			}
			return instance;
		}

		/// <summary> Find the Mime Content Type of a document from its URL.</summary>
		/// <param name="url">of the document to analyze.
		/// </param>
		/// <returns> the Mime Content Type of the specified document URL, or
		/// <code>null</code> if none is found.
		/// </returns>
		public MimeType GetMimeType(Uri url)
		{
			return GetMimeType(url.AbsolutePath);
		}

		/// <summary> Find the Mime Content Type of a document from its name.</summary>
		/// <param name="name">of the document to analyze.
		/// </param>
		/// <returns> the Mime Content Type of the specified document name, or
		/// <code>null</code> if none is found.
		/// </returns>
		public MimeType GetMimeType(string name)
		{
			MimeType[] founds = GetMimeTypes(name);
			if ((founds == null) || (founds.Length < 1))
			{
				// No mapping found, just return null
				return null;
			}
			// Arbitraly returns the first mapping
			return founds[0];
		}

		/// <summary> Find the Mime Content Type of a stream from its content.
		/// 
		/// </summary>
		/// <param name="data">are the first bytes of data of the content to analyze.
		/// Depending on the length of provided data, all known MimeTypes are
		/// checked. If the length of provided data is greater or egals to
		/// the value returned by {@link #getMinLength()}, then all known
		/// MimeTypes are checked, otherwise only the MimeTypes that could be
		/// analyzed with the length of provided data are analyzed.
		/// 
		/// </param>
		/// <returns> The Mime Content Type found for the specified data, or
		/// <code>null</code> if none is found.
		/// </returns>
		public MimeType GetMimeTypeByContent(Stream data)
		{
			// Preliminary checks
			if (data == null || (data.Length < 1))
				return null;

			var minLength = Math.Min(_mimeTypes.Max(x => x.MinLength), data.Length);

			var array = new byte[minLength];
			data.Read(array, 0, (int) minLength);

			// This is a very naive first approach (scanning all the magic
			//       bytes since one is matching.
			//       A first improvement could be to use a search path on the magic
			//       bytes.
			// A second improvement could be to search for the most qualified
			//       (the longuest) magic sequence (not the first that is matching).
			return _mimeTypes.FirstOrDefault(mimeType => mimeType.Matches(array));
		}

		public MimeType GetMimeTypeByExtension(string name)
		{
			MimeType[] mimeTypes = name != null ? GetMimeTypes(name) : null;
			var mimeType = mimeTypes?[0];
			return mimeType;
		}

		/// <summary> Return a MimeType from its name.</summary>
		public MimeType ForName(String name)
		{
			return (MimeType) _typesIdx[name];
		}

		/// <summary> Add the specified mime-types in the repository.</summary>
		/// <param name="types">are the mime-types to add.
		/// </param>
		internal void Add(MimeType[] types)
		{
			if (types == null)
			{
				return;
			}
			foreach (MimeType t in types)
			{
				Add(t);
			}
		}

		/// <summary> Add the specified mime-type in the repository.</summary>
		/// <param name="type">is the mime-type to add.
		/// </param>
		internal void Add(MimeType type)
		{
			_typesIdx[type.Name] = type;
			_types.Add(type);
			// Update minLentgth
			_minLength = Math.Max(_minLength, type.MinLength);
			// Update the extensions index...
			String[] exts = type.Extensions;
			if (exts != null)
			{
				foreach (string ext in exts)
				{
					IList list = (IList) _extIdx[ext];
					if (list == null)
					{
						// No type already registered for this extension...
						// So, create a list of types
						list = new ArrayList();
						_extIdx[ext] = list;
					}
					list.Add(type);
				}
			}
			// Update the magics index...
			if (type.HasMagic())
			{
				_mimeTypes.Add(type);
			}
		}

		/// <summary> Returns an array of matching MimeTypes from the specified name
		/// (many MimeTypes can have the same registered extensions).
		/// </summary>
		private MimeType[] GetMimeTypes(string name)
		{
			IList mimeTypes = null;
			int index = name.LastIndexOf('.');
			if ((index != -1) && (index != name.Length - 1))
			{
				// There's an extension, so try to find
				// the corresponding mime-types
				String ext = name.Substring(index + 1);
				mimeTypes = (IList) _extIdx[ext.ToLower()];
			}

			return
				(mimeTypes != null) ? (MimeType[]) SupportUtil.ToArray(mimeTypes, new MimeType[mimeTypes.Count]) : null;
		}
	}
}
