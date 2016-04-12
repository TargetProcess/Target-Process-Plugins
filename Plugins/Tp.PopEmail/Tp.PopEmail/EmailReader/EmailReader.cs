// 
// Copyright (c) 2005-2014 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Net.Mail;
using MailBee.Mime;
using NServiceBus;
using Tp.Integration.Common;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Plugin.Core.Attachments;
using Tp.PopEmailIntegration.Data;
using Tp.PopEmailIntegration.EmailReader.Client;
using Attachment = MailBee.Mime.Attachment;
using MailMessage = MailBee.Mime.MailMessage;
using MailPriority = MailBee.Mime.MailPriority;

namespace Tp.PopEmailIntegration.EmailReader
{
	public class EmailReader : IHandleMessages<EmailUidsRetrievedMessage>
	{
		private readonly IEmailClient _client;
		private readonly MessageUidRepository _messageUidRepository;
		private readonly ILocalBus _localBus;
		private readonly ProjectEmailProfile _profile;
		private readonly IActivityLogger _log;
		private readonly List<string> _processedUids;
		private int _index;

		public EmailReader(IStorageRepository storageRepository, ILocalBus localBus, IEmailClient client,
						   MessageUidRepository messageUidRepository, IActivityLogger log)
		{
			_client = client;
			_messageUidRepository = messageUidRepository;
			_localBus = localBus;
			_profile = storageRepository.GetProfile<ProjectEmailProfile>();
			_log = log;
			_processedUids = new List<string>();
		}

		public void Handle(EmailUidsRetrievedMessage message)
		{
			try
			{
				_log.InfoFormat("Downloading {0} messages from email server...", message.Uids.Count());
				_client.Connect();
				_client.Login();

				var count = 0;
				while (ReadNextEmailMessage(message.Uids))
				{
					count++;
				}
				_log.InfoFormat("Downloaded messages={0}", count);
			}
			catch (Exception ex)
			{
				_client.Disconnect();
				_log.Error(String.Format("Error downloading messages."), ex);
				var skippedUids = message.Uids.Where(x => !_processedUids.Contains(x));
				_messageUidRepository.Remove(skippedUids.ToArray());
			}
		}

		public bool ReadNextEmailMessage(string[] uids)
		{
			while (uids != null && _index < uids.Length)
			{
				var uid = uids[_index++];

				// 1. Download email message from the server.
				var mailBeeMailMessage = DoDownloadMailMessage(uid);

				if (mailBeeMailMessage == null)
				{
					continue;
				}

				// 2. Mark downloaded message as read, do not download it the next time.
				var messageUid = MarkAsRead(uid);

				// Copy MailBee message to System.Net.Mail message.
				System.Net.Mail.MailMessage mailMessage;
				try
				{
					mailMessage = ConvertToNetMailMessage(mailBeeMailMessage);
				}
				catch(Exception e)
				{
					//Just skip this message
					_log.Error(string.Format("Error parsing message."), e);
					return true;
				}

				// If incoming message has incorrect date MailBee may throw exception.
				var sentDate = SqlDateTime.MinValue.Value;
				try
				{
					if (mailBeeMailMessage.Date != DateTime.MinValue)
					{
						sentDate = mailBeeMailMessage.Date;
					}
				}
				catch
				{
				}

				var emailReceivedMessage = new EmailReceivedMessage { Mail = ConvertToDto(mailMessage, messageUid, sentDate) };
				_localBus.SendLocal(emailReceivedMessage);

				return true;
			}

			_client.Disconnect();

			return false;
		}

		private MailMessage DoDownloadMailMessage(string uid)
		{
			try
			{
				var message = _client.DownloadMessage(uid);

				//skip processing Delivery Status Notification message.
				if (_client.IsDsnMessage(message)) return null;

				// Make sure to disable throwing exception explicitly. Otherwise strange things might happen. See bug #5748 for details.
				// So please don't change this option unless you know what you are doing!!!
				message.ThrowExceptions = false;
				return message;
			}
			catch (EmailException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new EmailException("Error downloading email message", ex);
			}
		}

		private MessageUidDTO MarkAsRead(string uid)
		{
			var messageUid = new MessageUidDTO
			{
				MailServer = _profile.MailServer,
				MailLogin = _profile.Login,
				UID = uid
			};

			_processedUids.Add(messageUid.UID);
			return messageUid;
		}

		/// <summary>
		/// Converts MailBee mail message to .NET FX mail message.
		/// </summary>
		/// <param name="mailBeeMailMessage">MailBee mail message.</param>
		/// <returns>.NET FX mail message.</returns>
		private System.Net.Mail.MailMessage ConvertToNetMailMessage(MailMessage mailBeeMailMessage)
		{
			var mailMessage = new System.Net.Mail.MailMessage
			{
				Sender = new MailAddress(mailBeeMailMessage.From.AsString),
				Subject = mailBeeMailMessage.Subject?.Replace("\r\n", " ").Replace('\r', ' ').Replace('\n', ' ') ?? string.Empty,
				Body =
					mailBeeMailMessage.IsBodyAvail("text/html", false)
						? mailBeeMailMessage.BodyHtmlText
						: mailBeeMailMessage.BodyPlainText,
				IsBodyHtml = mailBeeMailMessage.IsBodyAvail("text/html", false),
				From = new MailAddress(mailBeeMailMessage.From.AsString),
			};

			foreach (var emailAddress in mailBeeMailMessage.To.Cast<EmailAddress>().Where(emailAddress => !mailMessage.To.TryAdd(emailAddress.AsString)))
			{
				_log.WarnFormat("To address \"{0}\" is not in a recognized format.", emailAddress.AsString);
			}
			foreach (var emailAddress in mailBeeMailMessage.ReplyTo.Cast<EmailAddress>().Where(emailAddress => !mailMessage.ReplyToList.TryAdd(emailAddress.AsString)))
			{
				_log.WarnFormat("ReplyTo address \"{0}\" is not in a recognized format.", emailAddress.AsString);
			}
			foreach (var emailAddress in mailBeeMailMessage.Cc.Cast<EmailAddress>().Where(emailAddress => !mailMessage.CC.TryAdd(emailAddress.AsString)))
			{
				_log.WarnFormat("Cc address \"{0}\" is not in a recognized format.", emailAddress.AsString);
			}
			foreach (var emailAddress in mailBeeMailMessage.Bcc.Cast<EmailAddress>().Where(emailAddress => !mailMessage.Bcc.TryAdd(emailAddress.AsString)))
			{
				_log.WarnFormat("Bcc address \"{0}\" is not in a recognized format.", emailAddress.AsString);
			}
			switch (mailBeeMailMessage.Priority)
			{
				case MailPriority.Low:
				case MailPriority.Lowest:
					mailMessage.Priority = System.Net.Mail.MailPriority.Low;
					break;
				case MailPriority.None:
				case MailPriority.Normal:
					mailMessage.Priority = System.Net.Mail.MailPriority.Normal;
					break;
				case MailPriority.High:
				case MailPriority.Highest:
					mailMessage.Priority = System.Net.Mail.MailPriority.High;
					break;
			}
			foreach (Attachment mailBeeAttachment in mailBeeMailMessage.Attachments)
			{
				if ((!mailBeeAttachment.IsFile && !mailBeeAttachment.IsInline) && !mailBeeAttachment.IsMessageInside)
				{
					continue;
				}

				var attachmentName = GetAttachmentName(mailBeeAttachment);

				var newAttachment = new System.Net.Mail.Attachment(
					new MemoryStream(mailBeeAttachment.GetData()),
					attachmentName,
					mailBeeAttachment.ContentType);

				if (mailBeeAttachment.IsInline && !String.IsNullOrEmpty(mailBeeAttachment.ContentID))
				{
					newAttachment.Name = mailBeeAttachment.ContentID;
				}
				mailMessage.Attachments.Add(newAttachment);
			}
			return mailMessage;
		}

		private static EmailMessage ConvertToDto(System.Net.Mail.MailMessage message, MessageUidDTO messageUidDto,
												 DateTime sentDate)
		{
			var emailMessageDto = new EmailMessage
			{
				FromAddress = message.From.Address,
				FromDisplayName = message.From.DisplayName,
				Recipients = String.Join("; ", message.To.Select(address => address.Address).ToArray()),
				CC = message.CC.Select(address => new MailAddressLite { Address = address.Address, DisplayName = address.DisplayName }).ToList(),
				ReplyTo = message.ReplyToList.Select(address => new MailAddressLite { Address = address.Address, DisplayName = address.DisplayName }).ToList(),
				Subject = message.Subject,
				ContentType = ContentTypeEnum.Email,
				Body = message.Body,
				SendDate = sentDate,
				MessageUidDto = messageUidDto
			};

			foreach (var attachment in message.Attachments)
			{
				var fileId = AttachmentFolder.Save(attachment.ContentStream);
				emailMessageDto.EmailAttachments.Add(new LocalStoredAttachment { FileId = fileId, FileName = attachment.Name });
			}

			return emailMessageDto;
		}

		private static string GetAttachmentName(Attachment mailBeeAttachment)
		{
			if (String.IsNullOrEmpty(mailBeeAttachment.FilenameOriginal))
			{
				return String.IsNullOrEmpty(mailBeeAttachment.Name) ? mailBeeAttachment.Filename : mailBeeAttachment.Name;
			}

			return mailBeeAttachment.FilenameOriginal;
		}
	}

	[Serializable]
	public class EmailUidsRetrievedMessage : IPluginLocalMessage
	{
		public string[] Uids { get; set; }
	}

	public static class MailAddressCollectionExtensions
	{
		public static bool TryAdd(this MailAddressCollection mailAddressCollection, string address)
		{
			try
			{
				var mailAddress = new MailAddress(address);
				mailAddressCollection.Add(mailAddress);
				return true;
			}
			catch
			{
			}
			return false;
		}
	}
}
