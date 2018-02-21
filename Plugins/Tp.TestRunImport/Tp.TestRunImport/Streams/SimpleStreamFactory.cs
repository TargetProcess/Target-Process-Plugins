// 
// Copyright (c) 2005-2016 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Net.Security;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using AlexPilotti.FTPS.Client;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.TestRunImport.Ftps;

namespace Tp.Integration.Plugin.TestRunImport.Streams
{
    public class SimpleStreamFactory : IStreamFactory
    {
        private readonly IActivityLogger _log;

        public SimpleStreamFactory(IActivityLogger log)
        {
            _log = log;
        }

        public StreamFactoryResult OpenStream(Uri uri, TestRunImportPluginProfile settings)
        {
            return OpenStreamIfModified(uri, settings, new LastModifyResult());
        }

        private static bool ValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            const SslPolicyErrors ignoredErrors =
                SslPolicyErrors.RemoteCertificateChainErrors | SslPolicyErrors.RemoteCertificateNameMismatch;

            return (sslPolicyErrors & ~ignoredErrors) == SslPolicyErrors.None;
        }

        public StreamFactoryResult OpenStreamIfModified(Uri uri, TestRunImportPluginProfile settings, LastModifyResult lastModifyResult)
        {
            if (uri == null)
            {
                _log.Error("OpenStreamIfModified method uri member is null");
                throw new ArgumentNullException(nameof(uri));
            }
            try
            {
                if (string.Compare("ftps", uri.Scheme, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return OpenFtpsStreamIfModified(uri, settings, lastModifyResult);
                }
                return OpenWebRequestStreamIfModified(WebRequest.Create(uri), settings, lastModifyResult);
            }
            catch (UriFormatException ex)
            {
                _log.ErrorFormat("Specified path has invalid format: {0}", ex.Message);
                throw new ApplicationException($"Specified path has invalid format: {ex.Message}", ex);
            }
            catch (NotSupportedException ex)
            {
                _log.ErrorFormat($"Specified path has invalid format: {ex.Message}", ex);
                throw new ApplicationException(
                    $"The request scheme specified in requestUri is not registered: {ex.Message}", ex);
            }
            catch (SecurityException ex)
            {
                _log.ErrorFormat(
                    "The caller does not have permission to connect to the requested URI or a URI that the request is redirected to: {0}",
                    ex.Message);
                throw new ApplicationException(
                    $"The caller does not have permission to connect to the requested URI or a URI that the request is redirected to: {ex.Message}",
                    ex);
            }
        }

        private static StreamFactoryResult OpenWebRequestStreamIfModified(WebRequest request, TestRunImportPluginProfile settings,
            LastModifyResult lastModifyResult)
        {
            if (request is FileWebRequest)
            {
                return OpenStreamIfModified((FileWebRequest) request, lastModifyResult);
            }
            if (request is HttpWebRequest)
            {
                return OpenStreamIfModified((HttpWebRequest) request, lastModifyResult);
            }
            if (request is FtpWebRequest)
            {
                var ftpWebRequest = (FtpWebRequest) request;
                ftpWebRequest.UsePassive = settings.PassiveMode;
                return OpenStreamIfModified(ftpWebRequest, lastModifyResult);
            }
            throw new ApplicationException($"Unsupported resource \"{request.RequestUri}\"");
        }

        private StreamFactoryResult OpenFtpsStreamIfModified(Uri uri, TestRunImportPluginProfile settings,
            LastModifyResult lastModifyResult)
        {
            if (string.Compare("ftps", uri.Scheme, StringComparison.OrdinalIgnoreCase) == 0)
            {
                using (var client = new FtpsClient())
                {
                    client.LogCommand += (sender, args) =>
                    {
                        _log.Info(args.CommandText);
                    };

                    NetworkCredential networkCredential = null;

                    if (!string.IsNullOrEmpty(uri.UserInfo))
                    {
                        var userInfo = uri.UserInfo;
                        var username = userInfo;
                        var password = string.Empty;
                        var index = userInfo.IndexOf(':');
                        if (index != -1)
                        {
                            username = Uri.UnescapeDataString(userInfo.Substring(0, index));
                            index++; // skip ':'
                            password = Uri.UnescapeDataString(userInfo.Substring(index, userInfo.Length - index));
                        }
                        networkCredential = new NetworkCredential(username, password);
                    }

                    client.Connect(uri.DnsSafeHost, uri.Port,
                        networkCredential,
                        ESSLSupportMode.Implicit,
                        ValidateCertificate,
                        null, 0, 0, 0, null, false, settings.PassiveMode ? EDataConnectionMode.Passive : EDataConnectionMode.Active);

                    var fileModificationTime = client.GetFileModificationTime(uri.AbsolutePath);

                    if (fileModificationTime.HasValue
                        && HasChanged(new DateTime(lastModifyResult.ModifyTimeUtcsTicks), fileModificationTime.Value))
                    {
                        using (Stream ftpStream = client.GetFile(uri.AbsolutePath))
                        {
                            var stream = new MemoryStream();
                            CopyTo(ftpStream, stream);
                            stream.Seek(0, SeekOrigin.Begin);

                            return new StreamFactoryResult
                            {
                                Stream = stream,
                                LastModifyResult =
                                    new LastModifyResult { ModifyTimeUtcsTicks = fileModificationTime.Value.ToUniversalTime().Ticks }
                            };
                        }
                    }
                    return null;
                }
            }
            throw new ApplicationException($"Unsupported FTPS resource \"{uri}\"");
        }

        private static StreamFactoryResult OpenStreamIfModified(FileWebRequest request, LastModifyResult lastModifyResult)
        {
            var path = request.RequestUri.LocalPath;
            var fileInfo = new FileInfo(path);
            if (!fileInfo.Exists)
            {
                throw new ApplicationException($"File \"{path}\" does not exist");
            }
            return HasChanged(new DateTime(lastModifyResult.ModifyTimeUtcsTicks), fileInfo.LastWriteTimeUtc)
                ? new StreamFactoryResult
                {
                    Stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read),
                    LastModifyResult = new LastModifyResult { ModifyTimeUtcsTicks = fileInfo.LastWriteTimeUtc.ToUniversalTime().Ticks }
                }
                : null;
        }

        private static StreamFactoryResult OpenStreamIfModified(HttpWebRequest request, LastModifyResult lastModifyResult)
        {
            request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
            if (!string.IsNullOrEmpty(lastModifyResult.ETagHeader))
            {
                request.Headers[HttpRequestHeader.IfNoneMatch] = lastModifyResult.ETagHeader;
            }
            else if (lastModifyResult.ModifyTimeUtcsTicks != DateTime.MinValue.Ticks)
            {
                request.IfModifiedSince = new DateTime(lastModifyResult.ModifyTimeUtcsTicks);
            }
            try
            {
                using (var response = (HttpWebResponse) request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.NotModified) return null;
                    {
                        var stream = new MemoryStream();
                        CopyTo(response.GetResponseStream(), stream);
                        stream.Seek(0, SeekOrigin.Begin);

                        return new StreamFactoryResult
                        {
                            Stream = stream,
                            LastModifyResult = new LastModifyResult
                            {
                                ModifyTimeUtcsTicks =
                                    response.Headers[HttpResponseHeader.LastModified] == null
                                        ? DateTime.MinValue.Ticks
                                        : response.LastModified.ToUniversalTime().Ticks,
                                ETagHeader = response.Headers[HttpResponseHeader.ETag] ?? string.Empty
                            }
                        };
                    }
                }
            }
            catch (WebException wex)
            {
                var httpResponse = wex.Response as HttpWebResponse;
                if (httpResponse != null && httpResponse.StatusCode == HttpStatusCode.NotModified)
                {
                    // resource was not modified.
                    return null;
                }
                // Something else happened. Rethrow or log.
                throw;
            }
        }

        private static StreamFactoryResult OpenStreamIfModified(FtpWebRequest request, LastModifyResult lastModifyResult)
        {
            request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
            request.Method = WebRequestMethods.Ftp.GetDateTimestamp;
            using (var dateTimestampResponse = (FtpWebResponse) request.GetResponse())
            {
                if (!HasChanged(new DateTime(lastModifyResult.ModifyTimeUtcsTicks), dateTimestampResponse.LastModified)) return null;

                var ftpWebRequest = (FtpWebRequest) WebRequest.Create(request.RequestUri);
                ftpWebRequest.UsePassive = request.UsePassive;
                ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                using (var downloadFileResponse = (FtpWebResponse) ftpWebRequest.GetResponse())
                {
                    var stream = new MemoryStream();
                    using (var responseStream = downloadFileResponse.GetResponseStream())
                    {
                        CopyTo(responseStream, stream);
                    }
                    stream.Seek(0, SeekOrigin.Begin);

                    return new StreamFactoryResult
                    {
                        Stream = stream,
                        LastModifyResult =
                            new LastModifyResult { ModifyTimeUtcsTicks = dateTimestampResponse.LastModified.ToUniversalTime().Ticks }
                    };
                }
            }
        }

        private static bool HasChanged(DateTime lastModifyTimeUtc, DateTime resourseDate)
        {
            if (lastModifyTimeUtc == DateTime.MinValue || resourseDate == DateTime.MinValue)
            {
                return true;
            }
            return lastModifyTimeUtc < resourseDate.ToUniversalTime();
        }

        public static void CopyTo(Stream input, Stream output)
        {
            var buffer = new byte[16 * 1024];
            int bytesRead;

            while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, bytesRead);
            }
        }
    }
}
