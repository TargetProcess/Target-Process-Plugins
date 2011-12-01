// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using Tp.Core;
using Tp.Integration.Plugin.Common.Validation;

namespace Tp.SourceControl.VersionControlSystem
{
	public interface IVersionControlSystem : IDisposable
	{
		/// <summary>
		/// Get changeset infos between the specified numbers, inclusive.
		/// </summary>
		/// <param name="revisionRange">Revision range to get log.</param>
		/// <returns></returns>
		RevisionInfo[] GetRevisions(RevisionRange revisionRange);

		/// <summary>
		/// Get contents of the specified text file at the specified changeset.
		/// </summary>
		/// <param name="changeset"></param>
		/// <param name="path"></param>
		/// <returns>Text content of the specified file.</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="path"/> is <c>null</c>.</exception>
		/// <exception cref="VersionControlException">If a file cannot be found in a specified changeset.</exception>
		string GetTextFileContent(RevisionId changeset, string path);

		/// <summary>
		/// Get contents of the specified binary file at the specified changeset.
		/// </summary>
		/// <param name="changeset"></param>
		/// <param name="path">The file name whose binary content to retrieve.</param>
		/// <returns>Binary content of the specifid file.</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="path"/> is <c>null</c>.</exception>
		/// <exception cref="VersionControlException">If a file cannot be found in a specified changeset.</exception>
		byte[] GetBinaryFileContent(RevisionId changeset, string path);

		///// <summary>
		///// Compare file content at the specified and previous revision.
		///// </summary>
		///// <param name="path">Path to file.</param>
		///// <param name="changeset">The specific changeset number.</param>
		///// <param name="original">Text file content at the specified changeset number.</param>
		///// <param name="previous">Text file content at the previous changeset number.</param>
		///// <returns></returns>
		///// <exception cref="ArgumentNullException">If <paramref name="path"/> is <c>null</c>.</exception>
		///// <exception cref="VersionControlException">If a file cannot be found in a specified changeset.</exception>
		//long GetComparedFiles(string path, IRevisionId changeset, out string original, out string previous);

		void CheckRevision(RevisionId revision, PluginProfileErrorCollection errors);
		string[] RetrieveAuthors(DateRange dateRange);

		RevisionRange[] GetFromTillHead(RevisionId from, int pageSize);
		RevisionRange[] GetAfterTillHead(RevisionId from, int pageSize);
		RevisionRange[] GetFromAndBefore(RevisionId from, RevisionId to, int pageSize);
	}
}