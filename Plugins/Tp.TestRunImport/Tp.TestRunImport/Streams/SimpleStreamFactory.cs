// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Security;
using Tp.Integration.Plugin.Common.Activity;

namespace Tp.Integration.Plugin.TestRunImport.Streams
{
	public class SimpleStreamFactory : IStreamFactory
	{
		private readonly IActivityLogger _log;

		public SimpleStreamFactory(IActivityLogger log)
		{
			_log = log;
		}

		public StreamFactoryResult OpenStream(Uri uri, bool passiveMode)
		{
			return OpenStreamIfModified(uri, new LastModifyResult(), passiveMode);
		}

		public StreamFactoryResult OpenStreamIfModified(Uri uri, LastModifyResult lastModifyResult, bool passiveMode)
		{
			if (uri == null)
			{
				_log.Error("OpenStreamIfModified method uri member is null");
				throw new ArgumentNullException("uri");
			}
			try
			{
				return OpenStreamIfModified(WebRequest.Create(uri), lastModifyResult, passiveMode);
			}
			catch (UriFormatException ex)
			{
				_log.ErrorFormat("Specified path has invalid format: {0}", ex.Message);
				throw new ApplicationException(string.Format("Specified path has invalid format: {0}", ex.Message), ex);
			}
			catch (NotSupportedException ex)
			{
				_log.ErrorFormat(string.Format("Specified path has invalid format: {0}", ex.Message), ex);
				throw new ApplicationException(
					string.Format("The request scheme specified in requestUri is not registered: {0}", ex.Message), ex);
			}
			catch (SecurityException ex)
			{
				_log.ErrorFormat("The caller does not have permission to connect to the requested URI or a URI that the request is redirected to: {0}", ex.Message);
				throw new ApplicationException(
					string.Format(
						"The caller does not have permission to connect to the requested URI or a URI that the request is redirected to: {0}",
						ex.Message), ex);
			}
		}

		private static StreamFactoryResult OpenStreamIfModified(WebRequest request, LastModifyResult lastModifyResult,
																bool passiveMode)
		{
			if (request is FileWebRequest)
			{
				return OpenStreamIfModified((FileWebRequest)request, lastModifyResult);
			}
			if (request is HttpWebRequest)
			{
				return OpenStreamIfModified((HttpWebRequest)request, lastModifyResult);
			}
			if (request is FtpWebRequest)
			{
				var ftpWebRequest = (FtpWebRequest)request;
				ftpWebRequest.UsePassive = passiveMode;
				return OpenStreamIfModified(ftpWebRequest, lastModifyResult);
			}
			throw new ApplicationException(string.Format("Unsupported resource \"{0}\"", request.RequestUri));
		}

		private static StreamFactoryResult OpenStreamIfModified(FileWebRequest request, LastModifyResult lastModifyResult)
		{
			var path = request.RequestUri.LocalPath;
			var fileInfo = new FileInfo(path);
			if (!fileInfo.Exists)
			{
				throw new ApplicationException(string.Format("File \"{0}\" does not exist", path));
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
				using (var response = (HttpWebResponse)request.GetResponse())
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
								ModifyTimeUtcsTicks = response.Headers[HttpResponseHeader.LastModified] == null ? DateTime.MinValue.Ticks : response.LastModified.ToUniversalTime().Ticks,
								ETagHeader = response.Headers["ETag"] ?? string.Empty
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
			using (var dateTimestampResponse = (FtpWebResponse)request.GetResponse())
			{
				if (!HasChanged(new DateTime(lastModifyResult.ModifyTimeUtcsTicks), dateTimestampResponse.LastModified)) return null;

				var ftpWebRequest = (FtpWebRequest)WebRequest.Create(request.RequestUri);
				ftpWebRequest.UsePassive = request.UsePassive;
				ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;
				using (var downloadFileResponse = (FtpWebResponse)ftpWebRequest.GetResponse())
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
								LastModifyResult = new LastModifyResult { ModifyTimeUtcsTicks = dateTimestampResponse.LastModified.ToUniversalTime().Ticks }
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