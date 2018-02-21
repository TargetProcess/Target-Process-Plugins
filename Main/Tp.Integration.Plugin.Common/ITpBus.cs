// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using NServiceBus;
using Tp.Integration.Messages;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.PluginLifecycle;

namespace Tp.Integration.Plugin.Common
{
    /// <summary>
    /// Provides all available bus operation.
    /// </summary>
    public interface ITpBus : ICommandBus, ILocalBus
    {
        /// <summary>
        /// Sends messages to TargetProcess.
        /// </summary>
        /// <param name="messages">Messages to send</param>
        /// <returns>Objects of this interface are returned from calling IBus.Send. The interface allows the caller to register for a callback when a response is received to their original call to IBus.Send.</returns>
        ICallback Send(params ITargetProcessMessage[] messages);

        /// <summary>
        /// Instantiates a message of type T and sends it.
        /// </summary>
        /// <typeparam name="T">The type of message, usually an interface</typeparam>
        /// <param name="messageConstructor">An action which initializes properties of the message</param>
        /// <remarks>
        /// The message will be sent to the destination configured for T
        /// </remarks>
        ICallback Send<T>(Action<T> messageConstructor) where T : ITargetProcessMessage;

        /// <summary>
        /// Sends the list of provided messages.
        /// </summary>
        /// <param name="destination">
        /// The address of the destination to which the messages will be sent.
        /// </param>
        /// <param name="messages">The list of messages to send.</param>
        ICallback Send(string destination, params ITargetProcessMessage[] messages);

        /// <summary>
        /// Instantiates a message of type T and sends it to the given destination.
        /// </summary>
        /// <typeparam name="T">The type of message, usually an interface</typeparam>
        /// <param name="destination">The destination to which the message will be sent.</param>
        /// <param name="messageConstructor">An action which initializes properties of the message</param>
        /// <returns></returns>
        ICallback Send<T>(string destination, Action<T> messageConstructor) where T : ITargetProcessMessage;

        /// <summary>
        /// Sends the messages to the destination as well as identifying this
        /// as a response to a message containing the Id found in correlationId.
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="correlationId"></param>
        /// <param name="messages"></param>
        void Send(string destination, string correlationId, params ITargetProcessMessage[] messages);

        /// <summary>
        /// Instantiates a message of the type T using the given messageConstructor,
        /// and sends it to the destination identifying it as a response to a message
        /// containing the Id found in correlationId.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="destination"></param>
        /// <param name="correlationId"></param>
        /// <param name="messageConstructor"></param>
        void Send<T>(string destination, string correlationId, Action<T> messageConstructor) where T : ITargetProcessMessage;

        /// <summary>
        /// Moves the message being handled to the back of the list of available 
        /// messages so it can be handled later.
        /// </summary>
        void HandleCurrentMessageLater();


        /// <summary>
        /// Tells the bus to stop dispatching the current message to additional
        /// handlers.
        /// </summary>
        void DoNotContinueDispatchingCurrentMessageToHandlers();

        /// <summary>
        /// Sets current saga id to the header of the message.
        /// </summary>
        /// <param name="id">The id of the saga</param>
        void SetOutSagaId(Guid id);

        /// <summary>
        /// Sends message to plugin queue with current context. So message will be processed only by current profile.
        /// </summary>
        /// <param name="profileNameName">Current profile name.</param>
        /// <param name="accountName">Current account name.</param>
        /// <param name="message">The message to send.</param>
        void SendLocalWithContext(ProfileName profileNameName, AccountName accountName, IMessage message);

        /// <summary>
        /// Sends message with plugin info to TargetProcess.
        /// </summary>
        /// <param name="pluginLifecycleMessages"></param>
        void Send(params IPluginLifecycleMessage[] pluginLifecycleMessages);

        /// <summary>
        /// Sends all messages to the endpoint which sent the message currently being handled on this thread.
        /// </summary>
        /// <param name="messages">The messages to send.</param>
        void Reply(params ITargetProcessMessage[] messages);

        void SendLocalUiWithContext(ProfileName profileName, AccountName accountName, IMessage message);
    }
}
