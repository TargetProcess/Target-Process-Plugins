// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using Tp.Integration.Messages.PluginLifecycle;

namespace Tp.Integration.Plugin.Common
{
    /// <summary>
    /// Represents a bus for sending messages to local plugin queue.
    /// This messages will be handled by plugin.
    /// </summary>
    public interface ILocalBus
    {
        /// <summary>
        /// Sends messages to plugin queue.
        /// </summary>
        /// <param name="messages">Messages to send</param>
        void SendLocal(params IPluginLocalMessage[] messages);

        /// <summary>
        /// Sends message to plugin queue
        /// </summary>
        /// <typeparam name="T">The Type of the message to send.</typeparam>
        /// <param name="messageConstructor">Message constructor.</param>
        void SendLocal<T>(Action<T> messageConstructor) where T : IPluginLocalMessage;
    }
}
