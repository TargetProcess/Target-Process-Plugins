using System;
using System.Globalization;

namespace Tp.Search.Model.Entity
{
	[Serializable]
	public class EntityDocument : hOOt.Document
	{
		public EntityDocument(){}
		public EntityDocument(string filename, string text) : base (filename, text)
		{
		}
		public string EntityTypeId { get; set; }
		public string ProjectId { get; set; }
		public string SquadId { get; set; }

		public static string CreateName(int? id)
		{
			return id.GetValueOrDefault().ToString(CultureInfo.InvariantCulture);
		}
	}
}