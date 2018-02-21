// 
// Copyright (c) 2005-2016 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Net.Security;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using AlexPilotti.FTPS.Client;
using AlexPilotti.FTPS.Common;
using StructureMap;
using Tp.Integration.Messages.Ticker;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Validation;
using Tp.Integration.Plugin.TestRunImport.Ftps;
using Tp.Integration.Plugin.TestRunImport.Streams;
using Tp.Integration.Plugin.TestRunImport.TestRunImport;

namespace Tp.Integration.Plugin.TestRunImport
{
    [Profile]
    [DataContract]
    public class TestRunImportPluginProfile : TestRunImportSettings, ISynchronizableProfile, IValidatable
    {
        public const string PatternTestIdGroupName = "testId";
        public const string PatternTestNameGroupName = "testName";

        public TestRunImportPluginProfile()
        {
            FrameworkType = FrameworkTypes.FrameworkTypes.None;
            SynchronizationInterval = 24;
        }

        #region ISynchronizableProfile members

        [DataMember]
        public int SynchronizationInterval { get; set; }

        #endregion

        public void Validate(PluginProfileErrorCollection errors)
        {
            if (FrameworkType == FrameworkTypes.FrameworkTypes.Selenium && PostResultsToRemoteUrl)
            {
                ValidateAuthTokenUserId(errors);
                ValidateRemoteResultsUrl(errors);
            }
            else
            {
                ValidateResultsFilePath(errors);
                ValidateIntegrationInterval(errors);
            }
            ValidateProject(errors);
            ValidateTestPlan(errors);
            ValidateFramework(errors);
            ValidateRegExp(errors);
        }

        public void ValidateMapperData(PluginProfileErrorCollection errors)
        {
            ValidateResultsFilePathForMapping(errors);
            ValidateProject(errors);
            ValidateTestPlan(errors);
            ValidateFramework(errors);
            ValidateRegExp(errors);
        }

        public void ValidateSeleniumUrlData(PluginProfileErrorCollection errors)
        {
            ValidateAuthTokenUserId(errors);
        }

        private void ValidateProject(PluginProfileErrorCollection errors)
        {
            if (Project <= 0)
            {
                errors.Add(new PluginProfileError { FieldName = "Project", Message = "Project should be specified" });
            }
        }

        private void ValidateAuthTokenUserId(PluginProfileErrorCollection errors)
        {
            if (AuthTokenUserId <= 0)
            {
                errors.Add(new PluginProfileError { FieldName = "AuthTokenUserId", Message = "User for authentication should be specified" });
            }
        }

        private void ValidateTestPlan(PluginProfileErrorCollection errors)
        {
            if (TestPlan <= 0)
            {
                errors.Add(new PluginProfileError { FieldName = "TestPlan", Message = "Test Plan should be specified" });
            }
        }

        private void ValidateFramework(PluginProfileErrorCollection errors)
        {
            if (FrameworkType == FrameworkTypes.FrameworkTypes.None)
            {
                errors.Add(new PluginProfileError { FieldName = "FrameworkType", Message = "Framework type should be specified" });
            }
        }

        private void ValidateRemoteResultsUrl(PluginProfileErrorCollection errors)
        {
            if (string.IsNullOrEmpty(RemoteResultsUrl) || string.IsNullOrEmpty(RemoteResultsUrl.Trim()))
            {
                errors.Add(new PluginProfileError { FieldName = "RemoteResultsUrl", Message = string.Empty });
            }
        }

        private void ValidateResultsFilePath(PluginProfileErrorCollection errors)
        {
            ValidateResultsFilePathForMapping(errors, false);
        }

        private static bool ValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            const SslPolicyErrors ignoredErrors =
                SslPolicyErrors.RemoteCertificateChainErrors | SslPolicyErrors.RemoteCertificateNameMismatch;

            return (sslPolicyErrors & ~ignoredErrors) == SslPolicyErrors.None;
        }

        private void ValidateResultsFilePathForMapping(PluginProfileErrorCollection errors, bool checkFileExists = true)
        {
            if (string.IsNullOrEmpty(ResultsFilePath) || string.IsNullOrEmpty(ResultsFilePath.Trim()))
            {
                errors.Add(new PluginProfileError
                {
                    FieldName = "ResultsFilePath",
                    Message = "Test result XML file path should be specified"
                });
            }
            else
            {
                try
                {
                    ResultsFilePath = ResultsFilePath.Trim();
                    var uri = new Uri(ResultsFilePath);

                    if (string.Compare("ftps", uri.Scheme, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        using (var client = new FtpsClient())
                        {
                            NetworkCredential networkCredential = null;

                            if (!string.IsNullOrEmpty(uri.UserInfo))
                            {
                                var userInfo = uri.UserInfo;
                                var username = userInfo;
                                var password = "";
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
                                null, 0, 0, 0, null, false, PassiveMode ? EDataConnectionMode.Passive : EDataConnectionMode.Active);

                            var fileSize = client.GetFileTransferSize(uri.AbsolutePath);

                            if (!fileSize.HasValue || fileSize.Value == 0)
                            {
                                errors.Add(new PluginProfileError
                                {
                                    FieldName = "ResultsFilePath",
                                    Message = $"The specified file {(!fileSize.HasValue ? "not found" : "is empty")}"
                                });
                                return;
                            }

                            using (var ftpStream = client.GetFile(uri.AbsolutePath))
                            {
                                SimpleStreamFactory.CopyTo(ftpStream, new MemoryStream());
                                ftpStream.Close();
                            }
                        }
                    }
                    else
                    {
                        var request = WebRequest.Create(uri);

                        if (request is FileWebRequest)
                        {
                            var path = request.RequestUri.LocalPath;
                            var fileInfo = new FileInfo(path);
                            if (checkFileExists && !fileInfo.Exists)
                            {
                                errors.Add(new PluginProfileError
                                {
                                    FieldName = "ResultsFilePath",
                                    Message = $"File \"{path}\" does not exist"
                                });
                            }
                        }
                        else if (request is HttpWebRequest)
                        {
                            if (checkFileExists)
                            {
                                request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
                                var response = (HttpWebResponse) request.GetResponse();
                                response.Close();
                            }
                        }
                        else if (request is FtpWebRequest)
                        {
                            if (checkFileExists)
                            {
                                var ftpWebRequest = (FtpWebRequest) request;
                                ftpWebRequest.UsePassive = PassiveMode;
                                ftpWebRequest.Method = WebRequestMethods.Ftp.GetFileSize;
                                ftpWebRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);

                                using (var fileSizeResponse = ftpWebRequest.GetResponse())
                                {
                                    if (fileSizeResponse.ContentLength <= 0)
                                    {
                                        errors.Add(new PluginProfileError
                                        {
                                            FieldName = "ResultsFilePath",
                                            Message = "The specified file is empty"
                                        });
                                        return;
                                    }

                                    var ftpRequest = (FtpWebRequest) WebRequest.Create(ftpWebRequest.RequestUri);
                                    ftpRequest.UsePassive = ftpWebRequest.UsePassive;
                                    ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                                    using (var downloadFileResponse = ftpRequest.GetResponse())
                                    {
                                        using (var responseStream = downloadFileResponse.GetResponseStream())
                                        {
                                            SimpleStreamFactory.CopyTo(responseStream, new MemoryStream());
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            errors.Add(new PluginProfileError
                            {
                                FieldName = "ResultsFilePath",
                                Message = $"Unsupported resource \"{request.RequestUri}\""
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    errors.Add(new PluginProfileError
                    {
                        FieldName = "ResultsFilePath",
                        Message = ex.Message
                    });
                }
            }
        }

        private void ValidateIntegrationInterval(PluginProfileErrorCollection errors)
        {
            if (SynchronizationInterval <= 0)
            {
                errors.Add(new PluginProfileError
                {
                    FieldName = "SynchronizationInterval",
                    Message = "Integration interval should be an integer greater than zero"
                });
            }
            SynchronizationInterval = SynchronizationInterval * 60;
        }

        private void ValidateRegExp(PluginProfileErrorCollection errors)
        {
            if (RegExp == null) return;
            RegExp = RegExp.Trim();
            if (RegExp == string.Empty)
            {
                RegExp = null;
            }
            else
            {
                try
                {
                    var rx = new Regex(RegExp);
                    var names = rx.GetGroupNames();
                    var s1 = Array.Find(names, s => s == PatternTestIdGroupName);
                    var s2 = Array.Find(names, s => s == PatternTestNameGroupName);
                    if ((s1 == null && s2 == null) || (s1 != null && s2 != null))
                    {
                        errors.Add(new PluginProfileError
                        {
                            FieldName = "RegExp",
                            Message =
                                $"Regular expression must define either &lt;{PatternTestIdGroupName}&gt; or &lt;{PatternTestNameGroupName}&gt; group"
                        });
                    }
                }
                catch (ArgumentException ex)
                {
                    errors.Add(new PluginProfileError { FieldName = "RegExp", Message = $"Cannot parse regular expression: {ex.Message}" });
                }
            }
        }
    }
}
