// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Text;
using System;
using Tp.Integration.Plugin.Common.Activity;

namespace Tp.Integration.Testing.Common
{
    public class LogMock : IActivityLogger
    {
        private readonly List<string> _messages = new List<string>();

        public List<string> Messages
        {
            get { return _messages; }
        }

        public void DebugFormat(string format, params object[] args)
        {
            AddMessage(string.Format(format, args));
        }

        public void Error(string message, Exception exception)
        {
            AddMessage(message);
        }

        public void InfoFormat(string format, params object[] args)
        {
            AddMessage(string.Format(format, args));
        }

        public void Info(string message)
        {
            AddMessage(message);
        }

        public void Warn(string message)
        {
            AddMessage(message);
        }

        public bool IsDebugEnabled
        {
            get { return true; }
        }

        public void Debug(string message)
        {
            AddMessage(message);
        }

        public void Error(string message)
        {
            AddMessage(message);
        }

        public void Error(Exception ex)
        {
            AddMessage(ex.Message);
        }

        public void Fatal(string message)
        {
            AddMessage(message);
        }

        public void Fatal(Exception ex)
        {
            AddMessage(ex.Message);
        }

        public void WarnFormat(string format, params object[] args)
        {
            AddMessage(string.Format(format, args));
        }


        public void ErrorFormat(string format, params object[] args)
        {
            AddMessage(string.Format(format, args));
        }

        private void AddMessage(string message)
        {
            _messages.Add(message);
        }
    }
}
