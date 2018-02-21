// 
// Copyright (c) 2005-2016 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using MailBee.Mime;
using NUnit.Framework;
using Tp.PopEmailIntegration.EmailReader.Client;
using Tp.Testing.Common.NUnit;

namespace Tp.PopEmailIntegration.BusinessScenarios.DeliveryStatusMessageParserFeature
{
    [TestFixture]
    [Category("PartPlugins1")]
    public class DeliveryStatusMessageParserTests
    {
        [Test]
        public void ShouldNotTreatMessageFromAdministratorWithKeywordFailedInBodyAsDsn()
        {
            var message = new MailMessage
            {
                Subject = "Test subject",
                BodyPlainText = @"My poor little kitten was so tired that failed to walk.",
                From = new EmailAddress
                {
                    AsString = "\"Nadia administrator\" <nadia.administrator@targetprocess.com>",
                    DisplayName = "Nadia administrator",
                    Email = "nadia.administrator@targetprocess.com"
                }
            };

            new DeliveryStatusMessageParser().IsDsnMessage(message)
                .Should(Be.False, "Should not treat message with failed keyword in body as a Delivery Status Notification message");
        }

        [Test]
        public void ShouldNotTreatMessageFromAdminWithKeywordIsNotAvailableInBodyAsDsn()
        {
            var message = new MailMessage
            {
                Subject = "Test subject",
                BodyPlainText = @"Courses tab is not available in ET portal for any students.",
                From = new EmailAddress
                {
                    AsString = "\"Nadia admin\" <nadia.admin@targetprocess.com>",
                    DisplayName = "Nadia admin",
                    Email = "nadia.admin@targetprocess.com"
                }
            };

            new DeliveryStatusMessageParser().IsDsnMessage(message)
                .Should(Be.False, "Should not treat message with failed keyword in body as a Delivery Status Notification message");
        }
    }
}
