using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using System;
using WFSTestFramework.Settings;

namespace WFSTestFramework.ComponentHelper
{
    internal class GenericHelper
    {
        public static bool IsElementPresent(By locator) => ObjectRepository.Driver.FindElements(locator).Count == 1;

        public static IWebElement GetElement(By locator) => IsElementPresent(locator)
            ? ObjectRepository.Driver.FindElement(locator)
            : throw new NoSuchElementException("Element not found : " + locator);

        public static void HighlightElement(IWebElement element)
        {
            try
            {
                IJavaScriptExecutor js = (IJavaScriptExecutor)ObjectRepository.Driver;
                js.ExecuteScript("arguments[0].style.border='3px solid red'",
                    element);
            }
            catch (StaleElementReferenceException)
            {
            }
        }

        public static void UnhighlightElement(IWebElement element)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)ObjectRepository.Driver;
            try
            {
                js.ExecuteScript("arguments[0].style.border='0px'",
                    element);
            }
            catch (StaleElementReferenceException)
            {
            }
        }

        public static void TakeScreenShot(string filename = "Screen")
        {
            Screenshot screen = ObjectRepository.Driver.TakeScreenshot();
            if (filename.Equals("Screen"))
            {
                string name = filename + DateTime.UtcNow.ToString("yyyy-MM-dd-ss") + "jpeg";
                screen.SaveAsFile(name, ScreenshotImageFormat.Jpeg);
                return;
            }
            screen.SaveAsFile(filename, ScreenshotImageFormat.Jpeg);
        }

        public static bool WaitForWebElement(By locator, TimeSpan timeout)
        {
            ObjectRepository.Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);
            WebDriverWait wait = new WebDriverWait(ObjectRepository.Driver, timeout)
            {
                PollingInterval = TimeSpan.FromMilliseconds(500)
            };
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotVisibleException));
            return wait.Until(x => x.FindElements(locator).Count == 1);
        }

        public static IWebElement WaitForWebElementInPage(By locator, TimeSpan timeout)
        {
            ObjectRepository.Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);
            WebDriverWait wait = new WebDriverWait(ObjectRepository.Driver, timeout)
            {
                PollingInterval = TimeSpan.FromMilliseconds(500)
            };
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotVisibleException));
            return wait.Until(x => x.FindElements(locator).Count == 1 ? x.FindElement(locator) : null);
        }
    }
}