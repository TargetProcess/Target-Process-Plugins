// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Tp.Utils.Csv
{
	/// <summary>
	/// Writes CSV files.
	/// </summary>
	public class CsvWriter
	{
		private readonly CsvRenderer _csvRenderer;

		/// <summary>
		/// Creates new instance of this class.
		/// </summary>
		public CsvWriter(CsvRenderer csvRenderer)
		{
			_csvRenderer = csvRenderer;
		}

		public void RenderDataTable(DataTable data)
		{
			RenderHeader(data);
			RenderRows(data.Rows.Cast<DataRow>());
		}

		public void RenderHeader(DataTable data)
		{
			var columnNames = data.Columns.OfType<DataColumn>().Select(x => x.ColumnName);
			_csvRenderer.Render(columnNames);
		}

		public void RenderRows(IEnumerable<DataRow> dataRowCollection)
		{
			foreach (var row in dataRowCollection)
			{
				RenderRow(row);
			}
		}

		public void RenderRow(DataRow row)
		{
			_csvRenderer.Render(row.ItemArray.Select(Convert.ToString));
		}
	}
}