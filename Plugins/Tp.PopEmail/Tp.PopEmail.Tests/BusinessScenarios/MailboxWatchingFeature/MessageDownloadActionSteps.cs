// 
// Copyright (c) 2005-2016 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using MailBee.Mime;
using NBehave.Narrator.Framework;
using StructureMap;
using Tp.Integration.Messages.Ticker;
using Tp.PopEmailIntegration.BusinessScenarios.HandleEmailsFromUserFeature;
using Tp.Testing.Common.NUnit;

namespace Tp.PopEmailIntegration.BusinessScenarios.MailboxWatchingFeature
{
    [ActionSteps]
    public class MessageDownloadActionSteps : EmailProcessingSagaActionSteps
    {
        [Given("there are messages in mail inbox:")]
        public void PutMessagesIntoInbox(string uid, string from)
        {
            SetSenderAddressAndSubject(uid.Trim(), "", from == "''" ? string.Empty : from.Trim());
        }

        [Given("profile has downloaded message '$uid'")]
        public void ProfileHasDownloadedMessage(string uid)
        {
            Context.MessageUids.Add(uid);
        }

        [Then(@"messages should be passed to process: (?<uids>([^,]+,?\s*)+)")]
        public void MessageShouldBeAttachedToProject(string[] uids)
        {
            Context.Transport.LocalQueue.GetMessages<EmailReceivedMessage>().Select(x => x.Mail.MessageUidDto.UID).ToArray().
                Should(Be.EquivalentTo(uids),
                    "Context.Transport.LocalQueue.GetMessages<EmailReceivedMessage>().Select(x => x.Mail.MessageUidDto.UID).ToArray().Should(Be.EquivalentTo(uids))");
        }

        [Given("message with uid '$uid' has sender address '$email'")]
        public void SetSenderAddress(string uid, string email)
        {
            SetSenderAddressAndSubject(uid, "", email);
        }

        [Given("message with uid '$uid' has subject '$subject' and sender address '$email'")]
        public void SetSenderAddressAndSubject(string uid, string subject, string email)
        {
            Context.EmailClient.Messages[uid] = new MailMessage
            {
                From = new EmailAddress(email),
                Subject = string.IsNullOrEmpty(subject) ? string.Empty : subject
            };
        }

        [Given(@"mail server has uids: (?<uids>([^,]+,?\s*)+)")]
        public void SetServerUids(string[] uids)
        {
            uids.ForEach(x => Context.EmailClient.Messages[x] = new MailMessage());
        }

        [Then(@"downloaded messages should be: (?<uids>([^,]+,?\s*)+)")]
        public void DownloadedMessagesShouldBe(string[] uids)
        {
            Context.MessageUids.GetUids().Should(Be.EquivalentTo(uids), "Context.MessageUids.GetUids().Should(Be.EquivalentTo(uids))");
        }

        [Given("message with uid '$uid' has empty sender address")]
        public void SetEmptyAddress(string uid)
        {
            Context.EmailClient.Messages[uid] = new MailMessage { From = new EmailAddress(string.Empty) };
        }

        [Given("email server is down")]
        public void SetEmailServerDown()
        {
            Context.EmailClient.SetDown();
        }

        [When("tick occurs")]
        public void Tick()
        {
            Context.Transport.HandleLocalMessage(Context.Profile, new TickMessage());
        }

        [Then("message '$uid' should be passed to process")]
        public void MessageShouldBeDownloaded(string uid)
        {
            Context.Transport.LocalQueue.GetMessages<EmailReceivedMessage>()
                .Count()
                .Should(Be.EqualTo(1), "Context.Transport.LocalQueue.GetMessages<EmailReceivedMessage>().Count().Should(Be.EqualTo(1))");
            Context.Transport.LocalQueue.GetMessages<EmailReceivedMessage>()
                .First()
                .Mail.MessageUidDto.UID.Should(Be.EqualTo(uid),
                    "Context.Transport.LocalQueue.GetMessages<EmailReceivedMessage>().First().Mail.MessageUidDto.UID.Should(Be.EqualTo(uid))");
        }

        [Then("downloaded messages should be empty")]
        public void DownloadedMessagesShouldBeEmpty()
        {
            Context.MessageUids.GetUids().Should(Be.Empty, "Context.MessageUids.GetUids().Should(Be.Empty)");
        }

        [Then("no messages should be passed to process")]
        public void NoMessageShouldBeDownloaded()
        {
            Context.Transport.LocalQueue.GetMessages<EmailReceivedMessage>()
                .Count()
                .Should(Be.EqualTo(0), "Context.Transport.LocalQueue.GetMessages<EmailReceivedMessage>().Count().Should(Be.EqualTo(0))");
        }

        private MessageDownloadContext Context
        {
            get { return ObjectFactory.GetInstance<MessageDownloadContext>(); }
        }
    }
}
