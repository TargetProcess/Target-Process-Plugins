using System;

namespace Tp.Utils.Mime
{
	/// <summary>
	/// Summary description for MimeTypeException.
	/// </summary>
	public class MimeTypeException : ApplicationException
	{
		#region Class Constructor

		/// <summary>
		/// 
		/// </summary>
		public MimeTypeException()
			: base("Mime detection exception")
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="strMsg"></param>
		public MimeTypeException(String strMsg)
			: base(strMsg)
		{
		}

		#endregion
	}
}
