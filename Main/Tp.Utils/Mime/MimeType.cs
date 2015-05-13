// 
// Copyright (c) 2005-2008 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Tp.Utils.Mime
{
	/// <summary>
	/// Defines a Mime Content Type
	/// </summary>
	public sealed class MimeType
	{
		#region Private Members

		/// <summary>The primary and sub types separator </summary>
		private const string SEPARATOR = "/";

		/// <summary>The parameters separator </summary>
		private const string PARAMS_SEP = ";";

		/// <summary>Special characters not allowed in content types. </summary>
		private const string SPECIALS = "()<>@,;:\\\"/[]?=";

		/// <summary>The Mime-Type full name </summary>
		private string _name;

		/// <summary>The Mime-Type primary type </summary>
		private string _primary;

		/// <summary>The Mime-Type sub type </summary>
		private string _sub;

		/// <summary>The Mime-Type associated extensions </summary>
		private ArrayList _extensions;

		/// <summary>The magic bytes associated to this Mime-Type </summary>
		private ArrayList _magics;

		/// <summary>The minimum length of data to provides for magic analyzis </summary>
		private Int32 _minLength;

		#endregion

		#region Class Constructor

		/// <summary> Creates a MimeType from a string.</summary>
		/// <param name="name">the MIME content type string.
		/// </param>
		public MimeType(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new MimeTypeException("The type can not be null or empty");
			}

			// Split the two parts of the Mime Content Type
			string[] parts = name.Split(new[] {SEPARATOR[0]}, 2);

			// Checks validity of the parts
			if (parts.Length != 2)
			{
				throw new MimeTypeException("Invalid Content Type " + name);
			}

			Init(parts[0], parts[1]);
		}

		/// <summary> Creates a MimeType with the given primary type and sub type.</summary>
		/// <param name="primary">the content type primary type.
		/// </param>
		/// <param name="sub">the content type sub type.
		/// </param>
		public MimeType(string primary, string sub)
		{
			Init(primary, sub);
		}

		#endregion

		#region Public Properties

		/// <summary> Return the name of this mime-type.</summary>
		/// <returns> the name of this mime-type.
		/// </returns>
		public string Name
		{
			get { return _name; }
		}

		/// <summary> Return the primary type of this mime-type.</summary>
		/// <returns> the primary type of this mime-type.
		/// </returns>
		public string PrimaryType
		{
			get { return _primary; }
		}

		/// <summary>Return the sub type of this mime-type.</summary>
		/// <returns>the sub type of this mime-type.</returns>
		public string SubType
		{
			get { return _sub; }
		}

		/// <summary>Return the description of this mime-type.</summary>
		internal string Description { get; set; }

		/// <summary>Return the extensions of this mime-type</summary>
		/// <returns>the extensions associated to this mime-type.</returns>
		internal string[] Extensions
		{
			get { return (string[]) SupportUtil.ToArray(_extensions, new string[_extensions.Count]); }
		}

		internal int MinLength
		{
			get { return _minLength; }
		}

		#endregion

		#region Public Methods

		/// <summary> Cleans a content-type.
		/// This method cleans a content-type by removing its optional parameters
		/// and returning only its <code>primary-type/sub-type</code>.
		/// </summary>
		/// <param name="type">is the content-type to clean.
		/// </param>
		/// <returns> the cleaned version of the specified content-type.
		/// </returns>
		/// <throws>  MimeTypeException if something wrong occurs during the </throws>
		/// <summary>         parsing/cleaning of the specified type.
		/// </summary>
		public static string Clean(string type)
		{
			return (new MimeType(type)).Name;
		}

		public override string ToString()
		{
			return Name;
		}

		/// <summary> Indicates if an object is equal to this mime-type.
		/// The specified object is equal to this mime-type if it is not null, and
		/// it is an instance of MimeType and its name is equals to this mime-type.
		/// 
		/// </summary>
		/// <param name="obj">the reference object with which to compare.
		/// </param>
		/// <returns> <code>true</code> if this mime-type is equal to the object
		/// argument; <code>false</code> otherwise.
		/// </returns>
		public override bool Equals(Object obj)
		{
			try
			{
				return ((MimeType) obj).Name.Equals(Name);
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		#endregion

		#region Internal Methods

		/// <summary> Add a supported extension.</summary>
		/// <param name="ext">extension to add to the list of extensions associated
		/// to this mime-type.
		/// </param>
		internal void AddExtension(string ext)
		{
			_extensions.Add(ext);
		}

		internal void AddMagic(int offset, string type, string magic)
		{
			// Some preliminary checks...
			if (string.IsNullOrEmpty(magic))
			{
				return;
			}
			var m = new Magic(offset, type, magic);
			_magics.Add(m);
			_minLength = Math.Max(_minLength, m.Size());
		}

		internal bool HasMagic()
		{
			return (_magics.Count > 0);
		}

		public bool Matches(string url)
		{
			bool match = false;
			int index = url.LastIndexOf('.');
			if ((index != - 1) && (index < url.Length - 1))
			{
				// There's an extension, so try to find if it matches mines
				match = _extensions.Contains(url.Substring(index + 1));
			}
			return match;
		}

		internal bool Matches(byte[] data)
		{
			if (!HasMagic())
			{
				return false;
			}

			return _magics.Cast<Magic>().Any(tested => tested.Matches(data));
		}

		#endregion

		#region Helper Methods

		/// <summary>Init method used by constructors. </summary>
		private void Init(string primary, string sub)
		{
			// Preliminary checks...
			if (string.IsNullOrEmpty(primary) || (!IsValid(primary)))
			{
				throw new MimeTypeException("Invalid Primary Type " + primary);
			}
			// Remove optional parameters from the sub type
			string clearedSub = null;
			if (sub != null)
			{
				clearedSub = sub.Split(PARAMS_SEP[0])[0];
			}
			if (string.IsNullOrEmpty(clearedSub) || (!IsValid(clearedSub)))
			{
				throw new MimeTypeException("Invalid Sub Type " + clearedSub);
			}

			// All is ok, assign values
			_name = primary + SEPARATOR + clearedSub;
			_primary = primary;
			_sub = clearedSub;
			_extensions = new ArrayList();
			_magics = new ArrayList();
		}

		/// <summary>Checks if the specified primary or sub type is valid. </summary>
		private static bool IsValid(string type)
		{
			return (type != null) && (type.Trim().Length > 0) && !HasCtrlOrSpecials(type);
		}

		/// <summary>Checks if the specified string contains some special characters. </summary>
		private static bool HasCtrlOrSpecials(string type)
		{
			int len = type.Length;
			int i = 0;
			while (i < len)
			{
				char c = type[i];
				if (c <= '\x001A' || SPECIALS.IndexOf(c) > 0)
				{
					return true;
				}
				i++;
			}
			return false;
		}

		#endregion

		#region Magic Class

		private class Magic
		{
			private readonly int _offset;
			private readonly byte[] _magic;

			internal Magic(int offset, string type, string magic)
			{
				_offset = offset;

				if ((type != null) && (type.Equals("System.Byte")))
				{
					_magic = ReadBytes(magic);
				}
				else
				{
					_magic = SupportUtil.ToByteArray(magic);
				}
			}

			internal int Size()
			{
				return (_offset + _magic.Length);
			}

			internal bool Matches(byte[] data)
			{
				if (data == null)
				{
					return false;
				}

				int idx = _offset;
				if ((idx + _magic.Length) > data.Length)
				{
					return false;
				}

				for (int i = 0; i < _magic.Length; i++)
				{
					if (_magic[i] != data[idx++])
					{
						return false;
					}
				}
				return true;
			}

			private static byte[] ReadBytes(string magic)
			{
				byte[] data = null;

				if (magic.Length % 2 != 0)
					return data;


				data = new byte[magic.Length / 2];
				for (int i = 0; i < magic.Length; i += 2)
				{
					string part = magic.Substring(i, 2);

					data[i/2] = byte.Parse(part, NumberStyles.HexNumber);
				}
				return data;

			}

			public override string ToString()
			{
				var buf = new StringBuilder();
				buf.Append("[").Append(_offset).Append("/").Append(Encoding.ASCII.GetString(_magic)).Append("]");
				return buf.ToString();
			}
		}

		#endregion
	}
}