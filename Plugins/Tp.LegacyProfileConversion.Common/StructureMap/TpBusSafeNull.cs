// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using NServiceBus;
using NServiceBus.Unicast;
using Tp.Core;
using Tp.Integration.Common;
using Tp.Integration.Messages;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common;

namespace Tp.LegacyProfileConvertsion.Common.StructureMap
{
    internal class TpBusSafeNull : SafeNull<TpBusSafeNull, ITpBus>, ITpBus, INullable
    {
        private static readonly Callback Callback = new Callback(Guid.Empty.ToString());

        public ICallback Send(params ICreateEntityCommand<DataTransferObject>[] createCommands)
        {
            return Callback;
        }

        public ICallback Send(params IUpdateEntityCommand<DataTransferObject>[] updateCommands)
        {
            return Callback;
        }

        public ICallback Send(params IDeleteEntityCommand[] deleteCommands)
        {
            return Callback;
        }

        public void SendLocal(params IPluginLocalMessage[] messages)
        {
        }

        public void SendLocal<T>(Action<T> messageConstructor) where T : IPluginLocalMessage
        {
        }

        public ICallback Send(params ITargetProcessMessage[] messages)
        {
            return Callback;
        }

        public ICallback Send<T>(Action<T> messageConstructor) where T : ITargetProcessMessage
        {
            return Callback;
        }

        public ICallback Send(string destination, params ITargetProcessMessage[] messages)
        {
            return Callback;
        }

        public ICallback Send<T>(string destination, Action<T> messageConstructor) where T : ITargetProcessMessage
        {
            return Callback;
        }

        public void Send(string destination, string correlationId, params ITargetProcessMessage[] messages)
        {
        }

        public void Send<T>(string destination, string correlationId, Action<T> messageConstructor) where T : ITargetProcessMessage
        {
        }

        public void HandleCurrentMessageLater()
        {
        }

        public void DoNotContinueDispatchingCurrentMessageToHandlers()
        {
        }

        public void SetOutSagaId(Guid id)
        {
        }

        public void SendLocalWithContext(ProfileName profileNameName, AccountName accountName, IMessage message)
        {
        }

        public void Send(params IPluginLifecycleMessage[] pluginLifecycleMessages)
        {
        }

        public void Reply(params ITargetProcessMessage[] messages)
        {
        }

        public void SendLocalUiWithContext(ProfileName profileName, AccountName accountName, IMessage message)
        {
        }
    }
}
