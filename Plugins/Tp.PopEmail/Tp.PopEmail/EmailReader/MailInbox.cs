// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Iesi.Collections.Generic;
using NServiceBus;
using StructureMap;
using Tp.Integration.Messages.Ticker;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.PopEmailIntegration.Data;
using Tp.PopEmailIntegration.EmailReader.Client;

namespace Tp.PopEmailIntegration.EmailReader
{
    public class MailInbox : IHandleMessages<TickMessage>
    {
        private readonly MessageUidRepository _messageUidRepository;
        private readonly IMessagePackSize _messagePackSize;
        private readonly ILocalBus _localBus;
        private readonly IActivityLogger _log;

        public MailInbox(ILocalBus localBus, MessageUidRepository messageUidRepository,
            IMessagePackSize messagePackSize, IActivityLogger log)
        {
            _messageUidRepository = messageUidRepository;
            _messagePackSize = messagePackSize;
            _localBus = localBus;
            _log = log;
        }

        public void Handle(TickMessage tickMessage)
        {
            _log.Info("Downloading uids from email server...");
            using (var client = ObjectFactory.GetInstance<IEmailClient>())
            {
                client.Connect();
                client.Login();

                var uids = GetNewUids(client);
                _messageUidRepository.AddRange(uids.ToArray());

                Debug.Assert(uids != null, "uids != null");

                var lastIndex = 0;
                while (true)
                {
                    var uidsPack = uids.Skip(lastIndex).Take(_messagePackSize.Value).ToArray();
                    if (!uidsPack.Any()) break;
                    lastIndex += _messagePackSize.Value;
                    _localBus.SendLocal(new EmailUidsRetrievedMessage { Uids = uidsPack });
                }

                client.Disconnect();
            }
        }

        /// <summary>
        /// Get ids of messages which have not been yet downloaded.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private string[] GetNewUids(IEmailClient client)
        {
            List<string> serverMessageUids;
            try
            {
                serverMessageUids = client.GetServerUids().ToList();
            }
            catch (MailBee.MailBeeException ex)
            {
                throw new EmailException("Error getting new Uids from Server", ex);
            }
            catch (EmailException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new EmailException("Error initializing email client", ex);
            }

            Debug.Assert(serverMessageUids != null, "serverUids != null");

            var alreadyRetrievedMsgUids = new HashedSet<string>(_messageUidRepository.GetUids());

            serverMessageUids.RemoveAll(alreadyRetrievedMsgUids.Contains);

            return serverMessageUids.ToArray();
        }
    }
}
