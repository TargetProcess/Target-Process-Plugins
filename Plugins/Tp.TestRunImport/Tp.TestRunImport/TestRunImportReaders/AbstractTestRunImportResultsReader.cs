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

        protected AbstractTestRunImportResultsReader(IActivityLogger log, TextReader reader)
        {
            if (log == null)
            {
                throw new ArgumentNullException("log");
            }
            _log = log;
            if (reader == null)
            {
                _log.Error("Ctor member reader is null");
                throw new ArgumentNullException("reader");
            }
            Reader = reader;
        }

        protected IActivityLogger Log
        {
            get { return _log; }
        }

        protected TextReader Reader { get; private set; }
        public abstract List<TestRunImportResultInfo> GetTestRunImportResults();
    }
}
