// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using Selenium;
using StructureMap;

namespace Tp.Integration.Testing.Common.Selenium
{
    public class WebDriverToSeleniumAdapter : ISelenium
    {
        private IBrowser _browser;

        public WebDriverToSeleniumAdapter()
        {
            _browser = ObjectFactory.GetInstance<IBrowser>();
        }

        public void SetExtensionJs(string extensionJs)
        {
            throw new System.NotImplementedException();
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public void Click(string locator)
        {
            _browser.FindElement(By.XPath(locator)).Click();
        }

        public void DoubleClick(string locator)
        {
            throw new System.NotImplementedException();
        }

        public void ContextMenu(string locator)
        {
            throw new System.NotImplementedException();
        }

        public void ClickAt(string locator, string coordString)
        {
            throw new System.NotImplementedException();
        }

        public void DoubleClickAt(string locator, string coordString)
        {
            throw new System.NotImplementedException();
        }

        public void ContextMenuAt(string locator, string coordString)
        {
            throw new System.NotImplementedException();
        }

        public void FireEvent(string locator, string eventName)
        {
            throw new System.NotImplementedException();
        }

        public void Focus(string locator)
        {
            throw new System.NotImplementedException();
        }

        public void KeyPress(string locator, string keySequence)
        {
            throw new System.NotImplementedException();
        }

        public void ShiftKeyDown()
        {
            throw new System.NotImplementedException();
        }

        public void ShiftKeyUp()
        {
            throw new System.NotImplementedException();
        }

        public void MetaKeyDown()
        {
            throw new System.NotImplementedException();
        }

        public void MetaKeyUp()
        {
            throw new System.NotImplementedException();
        }

        public void AltKeyDown()
        {
            throw new System.NotImplementedException();
        }

        public void AltKeyUp()
        {
            throw new System.NotImplementedException();
        }

        public void ControlKeyDown()
        {
            throw new System.NotImplementedException();
        }

        public void ControlKeyUp()
        {
            throw new System.NotImplementedException();
        }

        public void KeyDown(string locator, string keySequence)
        {
            throw new System.NotImplementedException();
        }

        public void KeyUp(string locator, string keySequence)
        {
            throw new System.NotImplementedException();
        }

        public void MouseOver(string locator)
        {
            throw new System.NotImplementedException();
        }

        public void MouseOut(string locator)
        {
            throw new System.NotImplementedException();
        }

        public void MouseDown(string locator)
        {
			Click(locator);
		}

    	public void MouseDownRight(string locator)
        {
            throw new System.NotImplementedException();
        }

        public void MouseDownAt(string locator, string coordString)
        {
            throw new System.NotImplementedException();
        }

        public void MouseDownRightAt(string locator, string coordString)
        {
            throw new System.NotImplementedException();
        }

        public void MouseUp(string locator)
        {
		}

        public void MouseUpRight(string locator)
        {
            throw new System.NotImplementedException();
        }

        public void MouseUpAt(string locator, string coordString)
        {
            throw new System.NotImplementedException();
        }

        public void MouseUpRightAt(string locator, string coordString)
        {
            throw new System.NotImplementedException();
        }

        public void MouseMove(string locator)
        {
            throw new System.NotImplementedException();
        }

        public void MouseMoveAt(string locator, string coordString)
        {
            throw new System.NotImplementedException();
        }

        public void Type(string locator, string value)
        {
            throw new System.NotImplementedException();
        }

        public void TypeKeys(string locator, string value)
        {
            throw new System.NotImplementedException();
        }

        public void SetSpeed(string value)
        {
            throw new System.NotImplementedException();
        }

        public string GetSpeed()
        {
            throw new System.NotImplementedException();
        }

        public void Check(string locator)
        {
            throw new System.NotImplementedException();
        }

        public void Uncheck(string locator)
        {
            throw new System.NotImplementedException();
        }

        public void Select(string selectLocator, string optionLocator)
        {
            throw new System.NotImplementedException();
        }

        public void AddSelection(string locator, string optionLocator)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveSelection(string locator, string optionLocator)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveAllSelections(string locator)
        {
            throw new System.NotImplementedException();
        }

        public void Submit(string formLocator)
        {
            throw new System.NotImplementedException();
        }

        public void Open(string url)
        {
            _browser.Navigate().GoToUrl(url);
        }

        public void OpenWindow(string url, string windowID)
        {
            throw new System.NotImplementedException();
        }

        public void SelectWindow(string windowID)
        {
            throw new System.NotImplementedException();
        }

        public void SelectPopUp(string windowID)
        {
            throw new System.NotImplementedException();
        }

        public void DeselectPopUp()
        {
            throw new System.NotImplementedException();
        }

        public void SelectFrame(string locator)
        {
            throw new System.NotImplementedException();
        }

        public bool GetWhetherThisFrameMatchFrameExpression(string currentFrameString, string target)
        {
            throw new System.NotImplementedException();
        }

        public bool GetWhetherThisWindowMatchWindowExpression(string currentWindowString, string target)
        {
            throw new System.NotImplementedException();
        }

        public void WaitForPopUp(string windowID, string timeout)
        {
            throw new System.NotImplementedException();
        }

        public void ChooseCancelOnNextConfirmation()
        {
            throw new System.NotImplementedException();
        }

        public void ChooseOkOnNextConfirmation()
        {
            throw new System.NotImplementedException();
        }

        public void AnswerOnNextPrompt(string answer)
        {
            throw new System.NotImplementedException();
        }

        public void GoBack()
        {
            throw new System.NotImplementedException();
        }

        public void Refresh()
        {
            throw new System.NotImplementedException();
        }

        public void Close()
        {
            throw new System.NotImplementedException();
        }

        public bool IsAlertPresent()
        {
            throw new System.NotImplementedException();
        }

        public bool IsPromptPresent()
        {
            throw new System.NotImplementedException();
        }

        public bool IsConfirmationPresent()
        {
            throw new System.NotImplementedException();
        }

        public string GetAlert()
        {
            throw new System.NotImplementedException();
        }

        public string GetConfirmation()
        {
            throw new System.NotImplementedException();
        }

        public string GetPrompt()
        {
            throw new System.NotImplementedException();
        }

        public string GetLocation()
        {
            throw new System.NotImplementedException();
        }

        public string GetTitle()
        {
            return _browser.Title;
        }

        public string GetBodyText()
        {
            return _browser.FindElement(By.TagName("body")).Text;
        }

        public string GetValue(string locator)
        {
			if (locator.Contains(@"/@"))
			{
				var parts = locator.Split(new[] {@"/@"}, StringSplitOptions.None);
				return GetAttributeValue(parts[0], parts[1]);
			}
        	return _browser.FindElement(By.XPath(locator)).Text;
        }

    	private string GetAttributeValue(string locator, string attributeName)
    	{
    		return _browser.FindElement(By.XPath(locator)).GetAttribute(attributeName);
    	}

    	public string GetText(string locator)
        {
			return _browser.FindElement(By.XPath(locator)).Text;
        }

        public void Highlight(string locator)
        {
            throw new System.NotImplementedException();
        }

        public string GetEval(string script)
        {
            return (string) _browser.ExecuteScript(script, new object[0]);
        }

        public bool IsChecked(string locator)
        {
            throw new System.NotImplementedException();
        }

        public string GetTable(string tableCellAddress)
        {
            throw new System.NotImplementedException();
        }

        public string[] GetSelectedLabels(string selectLocator)
        {
            throw new System.NotImplementedException();
        }

        public string GetSelectedLabel(string selectLocator)
        {
            throw new System.NotImplementedException();
        }

        public string[] GetSelectedValues(string selectLocator)
        {
            throw new System.NotImplementedException();
        }

        public string GetSelectedValue(string selectLocator)
        {
            throw new System.NotImplementedException();
        }

        public string[] GetSelectedIndexes(string selectLocator)
        {
            throw new System.NotImplementedException();
        }

        public string GetSelectedIndex(string selectLocator)
        {
            throw new System.NotImplementedException();
        }

        public string[] GetSelectedIds(string selectLocator)
        {
            throw new System.NotImplementedException();
        }

        public string GetSelectedId(string selectLocator)
        {
            throw new System.NotImplementedException();
        }

        public bool IsSomethingSelected(string selectLocator)
        {
            throw new System.NotImplementedException();
        }

        public string[] GetSelectOptions(string selectLocator)
        {
            throw new System.NotImplementedException();
        }

        public string GetAttribute(string attributeLocator)
        {
            throw new System.NotImplementedException();
        }

        public bool IsTextPresent(string pattern)
        {
            return _browser.FindElement(By.TagName("html")).Text.Contains(pattern);
        }

        public bool IsElementPresent(string locator)
        {
            // TODO: maybe wait some time when element is appeared
            return _browser.FindElements(By.XPath(locator)).Count > 0;
        }

        public bool IsVisible(string locator)
        {
            throw new System.NotImplementedException();
        }

        public bool IsEditable(string locator)
        {
            throw new System.NotImplementedException();
        }

        public string[] GetAllButtons()
        {
            throw new System.NotImplementedException();
        }

        public string[] GetAllLinks()
        {
            throw new System.NotImplementedException();
        }

        public string[] GetAllFields()
        {
            throw new System.NotImplementedException();
        }

        public string[] GetAttributeFromAllWindows(string attributeName)
        {
            throw new System.NotImplementedException();
        }

        public void Dragdrop(string locator, string movementsString)
        {
            throw new System.NotImplementedException();
        }

        public void SetMouseSpeed(string pixels)
        {
            throw new System.NotImplementedException();
        }

        public decimal GetMouseSpeed()
        {
            throw new System.NotImplementedException();
        }

        public void DragAndDrop(string locator, string movementsString)
        {
            throw new System.NotImplementedException();
        }

        public void DragAndDropToObject(string locatorOfObjectToBeDragged, string locatorOfDragDestinationObject)
        {
            throw new System.NotImplementedException();
        }

        public void WindowFocus()
        {
            throw new System.NotImplementedException();
        }

        public void WindowMaximize()
        {
            throw new System.NotImplementedException();
        }

        public string[] GetAllWindowIds()
        {
            throw new System.NotImplementedException();
        }

        public string[] GetAllWindowNames()
        {
            throw new System.NotImplementedException();
        }

        public string[] GetAllWindowTitles()
        {
            throw new System.NotImplementedException();
        }

        public string GetHtmlSource()
        {
            throw new System.NotImplementedException();
        }

        public void SetCursorPosition(string locator, string position)
        {
            throw new System.NotImplementedException();
        }

        public decimal GetElementIndex(string locator)
        {
            throw new System.NotImplementedException();
        }

        public bool IsOrdered(string locator1, string locator2)
        {
            throw new System.NotImplementedException();
        }

        public decimal GetElementPositionLeft(string locator)
        {
            throw new System.NotImplementedException();
        }

        public decimal GetElementPositionTop(string locator)
        {
            throw new System.NotImplementedException();
        }

        public decimal GetElementWidth(string locator)
        {
            throw new System.NotImplementedException();
        }

        public decimal GetElementHeight(string locator)
        {
            throw new System.NotImplementedException();
        }

        public decimal GetCursorPosition(string locator)
        {
            throw new System.NotImplementedException();
        }

        public string GetExpression(string expression)
        {
            throw new System.NotImplementedException();
        }

        public decimal GetXpathCount(string xpath)
        {
            throw new System.NotImplementedException();
        }

        public void AssignId(string locator, string identifier)
        {
            throw new System.NotImplementedException();
        }

        public void AllowNativeXpath(string allow)
        {
            throw new System.NotImplementedException();
        }

        public void IgnoreAttributesWithoutValue(string ignore)
        {
            throw new System.NotImplementedException();
        }

        public void WaitForCondition(string script, string timeout)
        {
			TimeSpan timeoutSpan = TimeSpan.FromMilliseconds(double.Parse(timeout));
        	script = script.TrimStart();
			
			bool disapear = false;
			if (script.StartsWith("!"))
			{
				script = script.TrimStart('!');
				disapear = true;
			}
			
			if (script.StartsWith("selenium.isElementPresent("))
			{
				var parts = script.Split('"');
				var xpath = parts[1];
				_browser.WaitForElement(By.XPath(xpath), timeoutSpan, disapear);
			}
			else
			{
				throw new InvalidOperationException("I don't know yet how to process this script");
			}
        }

        public void SetTimeout(string timeout)
        {
            throw new System.NotImplementedException();
        }

        public void WaitForPageToLoad(string timeout)
        {
			_browser.WaitForPageToLoad(timeout);
        }

        public void WaitForFrameToLoad(string frameAddress, string timeout)
        {
            throw new System.NotImplementedException();
        }

        public string GetCookie()
        {
            throw new System.NotImplementedException();
        }

        public string GetCookieByName(string name)
        {
            throw new System.NotImplementedException();
        }

        public bool IsCookiePresent(string name)
        {
            throw new System.NotImplementedException();
        }

        public void CreateCookie(string nameValuePair, string optionsString)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteCookie(string name, string optionsString)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteAllVisibleCookies()
        {
            throw new System.NotImplementedException();
        }

        public void SetBrowserLogLevel(string logLevel)
        {
            throw new System.NotImplementedException();
        }

        public void RunScript(string script)
        {
            throw new System.NotImplementedException();
        }

        public void AddLocationStrategy(string strategyName, string functionDefinition)
        {
            throw new System.NotImplementedException();
        }

        public void CaptureEntirePageScreenshot(string filename, string kwargs)
        {
            throw new System.NotImplementedException();
        }

        public void Rollup(string rollupName, string kwargs)
        {
            throw new System.NotImplementedException();
        }

        public void AddScript(string scriptContent, string scriptTagId)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveScript(string scriptTagId)
        {
            throw new System.NotImplementedException();
        }

        public void UseXpathLibrary(string libraryName)
        {
            throw new System.NotImplementedException();
        }

        public void SetContext(string context)
        {
            throw new System.NotImplementedException();
        }

        public void AttachFile(string fieldLocator, string fileLocator)
        {
            throw new System.NotImplementedException();
        }

        public void CaptureScreenshot(string filename)
        {
            throw new System.NotImplementedException();
        }

        public string CaptureScreenshotToString()
        {
            throw new System.NotImplementedException();
        }

        public string CaptureEntirePageScreenshotToString(string kwargs)
        {
            throw new System.NotImplementedException();
        }

        public void ShutDownSeleniumServer()
        {
            throw new System.NotImplementedException();
        }

        public string RetrieveLastRemoteControlLogs()
        {
            throw new System.NotImplementedException();
        }

        public void KeyDownNative(string keycode)
        {
            throw new System.NotImplementedException();
        }

        public void KeyUpNative(string keycode)
        {
            throw new System.NotImplementedException();
        }

        public void KeyPressNative(string keycode)
        {
            throw new System.NotImplementedException();
        }
    }
}