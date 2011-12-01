// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Tp.Integration.Testing.Common.Selenium
{
    public static class BrowserExtensions
    {
        public static void WaitForAjax(this IBrowser browser, int timeout)
        {
        	WaitForAjax(browser, TimeSpan.FromMilliseconds(timeout));
        }

    	private static void WaitForAjax(this IBrowser browser, TimeSpan timeout)
    	{
    		const string locator = "//div[contains(@id,'atlProgress') and contains(@style,'none')]";
    		Thread.Sleep(100);
    		WaitFor(browser, x => x.FindElements(By.XPath(locator)).Count > 0, timeout);
    	}

    	public static void WaitFor(this IBrowser browser, Func<IBrowser, bool> condition, TimeSpan timeout)
    	{
    		new WebDriverWait(browser, timeout).Until(x => condition(browser));
    	}

    	public static void WaitForPageToLoad(this IBrowser browser, string timeout)
        {
			var timeoutSpan = TimeSpan.FromMilliseconds(double.Parse(timeout));
    		try
    		{
				if (browser.FindElements(By.XPath("//div[contains(@id,'atlProgress')")).Count > 0)
				{
					browser.WaitForAjax(timeoutSpan);
				}
    		}
    		catch (InvalidOperationException e)
    		{
    			Trace.TraceInformation("Exception has been thrown but ignored as browser can be loading the page at that time: {0}", e);
    		}
    		browser.WaitForPageToLoad(timeoutSpan);
		}

    	private static void WaitForPageToLoad(this IBrowser browser, TimeSpan timeoutSpan)
    	{
    		browser.WaitFor(x => x.ExecuteScript("return document.readyState;").Equals("complete"), timeoutSpan);
    	}

    	public static IWebElement FindLabelFor(this IBrowser browser, IWebElement x)
    	{
    		return browser.FindElements(WebDriverSelectors.LabelFor(x)).SingleOrDefault();
    	}

		public static void WaitForElement(this IBrowser browser, By @by, TimeSpan timeout)
		{
			browser.WaitForElement(by, timeout, false);
		}

		public static void WaitForElement(this IBrowser browser, By @by, TimeSpan timeout, bool disapear)
		{
			browser.WaitFor(x => (x.FindElements(@by).Count > 0 && !disapear) || (x.FindElements(@by).Count == 0 && disapear), timeout);
		}
    }
}
