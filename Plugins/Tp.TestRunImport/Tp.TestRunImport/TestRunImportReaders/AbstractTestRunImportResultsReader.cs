// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.IO;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.TestRunImport.TestRunImport;

namespace Tp.Integration.Plugin.TestRunImport.TestRunImportReaders
{
    public abstract class AbstractTestRunImportResultsReader
    {
        private readonly IActivityLogger _log;

        protected AbstractTestRunImportResultsReader(IActivityLogger log, StreamReader reader)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            if (reader == null)
            {
                _log.Error("Ctor member reader is null");
                throw new ArgumentNullException(nameof(reader));
            }
            Reader = reader;
        }

        protected IActivityLogger Log => _log;

        protected StreamReader Reader { get; }
        public abstract List<TestRunImportResultInfo> GetTestRunImportResults();
    }
}
