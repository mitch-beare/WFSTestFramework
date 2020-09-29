using OpenQA.Selenium;

namespace WFSTestFramework.ComponentHelper
{
    public class ButtonHelper
    {
        private static IWebElement _element;

        public static void ClickButton(By locator)
        {
            _element = GenericHelper.GetElement(locator);
            _element.Click();
        }

        public static bool IsButtonEnabled(By locator)
        {
            _element = GenericHelper.GetElement(locator);
            return _element.Enabled;
        }

        public static string GetButtonText(By locator)
        {
            _element = GenericHelper.GetElement(locator);
            return _element.GetAttribute("value") == null ? " " : _element.GetAttribute("value");
        }
    }
}