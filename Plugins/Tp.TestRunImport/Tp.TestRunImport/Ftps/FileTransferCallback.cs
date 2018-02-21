using AlexPilotti.FTPS.Client;

namespace Tp.Integration.Plugin.TestRunImport.Ftps
{
    internal delegate void FileTransferCallback(FtpsClient sender, ETransferActions action, string localObjectName, string remoteObjectName, ulong fileTransmittedBytes, ulong? fileTransferSize, ref bool cancel);
}