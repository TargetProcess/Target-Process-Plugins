using System;

namespace Tp.Integration.Messages.ServiceBus.Transport
{
    internal class MessageLabel
    {
        private readonly string _windowsIdentityName;
        private readonly string _idForCorrelation;

        private const string IDFORCORRELATION = "CorrId";
        private const string WINDOWSIDENTITYNAME = "WinIdName";

        public MessageLabel(string windowsIdentityName, string idForCorrelation)
        {
            _windowsIdentityName = windowsIdentityName;
            _idForCorrelation = idForCorrelation;
        }

        public string WindowsIdentityName
        {
            get { return _windowsIdentityName; }
        }

        public string IdForCorrelation
        {
            get { return _idForCorrelation; }
        }

        public override string ToString()
        {
            return String.Format("<{0}>{2}</{0}><{1}>{3}</{1}>", IDFORCORRELATION, WINDOWSIDENTITYNAME, IdForCorrelation,
                WindowsIdentityName);
        }

        public static MessageLabel Parse(string label)
        {
            return new MessageLabel(GetWindowsIdentityName(label), GetIdForCorrelation(label));
        }

        private static string GetWindowsIdentityName(string label)
        {
            if (string.IsNullOrEmpty(label) || !label.Contains(WINDOWSIDENTITYNAME)) return null;

            var winStartIndex = label.IndexOf(String.Format("<{0}>", WINDOWSIDENTITYNAME)) + WINDOWSIDENTITYNAME.Length + 2;
            var winCount = label.IndexOf(String.Format("</{0}>", WINDOWSIDENTITYNAME)) - winStartIndex;

            return label.Substring(winStartIndex, winCount);
        }

        private static string GetIdForCorrelation(string label)
        {
            if (string.IsNullOrEmpty(label) || !label.Contains(IDFORCORRELATION)) return null;

            var idStartIndex = label.IndexOf(String.Format("<{0}>", IDFORCORRELATION)) + IDFORCORRELATION.Length + 2;
            var idCount = label.IndexOf(String.Format("</{0}>", IDFORCORRELATION)) - idStartIndex;

            return label.Substring(idStartIndex, idCount);
        }
    }
}
