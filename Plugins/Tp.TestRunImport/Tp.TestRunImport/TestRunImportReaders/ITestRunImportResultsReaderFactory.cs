// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.IO;
using Tp.Integration.Plugin.TestRunImport.TestRunImport;

namespace Tp.Integration.Plugin.TestRunImport.TestRunImportReaders
{
    /// <summary>
    /// Creates new AbstractTestRunImportResultsReader instances upon request.
    /// </summary>
    public interface ITestRunImportResultsReaderFactory
    {
        /// <summary>
        /// Creates new instance if AbstractTestRunImportResultsReader for the specified settings.
        /// </summary>
        /// <param name="settings">
        /// Current test run import plugin settings.
        /// </param>
        /// <param name="reader">
        /// A TextReader that can read test run results.
        /// </param>
        /// <returns>
        /// Instance of AbstractTestRunImportResultsReader.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="reader"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="settings"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ApplicationException">
        /// If no suitable AbstractTestRunImportResultsReader can be created.
        /// </exception>
        AbstractTestRunImportResultsReader GetResolver(TestRunImportSettings settings, StreamReader reader);
    }
}
