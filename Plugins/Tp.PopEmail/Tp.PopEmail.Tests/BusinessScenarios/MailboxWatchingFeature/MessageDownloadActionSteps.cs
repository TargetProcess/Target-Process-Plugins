// 
// Copyright (c) 2005-2018 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.IO;
using System.Linq;
using System.Text;
using MailBee.Mime;
using NBehave.Narrator.Framework;
using StructureMap;
using Tp.Integration.Messages.EntityLifecycle.Commands;
using Tp.Integration.Messages.Ticker;
using Tp.Integration.Testing.Common;
using Tp.PopEmailIntegration.BusinessScenarios.HandleEmailsFromUserFeature;
using Tp.PopEmailIntegration.Data;
using Tp.Testing.Common.NUnit;

namespace Tp.PopEmailIntegration.BusinessScenarios.MailboxWatchingFeature
{
    [ActionSteps]
    public class MessageDownloadActionSteps : EmailProcessingSagaActionSteps
    {
        [Given("there are messages in mail inbox:")]
        public void PutMessagesIntoInbox(string uid, string from)
        {
            SetSenderAddressAndSubject(uid.Trim(), string.Empty, from == "''" ? string.Empty : from.Trim());
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
            SetSenderAddressAndSubject(uid, string.Empty, email);
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

        [Given("reply text message with attachment from mac with uid '$uid' has subject '$subject'")]
        public void SetSenderAddressAndSubjectForTextMessageWithAttachmentFromMac(string uid, string subject)
        {
            var message = new MailMessage();
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(@"Delivered-To: email.integr@gmail.com
Received: by 2002:a0d:e582:0:0:0:0:0 with SMTP id o124-v6csp4397794ywe;
        Mon, 6 Aug 2018 04:20:59 -0700 (PDT)
X-Received: by 2002:a19:1460:: with SMTP id k93-v6mr10632892lfi.15.1533554459205;
        Mon, 06 Aug 2018 04:20:59 -0700 (PDT)
ARC-Seal: i=1; a=rsa-sha256; t=1533554459; cv=none;
        d=google.com; s=arc-20160816;
        b=fpMq99UI6IpTv8uUuUZdDZThWXzih+MtYalQxPcfR3qmoCP5mrNTOPM/1IyKASGQyR
         IXxO+igOv09sjnY4+KhFtzp1jQHULhSd/kY9Ukwy9WczNpROPCqcmXavMnnQD2ugNulW
         uA6PWLsbrI7kRdYqn6sS7YB8d2MXA2Q37J12jniE0Sy9AHMp33CR6fr+4J62m+jBQcyq
         PqMtJ930F3MdLnX0+sN9AZnpfwpwmPvfOvsCBwBoEpIsrlEvEnl3gZf68t3BFbeIm+f3
         PC+ElZj4CEAlNmuMdtHrlohDnGBiwcxRc4WXGoT2f9xbfq7TPe6sg6vH9X72itacRXki
         wSnw==
ARC-Message-Signature: i=1; a=rsa-sha256; c=relaxed/relaxed; d=google.com; s=arc-20160816;
        h=to:date:message-id:subject:mime-version:from:dkim-signature
         :arc-authentication-results;
        bh=fOkMElT7ATlyQbct/4tBdf1QutEoN0tbL3SrMjgIFPc=;
        b=K7Xdh+5BU5Bp1XFMd/hUnRWy3rKlvYADbh2p9zVMbbc14kzzRyOY/NFxon0Ldbwfqk
         lFsbUwMMX5iD4uTKuwZcXJxH/+stoZ9z4BAyzbvfahEMRF4fgqFm/g/WixJilFzTcyGh
         BA6v59G/ya0hhE0lwhYjax/GaBKtk3zpzGQfcxpIbqfekuc6q2UDM1yE8Ys4XmUFQQo0
         jjQCo/zWI4AzLe2qh6QtgdbVVNTDJC9BfqNivHb85/3eZn7JYW8sGbVTm4TR8GKUp2N3
         brq1DwpgzQfdjSC8x8srZcwO5llUVIc8iAewwW3hhqkvm8lOizCNvc1n9KenCN7Yb9C6
         AlLQ==
ARC-Authentication-Results: i=1; mx.google.com;
       dkim=pass header.i=@targetprocess-com.20150623.gappssmtp.com header.s=20150623 header.b=HHj15PNr;
       spf=pass (google.com: domain of andrey.vaskovskiy@targetprocess.com designates 209.85.220.65 as permitted sender) smtp.mailfrom=andrey.vaskovskiy@targetprocess.com
Return-Path: <andrey.vaskovskiy@targetprocess.com>
Received: from mail-sor-f65.google.com (mail-sor-f65.google.com. [209.85.220.65])
        by mx.google.com with SMTPS id j29-v6sor1207710lfi.125.2018.08.06.04.20.59
        for <email.integr@gmail.com>
        (Google Transport Security);
        Mon, 06 Aug 2018 04:20:59 -0700 (PDT)
Received-SPF: pass (google.com: domain of andrey.vaskovskiy@targetprocess.com designates 209.85.220.65 as permitted sender) client-ip=209.85.220.65;
Authentication-Results: mx.google.com;
       dkim=pass header.i=@targetprocess-com.20150623.gappssmtp.com header.s=20150623 header.b=HHj15PNr;
       spf=pass (google.com: domain of andrey.vaskovskiy@targetprocess.com designates 209.85.220.65 as permitted sender) smtp.mailfrom=andrey.vaskovskiy@targetprocess.com
DKIM-Signature: v=1; a=rsa-sha256; c=relaxed/relaxed;
        d=targetprocess-com.20150623.gappssmtp.com; s=20150623;
        h=from:mime-version:subject:message-id:date:to;
        bh=fOkMElT7ATlyQbct/4tBdf1QutEoN0tbL3SrMjgIFPc=;
        b=HHj15PNrbola49XFWYMI+oepKtgPeyAQEO62XESLc6X2Vz1PbdyRmpplrF372i3uiz
         cIylBZJmj5zw6S+7+Y4nZYD9WPVuxpQDzpy53bNWHX6Vgmds7KDVXcueaf0WLpPVh7rC
         HPUjTyafQKdSuHYAG/mHoj1UD8foTW7KZ+8oGRof6S+bCvshD4IZdksgCtMOcO09Yh5w
         uztTG7w3v3I11R1VtfMhvuA7eT+d1cCAKNAO9te/bzM2wuQOSXptXeKbiIOMtKnXtQ7b
         A32+YAfkc2sSsVu99l3m6dtAtruip/+t+H7Wj+6+3ee5XHVHGI2q0mmwDzQx5L+VfVpO
         fwjA==
X-Google-DKIM-Signature: v=1; a=rsa-sha256; c=relaxed/relaxed;
        d=1e100.net; s=20161025;
        h=x-gm-message-state:from:mime-version:subject:message-id:date:to;
        bh=fOkMElT7ATlyQbct/4tBdf1QutEoN0tbL3SrMjgIFPc=;
        b=q9iGWW69QDUGikNOK1FiAPeg8HqLE4aT6AX9E/5fhbJBCtpT+zPE7WzqLIjfZ2cYMP
         TLlmoL5hvEy/Beo982WH/4IAo3ePLjoOJnOeJGgYXRZgMOyhgk5xvWTcd4r6Yg4JZwxW
         +neuOqnTEdAcwHgyxoVFyJY0p/bLaJf9QGIWOmgbCdZl6ooMieKjdLEqeNkhe4AGOCQS
         hIeba8J+R7YNtb9D5kkQZNrEvDQUPslj05q5pt72tP9E0tyYYOCqUHxktaMpLqJE5A85
         sF1wg8ANizk0y+qcSuZOCIKW5xr2A4g38NoudA4irNXVzgRK26KLMPgn8ZIoJEAfHMtl
         EIsg==
X-Gm-Message-State: AOUpUlHUnDYZyAqqWw4cuessUsBNPvSjtpeKyQUOGNPyovMRlyhi4lUx
	PSZfqYGO6zcSb6/akoIuporAyDRwPLCVqQ==
X-Google-Smtp-Source: AAOMgpcqBKhnhK+f5hABE8i4HIrMXVew8n54gqzQijIGMwFB9/TLp3oTP7BQmaObbQ+w+RK9fh6cgw==
X-Received: by 2002:a19:a5c5:: with SMTP id o188-v6mr11782618lfe.149.1533554458649;
        Mon, 06 Aug 2018 04:20:58 -0700 (PDT)
Return-Path: <andrey.vaskovskiy@targetprocess.com>
Received: from powerusers-mbp.office.targetprocess.com ([212.98.161.211])
        by smtp.gmail.com with ESMTPSA id u4-v6sm2104516lfc.51.2018.08.06.04.20.57
        for <email.integr@gmail.com>
        (version=TLS1_2 cipher=ECDHE-RSA-AES128-GCM-SHA256 bits=128/128);
        Mon, 06 Aug 2018 04:20:58 -0700 (PDT)
From: Andrey Vaskovskiy <andrey.vaskovskiy@targetprocess.com>
Content-Type: multipart/mixed;
 boundary=""Apple-Mail=_5AF3D0CA-BBDB-4F78-B0D5-80162D1ACE55""
Mime-Version: 1.0 (Mac OS X Mail 10.3 \(3273\))
Subject: Text with attachment
Message-Id: <88C1634E-2EF7-4FDF-BD87-1B21028F8C9F@targetprocess.com>
Date: Mon, 6 Aug 2018 14:30:46 +0300
To: email.integr@gmail.com
X-Mailer: Apple Mail (2.3273)


--Apple-Mail=_5AF3D0CA-BBDB-4F78-B0D5-80162D1ACE55
Content-Transfer-Encoding: 7bit
Content-Type: text/plain;
	charset=us-ascii

Text 
--Apple-Mail=_5AF3D0CA-BBDB-4F78-B0D5-80162D1ACE55
Content-Disposition: attachment;
	filename=""Bugs by State (1).csv""
Content-Type: text/csv;
	x-unix-mode=0644;
	name=""Bugs by State (1).csv""
Content-Transfer-Encoding: 7bit

State,Count of Records
Open,1851
Planned,88
Reopen,14
In Dev,9
Coded,21
Testing,3
Tested,42

--Apple-Mail=_5AF3D0CA-BBDB-4F78-B0D5-80162D1ACE55
Content-Transfer-Encoding: quoted-printable
Content-Type: text/plain;
	charset=utf-8

=E2=80=94 Please do not remove this line! Targetprocess Ticket#5=

--Apple-Mail=_5AF3D0CA-BBDB-4F78-B0D5-80162D1ACE55--
")))
            {
                message.LoadMessage(stream);
            }

            message.Subject = string.IsNullOrEmpty(subject) ? message.Subject : subject;
            Context.EmailClient.Messages[uid] = message;

            var requesters = Context.Storage.Get<UserLite>().Where(x => x.UserType == UserType.Requester).ToList();
            var requester = requesters.FirstOrDefault(x => x.Email == message.From.Email);
            if (requester == null)
            {
                Context.UserRepository.Add(new UserLite { Email = message.From.Email, Id = EntityId.Next(), UserType = UserType.Requester });
            }
        }

        [Given("reply html message with attachment from mac with uid '$uid' has subject '$subject'")]
        public void SetSenderAddressAndSubjectForHtmlMessageWithAttachmentFromMac(string uid, string subject)
        {
            var message = new MailMessage();
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(@"Delivered-To: email.integr@gmail.com
Received: by 2002:a0d:e582:0:0:0:0:0 with SMTP id o124-v6csp2787955ywe;
        Wed, 1 Aug 2018 23:44:14 -0700 (PDT)
X-Received: by 2002:aa7:d7d0:: with SMTP id e16-v6mr1860919eds.82.1533192254487;
        Wed, 01 Aug 2018 23:44:14 -0700 (PDT)
ARC-Seal: i=1; a=rsa-sha256; t=1533192254; cv=none;
        d=google.com; s=arc-20160816;
        b=FTBQcXU+D4g1e3y7Rif2oZzX94S/ViYH5xh/9qLH9mcOzVTJpqX2IXIYpV0MUGT/X9
         e0P6auZ51JHM0hFvp/J8j5DwZHOgxn7DN/Endk2hp3y6mMY0rgCggs1f1uBBiZUK6AK2
         CXRFAGwzroWyt0fA1zU4ueOnEqjrVXRpuX9w0o28AItoWZG0f+2owLQBdeNa7pkK+GeN
         YYw/s068t1nBl5lNC+4ZaS5GFEW1Vugtob51p9VXUpZFU1fBMXgiUTFzhFtG26sjqqJh
         DjvpGrIkyegW3HnU0sWKGWgSyi025MLPh2Rs/EX82bz7Lik2pAUpOAd/QPQUCLApBqo7
         RTAg==
ARC-Message-Signature: i=1; a=rsa-sha256; c=relaxed/relaxed; d=google.com; s=arc-20160816;
        h=message-id:in-reply-to:to:references:date:subject:mime-version:from
         :dkim-signature:arc-authentication-results;
        bh=y/DGHRzqyG1SWkR8TWbD/UzdSL0yrdtc3oZO22SO07c=;
        b=Ywu7Tf1xUHEXNSF60/plbi+wNNRyJB2AOwKxqZl1DVvNYwr61twirHvTwx+Lewtwp1
         LWcAlec030U/VsAVn8eWCh8nBmkC8D33VuE5XtCtUSKSuU0JleUDsng+pYf0tt3p/qnw
         fGXpgmqDDSUOrYZKPYo+GW57huLnSMsPWQvSvbVhylYdJnWp3Gr3lpuK3TGvqHTtDVqu
         6Zc+BDg78jRMeg2MR8lmcXz6WVTaHZJ3lWTdlpEFmfh6zIJEb9mdfs2PjgWFJGByI+ld
         JA+/pI1vKa2HigoZBpHzO3HmYZICCD48eELbyzmB4t5KFW8/4OosFN7fdprPw18tgdRC
         7uMQ==
ARC-Authentication-Results: i=1; mx.google.com;
       dkim=pass header.i=@targetprocess-com.20150623.gappssmtp.com header.s=20150623 header.b=jFbvgP9D;
       spf=pass (google.com: domain of andrey.vaskovskiy@targetprocess.com designates 209.85.220.41 as permitted sender) smtp.mailfrom=andrey.vaskovskiy@targetprocess.com
Return-Path: <andrey.vaskovskiy@targetprocess.com>
Received: from mail-sor-f41.google.com (mail-sor-f41.google.com. [209.85.220.41])
        by mx.google.com with SMTPS id c12-v6sor334280edi.55.2018.08.01.23.44.14
        for <email.integr@gmail.com>
        (Google Transport Security);
        Wed, 01 Aug 2018 23:44:14 -0700 (PDT)
Received-SPF: pass (google.com: domain of andrey.vaskovskiy@targetprocess.com designates 209.85.220.41 as permitted sender) client-ip=209.85.220.41;
Authentication-Results: mx.google.com;
       dkim=pass header.i=@targetprocess-com.20150623.gappssmtp.com header.s=20150623 header.b=jFbvgP9D;
       spf=pass (google.com: domain of andrey.vaskovskiy@targetprocess.com designates 209.85.220.41 as permitted sender) smtp.mailfrom=andrey.vaskovskiy@targetprocess.com
DKIM-Signature: v=1; a=rsa-sha256; c=relaxed/relaxed;
        d=targetprocess-com.20150623.gappssmtp.com; s=20150623;
        h=from:mime-version:subject:date:references:to:in-reply-to:message-id;
        bh=y/DGHRzqyG1SWkR8TWbD/UzdSL0yrdtc3oZO22SO07c=;
        b=jFbvgP9DZoxRrrSMBy5Gg5fL93ox1C7s7g3eUOko9eGAv52TYnwEAltqNXjiyILNih
         y6odJbJsxafTOOTui81VwAsu5t0ftnaFJT7Wf8fcc7KBIXNqRBsaxMCFgNlgJE8Y/TDK
         eJRcxLhLtpnHRb7r0ABU2EhL+C/pGLF79J8DoDwKEWjTGYrF4QXUOmg8imfxhNgiz61b
         m99TXeiXIzbZV7pSMP/s4J2uajZa7SIRpPZUlRNzVXzQ0ROQEBcrz8zWSHgBXz9JjKJ1
         vaCv3QAcYDIrVsRstQbFl+9Lwg78WFumy5q2lxz4Q7mkj09sBvH2TvquNrO2t77e5UIh
         STZQ==
X-Google-DKIM-Signature: v=1; a=rsa-sha256; c=relaxed/relaxed;
        d=1e100.net; s=20161025;
        h=x-gm-message-state:from:mime-version:subject:date:references:to
         :in-reply-to:message-id;
        bh=y/DGHRzqyG1SWkR8TWbD/UzdSL0yrdtc3oZO22SO07c=;
        b=rOcfF7IgvMBcxMx0yUU86g7n+7bE1ibS2YAk3cvDwhW/qgoBNm2t0a+uDoSzCu1Bsn
         JArZzu9J7V/erW7QfWN5++jrlBRES6fKTU6U1hnzbG47s8oFDd//ADWcoPL82eVVhCb+
         5xJqJ4izEXq8eXbzfgqzdzkstWzRj9YYUoIsMy+l2+XcOTPtWetKE9dOYBKm3+lbwLAU
         eEStUhNzEAVQhlbKKW8W1HelS3BbFTgqRC9cpgyrzNpdVa6X67kOCGVU9wf9rOpFOHy6
         PCAI2d82xVZbvYwDlGnV8FPJ3Hl3tCKio0Cp4r5U4tJT+KJIiXskZMKnp67RcVxElLO7
         tGnA==
X-Gm-Message-State: AOUpUlEC00uS1DMmaQGzGdEvCsQZR1sOpIELYgI/tB2NU2AFRn7Z9JKf
	u195ln+5KxcVfiQ6vh00yNg/eKedrpI=
X-Google-Smtp-Source: AAOMgpenUxIlpQfy+L2FgM+6VpY2L5RznLpHVCDdg/gK7K1pDUILwrvM4pBRf7B199PSijZGu+ZrAg==
X-Received: by 2002:a50:8ca9:: with SMTP id q38-v6mr1890540edq.2.1533192253999;
        Wed, 01 Aug 2018 23:44:13 -0700 (PDT)
Return-Path: <andrey.vaskovskiy@targetprocess.com>
Received: from [192.168.10.104] (leased-line-60-236.telecom.by. [217.21.60.236])
        by smtp.gmail.com with ESMTPSA id t44-v6sm583375edd.96.2018.08.01.23.44.13
        for <email.integr@gmail.com>
        (version=TLS1_2 cipher=ECDHE-RSA-AES128-GCM-SHA256 bits=128/128);
        Wed, 01 Aug 2018 23:44:13 -0700 (PDT)
From: Andrey Vaskovskiy <andrey.vaskovskiy@targetprocess.com>
Mime-Version: 1.0 (Mac OS X Mail 10.3 \(3273\))
Subject: Re: New Comment added to Request #5 ""Test from Mac""
Date: Thu, 2 Aug 2018 09:53:53 +0300
References: <flAyC_TiSyGYHA_ahY5leQ@ismtpd0005p1lon1.sendgrid.net>
To: email.integr@gmail.com
In-Reply-To: <flAyC_TiSyGYHA_ahY5leQ@ismtpd0005p1lon1.sendgrid.net>
Message-Id: <9D8432A0-22A9-43B8-A5FB-7EAA9AD00940@targetprocess.com>
X-Mailer: Apple Mail (2.3273)
Content-Type: multipart/mixed;
	boundary=""Apple-Mail=_329359F1-F966-4C91-9743-6A6D2461D700""


--Apple-Mail=_329359F1-F966-4C91-9743-6A6D2461D700
Content-Type: multipart/alternative;
 boundary=""Apple-Mail=_79877418-78E2-4243-A071-4B74BA17796A""


--Apple-Mail=_79877418-78E2-4243-A071-4B74BA17796A
Content-Transfer-Encoding: 7bit
Content-Type: text/plain;
	charset=us-ascii

Bugs by State attachment file added
> On Aug 2, 2018, at 09:42, av@gmail.com wrote:
> 
> New Comment added to Request #5 ""Test from Mac"" in project Test project
> 
> Comment from Request
> Added at 09:42 AM by Administrator Administrator
> 
> 
> --- Please do not remove this line! Targetprocess Ticket#5 ---


--Apple-Mail=_79877418-78E2-4243-A071-4B74BA17796A
Content-Transfer-Encoding: 7bit
Content-Type: text/html;
	charset=us-ascii

<html><head><meta http-equiv=""Content-Type"" content=""text/html charset=us-ascii""></head><body style=""word-wrap: break-word; -webkit-nbsp-mode: space; -webkit-line-break: after-white-space;"" class=""""></body></html>
--Apple-Mail=_79877418-78E2-4243-A071-4B74BA17796A
Content-Transfer-Encoding: 7bit
Content-Type: text/html;
	charset=us-ascii

<html><head><meta http-equiv=""Content-Type"" content=""text/html charset=us-ascii""></head><body style=""word-wrap: break-word; -webkit-nbsp-mode: space; -webkit-line-break: after-white-space;"" class="""">Bugs by State attachment file added<br class=""""><div><blockquote type=""cite"" class=""""><div class="""">On Aug 2, 2018, at 09:42, <a href=""mailto:av@gmail.com"" class="""">av@gmail.com</a> wrote:</div><br class=""Apple-interchange-newline""><div class="""">

<title class="""">Targetprocess | Notification</title>

<div bottommargin=""0"" topmargin=""0"" style=""padding-left: 0;padding-top: 0; padding-right: 0; padding-bottom: 0;"" class="""">
    <table width=""100%"" align=""center"" bgcolor=""#ffffff"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""width: 100%; margin-left:0; margin-right:0; margin-top: 0; margin-bottom: 0;font-family: Verdana,sans-serif; font-weight: 400; font-style: normal; font-size: 14px;"" class="""">
        <tbody class="""">
            <tr class="""">
                <td align=""left"" valign=""top"" style=""margin: 0; padding-left:0; padding-right: 10%; padding-top: 13px; padding-bottom: 11px;"" class=""""><p style=""font-family: Verdana,sans-serif; font-style: normal; font-size: 14px; color: #333333; padding-bottom: 0; padding-top: 0; margin-top: 0; margin-bottom: 10px;"" class="""">
                        <b class="""">New Comment added to Request #5 ""Test from Mac""</b> in project <b class="""">Test project</b>                       
                         </p>
                    <div style=""font-family: Verdana,sans-serif; font-size: 14px;"" class="""">
                                                    <div class="""">Comment from Request</div>

                                            </div><p style=""font-family: Verdana,sans-serif; font-style: normal; font-size: 12px; color: #808080; padding-bottom: 0; padding-top: 0; margin-top: 5px; margin-bottom: 10px;"" class="""">
                        Added at 09:42 AM by Administrator Administrator    
                </p>
                </td>
            </tr>
            <tr class="""">
                <td align=""left"" valign=""top"" style=""padding-left:0; padding-right:0; padding-top: 15px; padding-bottom: 13px; border-top: 1px solid #e6e6e6; font-family: Verdana,sans-serif; font-weight: 400; font-style: normal; font-size: 12px;"" class="""">
                    <img width=""112"" height=""20"" src=""http://www.targetprocess.com/img/email/you-was-mentioned/logo.png"" alt=""Targetprocess.See.Change."" class=""""><p style=""font-family: Verdana,sans-serif; font-style: normal; font-size: 12px; color: #808080; padding-bottom: 0; padding-top: 0; margin-top: 10px; margin-bottom: 0;"" class="""">
                        --- Please do not remove this line! Targetprocess
 Ticket#5 ---
                    </p>
                </td>
            </tr>
        </tbody>
    </table>
</div>




</div></blockquote></div><br class=""""></body></html>
--Apple-Mail=_79877418-78E2-4243-A071-4B74BA17796A--

--Apple-Mail=_329359F1-F966-4C91-9743-6A6D2461D700
Content-Disposition: attachment;
	filename=""Bugs by State (1).csv""
Content-Type: text/csv;
	x-unix-mode=0644;
	name=""Bugs by State (1).csv""
Content-Transfer-Encoding: 7bit

State,Count of Records
Open,1851
Planned,88
Reopen,14
In Dev,9
Coded,21
Testing,3
Tested,42

--Apple-Mail=_329359F1-F966-4C91-9743-6A6D2461D700--
")))
            {
                message.LoadMessage(stream);
            }

            message.Subject = string.IsNullOrEmpty(subject) ? message.Subject : subject;
            Context.EmailClient.Messages[uid] = message;

            var requesters = Context.Storage.Get<UserLite>().Where(x => x.UserType == UserType.Requester).ToList();
            var requester = requesters.FirstOrDefault(x => x.Email == message.From.Email);
            if (requester == null)
            {
                Context.UserRepository.Add(new UserLite { Email = message.From.Email, Id = EntityId.Next(), UserType = UserType.Requester });
            }
        }

        [Given("message with uid '$uid' has attachment with mime '$mime' and sender address '$email'")]
        public void SetAttachmentSenderAndAddress(string uid, string mime, string email)
        {
            Context.EmailClient.Messages[uid] = new MailMessage
            {
                From = new EmailAddress(email),
                Subject = string.Empty,
                Attachments = { new Attachment(MimePart.Parse(Encoding.UTF8.GetBytes(mime))) }
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
                .Length
                .Should(Be.EqualTo(1), "Context.Transport.LocalQueue.GetMessages<EmailReceivedMessage>().Length.Should(Be.EqualTo(1))");
            Context.Transport.LocalQueue.GetMessages<EmailReceivedMessage>()
                .First()
                .Mail.MessageUidDto.UID.Should(Be.EqualTo(uid),
                    "Context.Transport.LocalQueue.GetMessages<EmailReceivedMessage>().First().Mail.MessageUidDto.UID.Should(Be.EqualTo(uid))");
        }

        [Then("message with subject '$subject' should be passed to process in TP")]
        public void CreateMessageCommant(string subject)
        {
            Context.Transport.TpQueue.GetMessages<CreateMessageCommand>()
                .Length
                .Should(Be.EqualTo(1), "Context.Transport.TpQueue.GetMessages<CreateMessageCommand>().Length.Should(Be.EqualTo(1))");
            Context.Transport.TpQueue.GetMessages<CreateMessageCommand>()
                .First().Dto.Subject.Should(Be.EqualTo(subject),
                    "Context.Transport.TpQueue.GetMessages<CreateMessageCommand>().First().Dto.Subject.Should(Be.EqualTo(subject))");
        }

        [Then("message with '$uid' with attachment '$fileName' of content type '$contentType' should be passed to process")]
        public void MessageShouldBeDownloadedWithAttachmentOfContentType(string uid, string fileName, string contentType)
        {
            Context.Transport.LocalQueue.GetMessages<EmailReceivedMessage>()
                .Length
                .Should(Be.EqualTo(1), "Context.Transport.LocalQueue.GetMessages<EmailReceivedMessage>().Length.Should(Be.EqualTo(1))");
            Context.Transport.LocalQueue.GetMessages<EmailReceivedMessage>()
                .First()
                .Mail.MessageUidDto.UID.Should(Be.EqualTo(uid),
                    "Context.Transport.LocalQueue.GetMessages<EmailReceivedMessage>().First().Mail.MessageUidDto.UID.Should(Be.EqualTo(uid))");
            Context.Transport.LocalQueue.GetMessages<EmailReceivedMessage>()
                .First()
                .Mail.EmailAttachments.First().FileName.Should(Be.EqualTo(fileName),
                    "Context.Transport.LocalQueue.GetMessages<EmailReceivedMessage>().First().Mail.EmailAttachments.First().FileName.Should(Be.EqualTo(fileName)");
            Context.Transport.LocalQueue.GetMessages<EmailReceivedMessage>()
                .First()
                .Mail.EmailAttachments.First().ContentType.Should(Be.EqualTo(contentType),
                    "Context.Transport.LocalQueue.GetMessages<EmailReceivedMessage>().First().Mail.EmailAttachments.First().ContentType.Should(Be.EqualTo(contentType)");
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
                .Length
                .Should(Be.EqualTo(0), "Context.Transport.LocalQueue.GetMessages<EmailReceivedMessage>().Length.Should(Be.EqualTo(0))");
        }

        private MessageDownloadContext Context => ObjectFactory.GetInstance<MessageDownloadContext>();
    }
}
