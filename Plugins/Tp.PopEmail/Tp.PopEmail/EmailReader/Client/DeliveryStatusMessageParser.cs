//
// Copyright (c) 2005-2016 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
//

using System;
using System.IO;
using MailBee.BounceMail;
using MailBee.Mime;

namespace Tp.PopEmailIntegration.EmailReader.Client
{
    public class DeliveryStatusMessageParser : IDeliveryStatusMessageParser
    {
        private static readonly DeliveryStatusParser _deliveryStatusParser;

        static DeliveryStatusMessageParser()
        {
            _deliveryStatusParser = new DeliveryStatusParser(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"BounceDatabase\all.xml"), true);
        }

        public bool IsDsnMessage(MailMessage message)
        {
            var result = _deliveryStatusParser.Process(message);
            return result != null;
        }
    }
}
