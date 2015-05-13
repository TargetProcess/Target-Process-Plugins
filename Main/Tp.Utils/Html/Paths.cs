// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using System.Text.RegularExpressions;
using System.Web;

namespace Tp.Utils.Html
{
	public class Paths
	{
		public static string MapImgVirtualPaths(string input)
		{
			try
			{
				return Regex.Replace(input, @"(<img)([^>]*src=['""])(&#126;&#47;|~/)([^""']*?['""][^>]*?>)",
				                     String.Format("$1 $2{0}$4", VirtualPathUtility.ToAbsolute("~/")),
				                     RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);
			}
			catch (HttpException)
			{
				return input;
			}
		}

		public static string MapImgVirtualPathsWrapLightBox(string input, bool customResize, bool displayImage)
		{
			try
			{
				if (String.IsNullOrEmpty(input))
					return input;
				return Regex.Replace(input, @"(<img)([^>]*src=['""])(&#126;&#47;|~/)([^""']*?['""][^>]*?>)",
				                     String.Format("$1 rel='{0}' style='display:{1}' $2{2}$4",
				                                   customResize ? "" : "mayNeedToResize", displayImage ? "block" : "none",
				                                   VirtualPathUtility.ToAbsolute("~/")),
				                     RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);
			}
			catch (HttpException)
			{
				return input;
			}
		}

		public static string MapImgVirtualPathsWrapLightBox(string input, bool displayImage)
		{
			return MapImgVirtualPathsWrapLightBox(input, false, displayImage);
		}

		public static string MapImgVirtualPathsWrapLightBox(string input)
		{
			return MapImgVirtualPathsWrapLightBox(input, false);
		}

		public static string MapImgVirtualPathsOld(string input)
		{
			try
			{
				var result = Regex.Replace(input, @"(<img[^>]*src=['""])(&#126;&#47;|~/)([^""']*?['""][^>]*?>)",
				                           "$1" + VirtualPathUtility.ToAbsolute("~/") + "$3",
				                           RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);

				return Regex.Replace(result, @"(<img)([^>]*src=['""])([^""']*)(['""][^>]*?>)",
				                     "<a id='aa_lnk' href=\"$3\">$1 id='a_thumb' $2$3$4</a>"); //style='dispaly:none'
			}
			catch (HttpException)
			{
				return input;
			}
		}

		public static string VirtualizeImgMappedPaths(string input)
		{
			try
			{
				return Regex.Replace(input,
				                     @"(<img[^>]*src=['""])(" + VirtualPathUtility.ToAbsolute("~/") +
				                     @")([^""']*?['""][^>]*?>)",
				                     "$1" + "~/" + "$3",
				                     RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);
			}
			catch (HttpException)
			{
				return input;
			}
		}
	}
}