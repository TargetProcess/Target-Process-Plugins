// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using OpenQA.Selenium;

namespace Tp.Integration.Testing.Common.Selenium
{
	public static class WebDriverSelectors
	{
		public static readonly By CheckBox = By.CssSelector("input[type=checkbox]");
		public static By LabelFor(IWebElement x)
		{
			return By.CssSelector(String.Format("label[for={0}]", x.GetId()));
		}
	}
}
