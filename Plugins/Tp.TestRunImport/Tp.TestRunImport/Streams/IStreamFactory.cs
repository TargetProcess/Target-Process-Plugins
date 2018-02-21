// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.IO;

namespace Tp.Integration.Plugin.TestRunImport.Streams
{
    /// <summary>
    /// Creates new stream instances upon request.
    /// </summary>
    public interface IStreamFactory
    {
        /// <summary>
        /// Open stream for the URL specified in settings, regarding modifications in the underlying file.
        /// Otherwise return <c>null</c>.
        /// </summary>
        /// <param name="uri">
        /// Path to file or web resource to open.
        /// </param>
        /// <param name="settings">
        /// Currrent TestRunImportPluginProfile.
        /// </param>
        /// <returns>
        /// Either a <see cref="Stream"/> instance, or <c>null</c> if the underlaying file has not been changed since the last read.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="uri"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ApplicationException">
        /// If <paramref name="uri"/> specify empty or non-existing file name.
        /// </exception>
        /// <exception cref="IOException">
        /// If any input/output error occurs.
        /// </exception>
        StreamFactoryResult OpenStream(Uri uri, TestRunImportPluginProfile settings);

        /// <summary>
        /// Open stream for the URL specified in settings, only if there are modifications in the underlying file.
        /// Otherwise return <c>null</c>.
        /// </summary>
        /// <param name="uri">
        /// Path to file or web resource to open.
        /// </param>
        /// <param name="settings">
        /// Currrent TestRunImportPluginProfile.
        /// </param>
        /// <param name="lastModifyResult">
        /// Stream last modification result.
        /// </param>
        /// <returns>
        /// Either a <see cref="Stream"/> instance, or <c>null</c> if the underlaying file has not been changed since the last read.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="uri"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ApplicationException">
        /// If <paramref name="uri"/> specify empty or non-existing file name.
        /// </exception>
        /// <exception cref="IOException">
        /// If any input/output error occurs.
        /// </exception>
        StreamFactoryResult OpenStreamIfModified(Uri uri, TestRunImportPluginProfile settings, LastModifyResult lastModifyResult);
    }
}
