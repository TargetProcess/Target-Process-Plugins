// 
// Copyright (c) 2005-2019 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Tp.Bugzilla.ConnectionValidators;
using Tp.Core;
using Tp.Integration.Plugin.Common.Validation;

namespace Tp.Bugzilla
{
    internal class ServerCertificateValidator : IDisposable
    {
        public static readonly ServerCertificateValidator Instance = new ServerCertificateValidator();

        private readonly ConcurrentDictionary<WebRequest, PluginProfileErrorCollection> _state;

        private ServerCertificateValidator()
        {
            _state = new ConcurrentDictionary<WebRequest, PluginProfileErrorCollection>();
            ServicePointManager.ServerCertificateValidationCallback += ValidateRemoteCertificate;
        }

        public IDisposable Register(WebRequest request, PluginProfileErrorCollection errors)
        {
            _state.TryAdd(request, errors);
            return Disposable.Create(() =>
            {
                PluginProfileErrorCollection _;
                _state.TryRemove(request, out _);
            });
        }

        public void Dispose()
        {
            ServicePointManager.ServerCertificateValidationCallback -= ValidateRemoteCertificate;
        }

        private bool ValidateRemoteCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            var request = sender as WebRequest;
            if (request == null)
            {
                //logging
                return true;
            }

            if (!_state.TryGetValue(request, out var errors))
            {
                //logging
                return true;
            }
            // If the certificate is a valid, signed certificate, return true.
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }
            // If there are errors in the certificate chain, look at each error to determine the cause.
            if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateChainErrors) != 0)
            {
                if (chain != null)
                {
                    foreach (X509ChainStatus status in chain.ChainStatus)
                    {
                        if (((certificate.Subject == certificate.Issuer) &&
                            (status.Status == X509ChainStatusFlags.UntrustedRoot)) || (status.Status == X509ChainStatusFlags.PartialChain))
                        {
                            // Self-signed certificates with an untrusted root are valid.
                            continue;
                        }
                        if (status.Status != X509ChainStatusFlags.NoError)
                        {
                            errors.Add(new PluginProfileError
                            {
                                FieldName = BugzillaProfile.UrlField,
                                Message = status.StatusInformation,
                                AdditionalInfo = ValidationErrorType.TrustFailure.ToString()
                            });
                            // If there are any other errors in the certificate chain, the certificate is invalid,
                            // so the method returns false.
                            return false;
                        }
                    }
                }
                // When processing reaches this line, the only errors in the certificate chain are
                // untrusted root errors for self-signed certificates. These certificates are valid
                // for default Exchange Server installations, so return true.
                return true;
            }
            // In all other cases, return false.
            return false;
        }
    };

    internal class TpWebClient : WebClient
    {
        private readonly PluginProfileErrorCollection _errors;
        private IDisposable _subscription;

        public TpWebClient(PluginProfileErrorCollection errors)
        {
            _errors = errors;
        }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest w = base.GetWebRequest(uri);
            w.Timeout = 30 * 60 * 1000;
            _subscription = ServerCertificateValidator.Instance.Register(w, _errors);
            return w;
        }

        protected override void Dispose(bool disposing)
        {
            _subscription?.Dispose();
            base.Dispose(disposing);
        }
    }
}
