// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OpenQA.Selenium;

namespace Tp.Integration.Testing.Common.Selenium
{
	public static class WebElementExtensions
	{
		public static bool HasClass(this IWebElement element, string className)
		{
			return element.GetAttribute("class").Contains(className);
		}

		public static string GetId(this IWebElement element)
		{
			return element.GetAttribute("id");
		}

		public static string[] GetOptions(this IWebElement element)
		{
			return element
				.FindElements(By.TagName("option"))
				.Select(x => x.Text)
				.ToArray();
		}

		public static ReadOnlyCollection<IWebElement> FindChildren(this IWebElement element)
		{
			return element.FindElements(By.CssSelector("*"));
		}
	}
}