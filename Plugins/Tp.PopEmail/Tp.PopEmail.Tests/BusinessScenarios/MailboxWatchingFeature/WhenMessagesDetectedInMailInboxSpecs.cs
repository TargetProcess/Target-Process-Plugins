// 
// Copyright (c) 2005-2016 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NUnit.Framework;
using Tp.Testing.Common.NBehave;

namespace Tp.PopEmailIntegration.BusinessScenarios.MailboxWatchingFeature
{
    [TestFixture]
    [Category("PartPlugins1")]
    public class WhenMessagesDetectedInMailInboxSpecs
    {
        [Test]
        public void ShouldDownloadNewMessagesOnly()
        {
            @"Given profile has downloaded message 'Uid1'
					And mail server has uids: Uid1,Uid2
					And message with uid 'Uid1' has sender address '1@1.com'
					And message with uid 'Uid2' has sender address '2@2.com'
				When tick occurs
				Then message 'Uid2' should be passed to process"
                .Execute(In.Context<MessageDownloadActionSteps>());
        }

        [Test]
        public void ShouldNotMarkMessageAsReadIfExceptionOccursOnMailServer()
        {
            @"Given project 1
					And profile has a rule: then attach to project 1
					And there are messages in mail inbox:
					|uid	|from|
					|uid1	|1@1.com|
					|uid2	|2@2.com|
					And email server is down
				When tick occurs
				Then downloaded messages should be empty
					And no messages should be passed to process"
                .Execute(In.Context<MessageDownloadActionSteps>());
        }

        [Test]
        public void ShouldSkipMessagesWithEmptyFromAddress()
        {
            @"Given project 1
					And profile has a rule: then attach to project 1
					And there are messages in mail inbox:
					|uid	|from|
					|uid1	|''|
					|uid2	|1@1.com|
				When tick occurs
				Then downloaded messages should be: uid1, uid2
					And message 'uid2' should be passed to process"
                .Execute(In.Context<MessageDownloadActionSteps>());
        }

        [Test]
        public void ShouldProcessNewMessagesMultyLineSubject()
        {
            @"Given profile has downloaded message 'Uid1'
					And mail server has uids: Uid1,Uid2
					And message with uid 'Uid1' has sender address '1@1.com'
					And message with uid 'Uid2' has subject 'Test subject line 1
Test subject line2' and sender address '2@2.com'
				When tick occurs
				Then message 'Uid2' should be passed to process"
                .Execute(In.Context<MessageDownloadActionSteps>());
        }

        [Test]
        public void ShouldProcessReplyToTextMessageFromMacWithAttachment()
        {
            @"Given project 1
					And reply text message with attachment from mac with uid 'Uid1' has subject 'Re: New Comment added to Request #5 ""Test from Mac""'
				When tick occurs
				Then downloaded messages should be: Uid1
					And message 'Uid1' should be passed to process
					And message with subject 'Re: New Comment added to Request #5 ""Test from Mac""' should be passed to process in TP"
                .Execute(In.Context<MessageDownloadActionSteps>());
        }

        [Test]
        public void ShouldProcessReplyToHtmlMessageFromMacWithAttachment()
        {
            @"Given project 1
					And reply html message with attachment from mac with uid 'Uid1' has subject 'Re: New Comment added to Request #5 ""Test from Mac""'
				When tick occurs
				Then downloaded messages should be: Uid1
					And message 'Uid1' should be passed to process
					And message with subject 'Re: New Comment added to Request #5 ""Test from Mac""' should be passed to process in TP"
                .Execute(In.Context<MessageDownloadActionSteps>());
        }

        [Test]
        public void ShouldProcessNewMessagesWithPdfAttachmentWithRfc2045Mime()
        {
            @"Given profile has downloaded message 'Uid1'
					And mail server has uids: Uid1,Uid2
					And message with uid 'Uid2' has attachment with mime 'Content-Type: application/pdf
	name=""=?utf-8?Q?ERRORE_LCL_6301419062.pdf?=""
Content-Transfer-Encoding: base64
Content-Disposition: attachment; filename=""=?utf-8?Q?ERRORE_LCL_6301419062.pdf?=""' and sender address '1@1.com'
				When tick occurs
				Then message with 'Uid2' with attachment 'ERRORE LCL 6301419062.pdf' of content type 'application/pdf' should be passed to process"
                .Execute(In.Context<MessageDownloadActionSteps>());
        }

        [Test]
        public void ShouldProcessNewMessagesWithZipAttachmentWithRfc2045Mime()
        {
            @"Given profile has downloaded message 'Uid1'
					And mail server has uids: Uid1,Uid2
					And message with uid 'Uid2' has attachment with mime 'Content-Type: application/zip, application/octet-stream;
	name=""15156891401442_0.zip""
Content-Transfer-Encoding: base64
Content-Disposition: attachment;
	filename=""15156891401442_0.zip""' and sender address '1@1.com'
				When tick occurs
				Then message with 'Uid2' with attachment '15156891401442_0.zip' of content type 'application/zip' should be passed to process"
                .Execute(In.Context<MessageDownloadActionSteps>());
        }
    }
}
