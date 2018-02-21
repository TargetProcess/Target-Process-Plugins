using System;
using Tp.Core;

namespace Tp.Integration.Messages.Ticker
{
    [Serializable]
    public class LastSyncDate
    {
        public LastSyncDate()
        {
            Value = CurrentDate.Value;
        }

        public LastSyncDate(DateTime lastSyncDate)
        {
            Value = lastSyncDate;
        }

        public DateTime Value { get; set; }
    }
}
