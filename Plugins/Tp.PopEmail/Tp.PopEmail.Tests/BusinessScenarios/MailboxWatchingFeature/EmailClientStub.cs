// 
// Copyright (c) 2005-2016 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using MailBee.Mime;
using Tp.Integration.Plugin.Common.Validation;
using Tp.PopEmailIntegration.EmailReader;
using Tp.PopEmailIntegration.EmailReader.Client;

namespace Tp.PopEmailIntegration.BusinessScenarios.MailboxWatchingFeature
{
    public class EmailClientStub : IEmailClient
    {
        private readonly Dictionary<string, MailMessage> _messages = new Dictionary<string, MailMessage>();
        private bool _isDown;

        public void Dispose()
        {
        }

        public void Connect()
        {
        }

        public void Login()
        {
        }

        public void Disconnect()
        {
        }

        public Dictionary<string, MailMessage> Messages => _messages;

        public string[] GetServerUids()
        {
            return _messages.Keys.ToArray();
        }

        public void CheckConnection(PluginProfileErrorCollection errors)
        {
        }

        public MailMessage DownloadMessage(string uid)
        {
            if (_isDown)
            {
                throw new Exception("Server is down");
            }
            return _messages[uid];
        }

        public bool IsDsnMessage(MailMessage message)
        {
            return false;
        }

        void IEmailClient.Connect()
        {
            try
            {
                Connect();
            }
            catch (EmailException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new EmailException("Cannot connect", ex);
            }
        }

        void IEmailClient.Login()
        {
            try
            {
                Login();
            }
            catch (EmailException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new EmailException("Cannot login", ex);
            }
        }

        void IEmailClient.Disconnect()
        {
            try
            {
                Disconnect();
            }
            catch (EmailException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new EmailException("Cannot disconnect", ex);
            }
        }

        public void SetDown()
        {
            _isDown = true;
        }
    }
}
